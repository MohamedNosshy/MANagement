using System.Diagnostics;
using Mangement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Mangement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CompContext _context;
        private readonly IMemoryCache _cache;

        public HomeController(ILogger<HomeController> logger, CompContext context, IMemoryCache cache)
        {
            _logger = logger;
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            // Cache dashboard statistics
            var cacheKey = "DashboardStats";
            if (!_cache.TryGetValue(cacheKey, out (int employees, int departments, int tasks, int leaves) stats))
            {
                stats = (
                    await _context.Users.CountAsync(),
                    await _context.Departments.CountAsync(),
                    await _context.EmployeeTasks.Where(t => t.Status != "Completed").CountAsync(),
                    await _context.Leaves.Where(l => l.Status == "Pending").CountAsync()
                );

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5));
                _cache.Set(cacheKey, stats, cacheOptions);
            }

            ViewBag.TotalEmployees = stats.employees;
            ViewBag.TotalDepartments = stats.departments;
            ViewBag.ActiveTasks = stats.tasks;
            ViewBag.PendingLeaves = stats.leaves;

            // Cache recent activities
            var recentCacheKey = "RecentActivities";
            if (!_cache.TryGetValue(recentCacheKey, out (List<EmployeeTask> tasks, List<Leave> leaves, List<Salary> salaries) recent))
            {
                recent = (
                    await _context.EmployeeTasks
                        .Include(t => t.AssignedTo)
                        .OrderByDescending(t => t.CreatedAt)
                        .Take(5)
                        .ToListAsync(),
                    await _context.Leaves
                        .Include(l => l.User)
                        .OrderByDescending(l => l.RequestDate)
                        .Take(5)
                        .ToListAsync(),
                    await _context.Salaries
                        .Include(s => s.User)
                        .OrderByDescending(s => s.PaymentDate)
                        .Take(5)
                        .ToListAsync()
                );

                var recentCacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(2));
                _cache.Set(recentCacheKey, recent, recentCacheOptions);
            }

            ViewBag.RecentTasks = recent.tasks;
            ViewBag.RecentLeaves = recent.leaves;
            ViewBag.RecentSalaries = recent.salaries;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult NotFound()
        {
            return View();
        }
    }
}

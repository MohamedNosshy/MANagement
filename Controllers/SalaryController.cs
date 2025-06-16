using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mangement.Models;

namespace Mangement.Controllers
{
    public class SalaryController : Controller
    {
        private readonly CompContext _context;

        public SalaryController(CompContext context)
        {
            _context = context;
        }

        // عرض قائمة المرتبات
        public async Task<IActionResult> Index()
        {
            var salaries = await _context.Salaries
                .Include(s => s.User)
                .OrderByDescending(s => s.PaymentDate)
                .ToListAsync();
            return View(salaries);
        }

        // عرض نموذج مرتب جديد
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = await _context.Users.ToListAsync();
            return View();
        }

        // حفظ مرتب جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Salary salary)
        {
            if (ModelState.IsValid)
            {
                // حساب إجمالي المرتب
                salary.TotalSalary = salary.BaseSalary;
                if (salary.Allowances.HasValue)
                    salary.TotalSalary += salary.Allowances.Value;
                if (salary.Deductions.HasValue)
                    salary.TotalSalary -= salary.Deductions.Value;

                _context.Add(salary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = await _context.Users.ToListAsync();
            return View(salary);
        }

        // تحديث حالة الدفع
        [HttpPost]
        public async Task<IActionResult> UpdatePaymentStatus(int id)
        {
            var salary = await _context.Salaries.FindAsync(id);
            if (salary == null)
            {
                return NotFound();
            }

            salary.IsPaid = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // عرض تقرير المرتبات
        public async Task<IActionResult> Report(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Salaries
                .Include(s => s.User)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(s => s.PaymentDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(s => s.PaymentDate <= endDate.Value);

            var salaries = await query.ToListAsync();
            return View(salaries);
        }
    }
} 
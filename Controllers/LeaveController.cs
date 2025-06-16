using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mangement.Models;

namespace Mangement.Controllers
{
    public class LeaveController : Controller
    {
        private readonly CompContext _context;

        public LeaveController(CompContext context)
        {
            _context = context;
        }

        // عرض قائمة الإجازات
        public async Task<IActionResult> Index()
        {
            var leaves = await _context.Leaves
                .Include(l => l.User)
                .OrderByDescending(l => l.RequestDate)
                .ToListAsync();
            return View(leaves);
        }

        // عرض نموذج طلب إجازة جديد
        public IActionResult Create()
        {
            return View();
        }

        // حفظ طلب إجازة جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Leave leave)
        {
            if (ModelState.IsValid)
            {
                // هنا يجب إضافة معرف المستخدم الحالي
                leave.UserId = 1; // مؤقتاً
                leave.RequestDate = DateTime.Now;
                leave.Status = "Pending";

                _context.Add(leave);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leave);
        }

        // الموافقة على طلب إجازة
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null)
            {
                return NotFound();
            }

            leave.Status = "Approved";
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // رفض طلب إجازة
        [HttpPost]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null)
            {
                return NotFound();
            }

            leave.Status = "Rejected";
            leave.RejectionReason = reason;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
} 
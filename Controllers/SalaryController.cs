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
            try
            {
                var salaries = await _context.Salaries
                    .Include(s => s.User)
                    .OrderByDescending(s => s.PaymentDate)
                    .ToListAsync();
                return View(salaries);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading salaries.";
                return View(new List<Salary>());
            }
        }

        // عرض نموذج مرتب جديد
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.Users = await _context.Users.ToListAsync();
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading the form.";
                return RedirectToAction(nameof(Index));
            }
        }

        // حفظ مرتب جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Salary salary)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // حساب إجمالي المرتب
                    salary.TotalSalary = salary.BaseSalary;
                    if (salary.Allowances.HasValue)
                        salary.TotalSalary += salary.Allowances.Value;
                    if (salary.Deductions.HasValue)
                        salary.TotalSalary -= salary.Deductions.Value;

                    salary.CreatedAt = DateTime.Now;
                    salary.PaymentDate = DateTime.Now;

                    _context.Add(salary);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Salary record created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                
                ViewBag.Users = await _context.Users.ToListAsync();
                return View(salary);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while creating the salary record.";
                ViewBag.Users = await _context.Users.ToListAsync();
                return View(salary);
            }
        }

        // عرض تفاصيل المرتب
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var salary = await _context.Salaries
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (salary == null)
                {
                    return NotFound();
                }

                return View(salary);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading salary details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // تحديث حالة الدفع
        [HttpPost]
        public async Task<IActionResult> UpdatePaymentStatus(int id)
        {
            try
            {
                var salary = await _context.Salaries.FindAsync(id);
                if (salary == null)
                {
                    return NotFound();
                }

                salary.IsPaid = true;
                salary.PaymentDate = DateTime.Now;
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Payment status updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while updating payment status.";
                return RedirectToAction(nameof(Index));
            }
        }

        // عرض تقرير المرتبات
        public async Task<IActionResult> Report(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var query = _context.Salaries
                    .Include(s => s.User)
                    .AsQueryable();

                if (startDate.HasValue)
                    query = query.Where(s => s.PaymentDate >= startDate.Value);
                if (endDate.HasValue)
                    query = query.Where(s => s.PaymentDate <= endDate.Value);

                var salaries = await query.ToListAsync();
                
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;
                
                return View(salaries);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while generating the report.";
                return View(new List<Salary>());
            }
        }

        // حذف سجل المرتب
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var salary = await _context.Salaries
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (salary == null)
                {
                    return NotFound();
                }

                return View(salary);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading delete confirmation.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var salary = await _context.Salaries.FindAsync(id);
                if (salary == null)
                {
                    return NotFound();
                }

                _context.Salaries.Remove(salary);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Salary record deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the salary record.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mangement.Models;

namespace Mangement.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly CompContext _context;

        public DepartmentController(CompContext context)
        {
            _context = context;
        }

        // عرض جميع الأقسام
        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments.ToListAsync();
            return View(departments);
        }

        // تفاصيل قسم
        public async Task<IActionResult> Details(int id)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (department == null)
                return NotFound();
            return View(department);
        }

        // عرض نموذج إضافة قسم
        public IActionResult Create()
        {
            return View();
        }

        // إضافة قسم جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // عرض نموذج تعديل قسم
        public async Task<IActionResult> Edit(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return NotFound();
            return View(department);
        }

        // تعديل قسم
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Department department)
        {
            if (id != department.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                _context.Entry(department).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        // عرض تأكيد حذف قسم
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (department == null)
                return NotFound();
            return View(department);
        }

        // حذف قسم
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return NotFound();
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
} 
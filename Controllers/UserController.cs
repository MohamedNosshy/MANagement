using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mangement.Models;

namespace Mangement.Controllers
{
    public class UserController : Controller
    {
        private readonly CompContext _context;

        public UserController(CompContext context)
        {
            _context = context;
        }

        // عرض جميع الموظفين
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.Include(u => u.Department).ToListAsync();
            return View(users);
        }

        // تفاصيل موظف
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users.Include(u => u.Department).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        // عرض نموذج إضافة موظف
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = await _context.Departments.ToListAsync();
            return View();
        }

        // إضافة موظف جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = await _context.Departments.ToListAsync();
            return View(user);
        }

        // عرض نموذج تعديل موظف
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            ViewBag.Departments = await _context.Departments.ToListAsync();
            return View(user);
        }

        // تعديل موظف
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Departments = await _context.Departments.ToListAsync();
            return View(user);
        }

        // عرض تأكيد حذف موظف
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.Include(u => u.Department).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
                return NotFound();
            return View(user);
        }

        // حذف موظف
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
} 
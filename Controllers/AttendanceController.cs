using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mangement.Models;

namespace Mangement.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly CompContext _context;

        public AttendanceController(CompContext context)
        {
            _context = context;
        }

        // عرض جميع الحضور
        public async Task<IActionResult> Index()
        {
            var attendances = await _context.Attendances.Include(a => a.User).ToListAsync();
            return View(attendances);
        }

        // تفاصيل حضور
        public async Task<IActionResult> Details(int id)
        {
            var attendance = await _context.Attendances.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == id);
            if (attendance == null)
                return NotFound();
            return View(attendance);
        }

        // عرض نموذج إضافة حضور
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = await _context.Users.ToListAsync();
            return View();
        }

        // إضافة حضور جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Attendance attendance)
        {
            if (ModelState.IsValid)
            {
                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = await _context.Users.ToListAsync();
            return View(attendance);
        }

        // عرض نموذج تعديل حضور
        public async Task<IActionResult> Edit(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
                return NotFound();
            ViewBag.Users = await _context.Users.ToListAsync();
            return View(attendance);
        }

        // تعديل حضور
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Attendance attendance)
        {
            if (id != attendance.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                _context.Entry(attendance).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Users = await _context.Users.ToListAsync();
            return View(attendance);
        }

        // عرض تأكيد حذف حضور
        public async Task<IActionResult> Delete(int id)
        {
            var attendance = await _context.Attendances.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == id);
            if (attendance == null)
                return NotFound();
            return View(attendance);
        }

        // حذف حضور
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
                return NotFound();
            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
} 
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
            try
            {
                var leaves = await _context.Leaves
                    .Include(l => l.User)
                    .OrderByDescending(l => l.RequestDate)
                    .ToListAsync();
                return View(leaves);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading leaves.";
                return View(new List<Leave>());
            }
        }

        // عرض نموذج طلب إجازة جديد
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

        // حفظ طلب إجازة جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Leave leave)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Get first user as default (for demo purposes)
                    var users = await _context.Users.ToListAsync();
                    var currentUser = users.FirstOrDefault();
                    
                    if (currentUser == null)
                    {
                        TempData["Error"] = "No users found in the system.";
                        ViewBag.Users = new List<User>();
                        return View(leave);
                    }

                    leave.UserId = currentUser.Id;
                    leave.RequestDate = DateTime.Now;
                    leave.Status = "Pending";

                    _context.Add(leave);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Leave request created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                
                ViewBag.Users = await _context.Users.ToListAsync();
                return View(leave);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while creating the leave request.";
                ViewBag.Users = await _context.Users.ToListAsync();
                return View(leave);
            }
        }

        // عرض تفاصيل الإجازة
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var leave = await _context.Leaves
                    .Include(l => l.User)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (leave == null)
                {
                    return NotFound();
                }

                return View(leave);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading leave details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // الموافقة على طلب إجازة
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var leave = await _context.Leaves.FindAsync(id);
                if (leave == null)
                {
                    return NotFound();
                }

                leave.Status = "Approved";
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Leave request approved successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while approving the leave request.";
                return RedirectToAction(nameof(Index));
            }
        }

        // رفض طلب إجازة
        [HttpPost]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            try
            {
                var leave = await _context.Leaves.FindAsync(id);
                if (leave == null)
                {
                    return NotFound();
                }

                leave.Status = "Rejected";
                leave.RejectionReason = reason;
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Leave request rejected successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while rejecting the leave request.";
                return RedirectToAction(nameof(Index));
            }
        }

        // حذف طلب إجازة
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var leave = await _context.Leaves
                    .Include(l => l.User)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (leave == null)
                {
                    return NotFound();
                }

                return View(leave);
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
                var leave = await _context.Leaves.FindAsync(id);
                if (leave == null)
                {
                    return NotFound();
                }

                _context.Leaves.Remove(leave);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Leave request deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the leave request.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 
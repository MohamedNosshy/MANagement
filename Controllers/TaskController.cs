using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Mangement.Models;
using Mangement.Interfaces;
using Mangement.ViewModels;

namespace Mangement.Controllers
{
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(
            ITaskService taskService,
            IUserService userService,
            ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity?.Name))
                {
                    return RedirectToAction("Login", "Account");
                }
                var currentUser = await _userService.GetUserByEmailAsync(User.Identity.Name!);
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                IEnumerable<EmployeeTask> tasks;
                if (currentUser.isAdmin)
                {
                    tasks = await _taskService.GetAllTasksAsync();
                }
                else
                {
                    tasks = await _taskService.GetTasksByUserAsync(currentUser.Id);
                }

                var viewModels = tasks.Select(t => new TaskViewModel
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    Comments = t.Comments,
                    CreatedAt = t.CreatedAt,
                    CompletedAt = t.CompletedAt,
                    AssignedToId = t.AssignedToId,
                    CreatedById = t.CreatedById,
                    AssignedTo = t.AssignedTo,
                    CreatedBy = t.CreatedBy
                });

                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching tasks");
                TempData["Error"] = "An error occurred while loading tasks. Please try again.";
                return View(new List<TaskViewModel>());
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity?.Name))
                {
                    return RedirectToAction("Login", "Account");
                }
                var currentUser = await _userService.GetUserByEmailAsync(User.Identity.Name!);
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                IEnumerable<User> users;
                if (currentUser.isAdmin)
                {
                    users = await _userService.GetAllUsersAsync();
                }
                else
                {
                    users = await _userService.GetUsersByDepartmentAsync(currentUser.DepartmentId);
                }

                var viewModel = new TaskViewModel
                {
                    AvailableUsers = users
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading create task form");
                TempData["Error"] = "An error occurred while loading the form. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskViewModel viewModel)
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity?.Name))
                {
                    return RedirectToAction("Login", "Account");
                }
                var currentUser = await _userService.GetUserByEmailAsync(User.Identity.Name!);
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (ModelState.IsValid)
                {
                    var task = new EmployeeTask
                    {
                        Title = viewModel.Title,
                        Description = viewModel.Description,
                        AssignedToId = viewModel.AssignedToId,
                        CreatedById = currentUser.Id
                    };

                    await _taskService.CreateTaskAsync(task);
                    
                    TempData["Success"] = "Task created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                IEnumerable<User> users;
                if (currentUser.isAdmin)
                {
                    users = await _userService.GetAllUsersAsync();
                }
                else
                {
                    users = await _userService.GetUsersByDepartmentAsync(currentUser.DepartmentId);
                }

                viewModel.AvailableUsers = users;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating task");
                TempData["Error"] = "An error occurred while creating the task. Please try again.";
                return View(viewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity?.Name))
                {
                    return RedirectToAction("Login", "Account");
                }
                var currentUser = await _userService.GetUserByEmailAsync(User.Identity.Name!);
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound();
                }

                // Check if user has permission to update status
                if (!currentUser.isAdmin && task.AssignedToId != currentUser.Id && task.CreatedById != currentUser.Id)
                {
                    TempData["Error"] = "You don't have permission to update this task's status.";
                    return RedirectToAction(nameof(Index));
                }

                await _taskService.UpdateTaskStatusAsync(id, status);
                TempData["Success"] = "Task status updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating task status");
                TempData["Error"] = "An error occurred while updating the task status. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int id, string comment)
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity?.Name))
                {
                    return RedirectToAction("Login", "Account");
                }
                var currentUser = await _userService.GetUserByEmailAsync(User.Identity.Name!);
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound();
                }

                // Check if user has permission to add comment
                if (!currentUser.isAdmin && task.AssignedToId != currentUser.Id && task.CreatedById != currentUser.Id)
                {
                    TempData["Error"] = "You don't have permission to add comments to this task.";
                    return RedirectToAction(nameof(Index));
                }

                await _taskService.AddTaskCommentAsync(id, comment);
                TempData["Success"] = "Comment added successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding comment");
                TempData["Error"] = "An error occurred while adding the comment. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 
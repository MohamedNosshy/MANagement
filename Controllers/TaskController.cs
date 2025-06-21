using Microsoft.AspNetCore.Mvc;
using Mangement.Models;
using Mangement.Interfaces;
using Mangement.ViewModels;

namespace Mangement.Controllers
{
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
                // Get first user as default (for demo purposes)
                var users = await _userService.GetAllUsersAsync();
                var currentUser = users.FirstOrDefault();
                
                if (currentUser == null)
                {
                    TempData["Error"] = "No users found in the system.";
                    return View(new List<TaskViewModel>());
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
                    Priority = t.Priority,
                    StartDate = t.StartDate,
                    DueDate = t.DueDate,
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
                var users = await _userService.GetAllUsersAsync();
                var currentUser = users.FirstOrDefault();
                
                if (currentUser == null)
                {
                    TempData["Error"] = "No users found in the system.";
                    return RedirectToAction(nameof(Index));
                }

                IEnumerable<User> availableUsers;
                if (currentUser.isAdmin)
                {
                    availableUsers = await _userService.GetAllUsersAsync();
                }
                else
                {
                    availableUsers = await _userService.GetUsersByDepartmentAsync(currentUser.DepartmentId);
                }

                var viewModel = new TaskViewModel
                {
                    AvailableUsers = availableUsers,
                    StartDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(7)
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
                var users = await _userService.GetAllUsersAsync();
                var currentUser = users.FirstOrDefault();
                
                if (currentUser == null)
                {
                    TempData["Error"] = "No users found in the system.";
                    return RedirectToAction(nameof(Index));
                }

                if (ModelState.IsValid)
                {
                    var task = new EmployeeTask
                    {
                        Title = viewModel.Title,
                        Description = viewModel.Description,
                        AssignedToId = viewModel.AssignedToId,
                        CreatedById = currentUser.Id,
                        StartDate = viewModel.StartDate,
                        DueDate = viewModel.DueDate,
                        Priority = viewModel.Priority ?? "Medium",
                        Status = "Pending"
                    };

                    await _taskService.CreateTaskAsync(task);
                    
                    TempData["Success"] = "Task created successfully.";
                    return RedirectToAction(nameof(Index));
                }

                IEnumerable<User> availableUsers;
                if (currentUser.isAdmin)
                {
                    availableUsers = await _userService.GetAllUsersAsync();
                }
                else
                {
                    availableUsers = await _userService.GetUsersByDepartmentAsync(currentUser.DepartmentId);
                }

                viewModel.AvailableUsers = availableUsers;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating task");
                TempData["Error"] = "An error occurred while creating the task. Please try again.";
                return View(viewModel);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound();
                }

                var viewModel = new TaskViewModel
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    StartDate = task.StartDate,
                    DueDate = task.DueDate,
                    Comments = task.Comments,
                    CreatedAt = task.CreatedAt,
                    CompletedAt = task.CompletedAt,
                    AssignedToId = task.AssignedToId,
                    CreatedById = task.CreatedById,
                    AssignedTo = task.AssignedTo,
                    CreatedBy = task.CreatedBy
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching task details");
                TempData["Error"] = "An error occurred while loading task details.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound();
                }

                var users = await _userService.GetAllUsersAsync();
                var currentUser = users.FirstOrDefault();
                
                IEnumerable<User> availableUsers;
                if (currentUser?.isAdmin == true)
                {
                    availableUsers = await _userService.GetAllUsersAsync();
                }
                else
                {
                    availableUsers = await _userService.GetUsersByDepartmentAsync(currentUser?.DepartmentId ?? 1);
                }

                var viewModel = new TaskViewModel
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    Priority = task.Priority,
                    StartDate = task.StartDate,
                    DueDate = task.DueDate,
                    Comments = task.Comments,
                    AssignedToId = task.AssignedToId,
                    AvailableUsers = availableUsers
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading edit task form");
                TempData["Error"] = "An error occurred while loading the edit form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskViewModel viewModel)
        {
            try
            {
                if (id != viewModel.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    var existingTask = await _taskService.GetTaskByIdAsync(id);
                    if (existingTask == null)
                    {
                        return NotFound();
                    }

                    existingTask.Title = viewModel.Title;
                    existingTask.Description = viewModel.Description;
                    existingTask.AssignedToId = viewModel.AssignedToId;
                    existingTask.StartDate = viewModel.StartDate;
                    existingTask.DueDate = viewModel.DueDate;
                    existingTask.Priority = viewModel.Priority ?? "Medium";
                    existingTask.Status = viewModel.Status;

                    await _taskService.UpdateTaskAsync(existingTask);
                    
                    TempData["Success"] = "Task updated successfully.";
                    return RedirectToAction(nameof(Index));
                }

                var users = await _userService.GetAllUsersAsync();
                viewModel.AvailableUsers = users;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating task");
                TempData["Error"] = "An error occurred while updating the task.";
                return View(viewModel);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound();
                }

                var viewModel = new TaskViewModel
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Status = task.Status,
                    AssignedTo = task.AssignedTo,
                    CreatedBy = task.CreatedBy
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading delete confirmation");
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
                await _taskService.DeleteTaskAsync(id);
                TempData["Success"] = "Task deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting task");
                TempData["Error"] = "An error occurred while deleting the task.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound();
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
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound();
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
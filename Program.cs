using Microsoft.EntityFrameworkCore;
using Mangement.Models;
using Mangement.Services;
using Mangement.Repositories;
using Mangement.Interfaces;

namespace Mangement
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
            
            // Add Application Services
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITaskService, TaskService>();
            // builder.Services.AddScoped<IAttendanceService, AttendanceService>();
            // builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            // builder.Services.AddScoped<ILeaveService, LeaveService>();
            // builder.Services.AddScoped<ISalaryService, SalaryService>();

            // Add Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ITaskRepository, TaskRepository>();
            // builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            // builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            // builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
            // builder.Services.AddScoped<ISalaryRepository, SalaryRepository>();

            // Add Authentication
            builder.Services.AddAuthentication("Cookies")
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                });

            // Add Authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy =>
                    policy.RequireRole("Admin"));
                options.AddPolicy("RequireManagerRole", policy =>
                    policy.RequireRole("Manager"));
            });

            // Add caching
            builder.Services.AddMemoryCache();
            builder.Services.AddResponseCaching();
            
            // Configure DbContext with performance optimizations
            builder.Services.AddDbContext<CompContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("cs"), 
                    sqlOptions => 
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
                options.EnableSensitiveDataLogging(false);
                options.EnableDetailedErrors(false);
            });

            // Add response compression
            builder.Services.AddResponseCompression();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            
            // Enable response caching
            app.UseResponseCaching();
            
            // Add compression
            app.UseResponseCompression();

            app.UseRouting();

            // Add Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

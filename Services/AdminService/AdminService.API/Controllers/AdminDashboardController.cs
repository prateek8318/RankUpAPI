using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;

using AdminService.Application.Interfaces;

using AdminService.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;



namespace AdminService.API.Controllers

{

    /// <summary>

    /// Admin Dashboard Controller - Real-time Overview

    /// </summary>

    [Route("api/admin/dashboard")]

    [ApiController]

    [Authorize(Roles = "Admin")]

    public class AdminDashboardController : ControllerBase

    {

        private readonly ILogger<AdminDashboardController> _logger;

        private readonly AdminDbContext _context;

        private readonly IUserServiceClient _userServiceClient;

        private readonly IExamServiceClient _examServiceClient;



        public AdminDashboardController(

            ILogger<AdminDashboardController> logger,

            AdminDbContext context,

            IUserServiceClient userServiceClient,

            IExamServiceClient examServiceClient)

        {

            _logger = logger;

            _context = context;

            _userServiceClient = userServiceClient;

            _examServiceClient = examServiceClient;

        }



        /// <summary>

        /// Get dashboard totals
        /// </summary>

        [HttpGet("totals")]

        public async Task<ActionResult> GetDashboardTotals()

        {

            try

            {

                // Get real data from database

                var totalAdmins = await _context.Admins.CountAsync();

                var activeAdmins = await _context.Admins.CountAsync(a => a.IsActive);

                

                // Try to get user data from UserService

                var totalUsers = 0;

                var activeUsers = 0;

                try

                {

                    totalUsers = await _userServiceClient.GetTotalUsersCountAsync();

                    var users = await _userServiceClient.GetAllUsersAsync(1, 10000);

                    if (users != null)

                    {

                        activeUsers = users.Count(u => u.IsActive);

                    }

                }

                catch (Exception ex)

                {

                    _logger.LogWarning(ex, "Could not fetch user data from UserService: {ErrorMessage}", ex.Message);

                }



                // Try to get exam data from ExamService

                var totalExams = 0;

                var activeExams = 0;

                try

                {

                    var exams = await _examServiceClient.GetAllExamsAsync();

                    if (exams != null)

                    {

                        totalExams = exams.Count();

                        activeExams = exams.Count(e => e.IsActive);

                    }

                }

                catch (Exception ex)

                {

                    _logger.LogWarning(ex, "Could not fetch exam data from ExamService");

                }



                var totals = new

                {

                    TotalUsers = totalUsers,

                    ActiveUsers = activeUsers,

                    TotalAdmins = totalAdmins,

                    ActiveAdmins = activeAdmins,

                    TotalExams = totalExams,

                    ActiveExams = activeExams,

                    TotalRevenue = 0, // TODO: Get from PaymentService

                    MonthlyRevenue = 0 // TODO: Get from PaymentService

                };



                return Ok(totals);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Error getting dashboard totals");

                return StatusCode(500, new { Error = "Failed to load dashboard totals" });

            }

        }



        /// <summary>

        /// Get dashboard overview

        /// </summary>

        [HttpGet("overview")]

        public async Task<ActionResult> GetDashboardOverview()

        {

            try

            {

                // Get real data from database

                var totalAdmins = await _context.Admins.CountAsync();

                var activeAdmins = await _context.Admins.CountAsync(a => a.IsActive);

                

                // Try to get user data from UserService

                var totalUsers = 0;

                var activeUsers = 0;

                try

                {

                    // Get total count first (more efficient)

                    totalUsers = await _userServiceClient.GetTotalUsersCountAsync();

                    

                    // Get all users to count active ones (you might want to optimize this with a separate endpoint)

                    var users = await _userServiceClient.GetAllUsersAsync(1, 10000); // Get large batch to count active

                    if (users != null)

                    {

                        activeUsers = users.Count(u => u.IsActive);

                    }

                }

                catch (Exception ex)

                {

                    _logger.LogWarning(ex, "Could not fetch user data from UserService: {ErrorMessage}", ex.Message);

                }



                // Try to get exam data from ExamService

                var totalExams = 0;

                var activeExams = 0;

                try

                {

                    var exams = await _examServiceClient.GetAllExamsAsync();

                    if (exams != null)

                    {

                        totalExams = exams.Count();

                        activeExams = exams.Count(e => e.IsActive);

                    }

                }

                catch (Exception ex)

                {

                    _logger.LogWarning(ex, "Could not fetch exam data from ExamService");

                }



                var overview = new

                {

                    TotalUsers = totalUsers,

                    ActiveUsers = activeUsers,

                    TotalAdmins = totalAdmins,

                    ActiveAdmins = activeAdmins,

                    TotalExams = totalExams,

                    ActiveExams = activeExams,

                    TotalRevenue = 0, // TODO: Get from PaymentService

                    MonthlyRevenue = 0, // TODO: Get from PaymentService

                    RecentActivity = await GetRecentActivities(),

                    SystemHealth = new

                    {

                        AdminService = "Healthy",

                        Database = "Connected",

                        UserService = totalUsers > 0 ? "Connected" : "Unavailable",

                        ExamService = totalExams > 0 ? "Connected" : "Unavailable",

                        LastUpdated = DateTime.UtcNow

                    }

                };



                return Ok(overview);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Error getting dashboard overview");

                return StatusCode(500, new { Error = "Failed to load dashboard data" });

            }

        }



        /// <summary>

        /// Get system statistics

        /// </summary>

        [HttpGet("stats")]

        public async Task<ActionResult> GetSystemStats()

        {

            try

            {

                // Get real admin stats

                var totalAdmins = await _context.Admins.CountAsync();

                var activeAdmins = await _context.Admins.CountAsync(a => a.IsActive);

                var newAdminsThisMonth = await _context.Admins.CountAsync(a => a.CreatedAt >= DateTime.UtcNow.AddDays(-30));



                // Try to get user stats

                var userStats = new { Total = 0, Active = 0, NewThisMonth = 0, Premium = 0 };

                try

                {

                    var totalUsers = await _userServiceClient.GetTotalUsersCountAsync();

                    var dailyActiveUsers = await _userServiceClient.GetDailyActiveUsersCountAsync();

                    

                    // Get users to calculate other stats

                    var users = await _userServiceClient.GetAllUsersAsync(1, 10000);

                    if (users != null)

                    {

                        var userList = users.ToList();

                        userStats = new

                        {

                            Total = totalUsers,

                            Active = userList.Count(u => u.IsActive),

                            NewThisMonth = userList.Count(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30)),

                            Premium = 0 // UserDto doesn't have IsPremium property

                        };

                    }

                }

                catch (Exception ex)

                {

                    _logger.LogWarning(ex, "Could not fetch user stats from UserService: {ErrorMessage}", ex.Message);

                }



                // Try to get exam stats

                var examStats = new { Total = 0, Published = 0, Draft = 0, Categories = 0 };

                try

                {

                    var exams = await _examServiceClient.GetAllExamsAsync();

                    if (exams != null)

                    {

                        var examList = exams.ToList();

                        examStats = new

                        {

                            Total = examList.Count,

                            Published = examList.Count(e => e.IsActive),

                            Draft = examList.Count(e => !e.IsActive),

                            Categories = examList.SelectMany(e => e.QualificationIds).Distinct().Count()

                        };

                    }

                }

                catch (Exception ex)

                {

                    _logger.LogWarning(ex, "Could not fetch exam stats from ExamService");

                }



                var stats = new

                {

                    Admins = new

                    {

                        Total = totalAdmins,

                        Active = activeAdmins,

                        NewThisMonth = newAdminsThisMonth,

                        SuperAdmin = await _context.Admins.CountAsync(a => a.Role == "SuperAdmin")

                    },

                    Users = userStats,

                    Exams = examStats,

                    Performance = new

                    {

                        AvgCompletionRate = 78.5, // Would come from analytics

                        AvgScore = 82.3, // Would come from analytics

                        TotalAttempts = 5678, // Would come from analytics

                        PassRate = 85.2 // Would come from analytics

                    },

                    Revenue = new

                    {

                        Total = 15420.50, // Would come from PaymentService

                        ThisMonth = 3250.00, // Would come from PaymentService

                        LastMonth = 2890.00, // Would come from PaymentService

                        Growth = 12.4 // Would come from PaymentService

                    }

                };



                return Ok(stats);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Error getting system stats");

                return StatusCode(500, new { Error = "Failed to load statistics" });

            }

        }



        /// <summary>

        /// Get recent activities

        /// </summary>

        [HttpGet("activity")]

        public async Task<ActionResult> GetRecentActivity()

        {

            try

            {

                var activities = await GetRecentActivities();

                return Ok(activities);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Error getting recent activity");

                return StatusCode(500, new { Error = "Failed to load activities" });

            }

        }



        /// <summary>

        /// Get health status

        /// </summary>

        [HttpGet("health")]

        public ActionResult GetHealthStatus()

        {

            try

            {

                var health = new

                {

                    Status = "Healthy",

                    Services = new

                    {

                        AdminService = "Running",

                        Database = "Connected",

                        UserService = "Checking...", // Would check actual service

                        ExamService = "Checking...", // Would check actual service

                        PaymentService = "Checking..." // Would check actual service

                    },

                    Metrics = new

                    {

                        Uptime = "2 days, 14 hours", // Would calculate actual uptime

                        MemoryUsage = "245 MB", // Would get actual memory usage

                        CpuUsage = "12%", // Would get actual CPU usage

                        ResponseTime = "45 ms" // Would measure actual response time

                    },

                    LastCheck = DateTime.UtcNow

                };



                return Ok(health);

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Error getting health status");

                return StatusCode(500, new { Error = "Failed to get health status" });

            }

        }



        private async Task<object[]> GetRecentActivities()

        {

            var activities = new List<object>();



            // Get recent admin activities from database

            var recentAdmins = await _context.Admins

                .OrderByDescending(a => a.LastLoginAt)

                .Take(3)

                .ToListAsync();



            foreach (var admin in recentAdmins)

            {

                activities.Add(new

                {

                    Id = admin.Id,

                    Type = "Admin Login",

                    Description = $"Admin ID {admin.UserId} logged in",

                    User = $"Admin {admin.Id}",

                    Time = GetRelativeTime(admin.LastLoginAt ?? DateTime.UtcNow),

                    Icon = "log-in"

                });

            }



            // Add real user activities from UserService (if available)

            try

            {

                var recentUsers = await _userServiceClient.GetAllUsersAsync(1, 5);

                if (recentUsers != null)

                {

                    foreach (var user in recentUsers.Take(3))

                    {

                        activities.Add(new

                        {

                            Id = user.Id,

                            Type = "User Activity",

                            Description = $"User {user.Name} active",

                            User = user.Name,

                            Time = GetRelativeTime(user.LastLoginAt ?? user.CreatedAt),

                            Icon = "user"

                        });

                    }

                }

            }

            catch (Exception ex)

            {

                _logger.LogWarning(ex, "Could not fetch user activities from UserService");

            }



            return activities.Take(10).ToArray();

        }



        private string GetRelativeTime(DateTime? dateTime)

        {

            if (!dateTime.HasValue) return "Unknown";

            

            var span = DateTime.UtcNow - dateTime.Value;

            if (span.TotalMinutes < 1) return "Just now";

            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes} mins ago";

            if (span.TotalHours < 24) return $"{(int)span.TotalHours} hours ago";

            return $"{(int)span.TotalDays} days ago";

        }

    }

}


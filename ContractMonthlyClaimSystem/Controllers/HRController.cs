/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: w3schools
 * Date: 2025
 * Title: C# Tutorial
 * Type: [Source Code]
 * Available at: <https://www.w3schools.com/cs/index.php> [Accessed 2 November 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: The IIE
 * Date: 2025
 * Title: HOW TO MAKE AN MVC [CLDV6211]
 * Type: [Source Code]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2025
 * Title: Quickstart: Deploy an ASP.NET web app
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/azure/app-service/quickstart-dotnetcore?tabs=net80&pivots=development-environment-vs> [Accessed 2 November 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2025
 * Title: Get started with ASP.NET Core MVC
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-9.0&tabs=visual-studio> [Accessed 2 November 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: The IIE
 * Date: 2025
 * Title: Session Management – Roles [PROG6212]
 * Type: [Source Code]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: OpenAI
 * Date: 2025
 * Title: Example of how to generate a report
 * Type: [Source Code]
 * Link to chat: <https://chatgpt.com/share/6909ff85-f7d4-8009-9d12-a0cc7d2b6598> [Accessed 2 November 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2025
 * Title: Policy-based authorization in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-10.0> [Accessed 16 November 2025]
 **************************************************************************************************************/

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace ContractMonthlyClaimSystem.Controllers
{
    [Authorize(Policy = "HROnly")]
    public class HRController : Controller
    {
        private readonly CmcsDatabaseContext _context;
        private readonly ILogger<HRController> _logger;

        public HRController(CmcsDatabaseContext context, ILogger<HRController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //_______________________________ In-Text Referencing _____________________________//
        //The following layout was taken from The IIE (2025) Session Management – Roles [PROG6212]

        // Helper methods for authentication
        private bool IsAuthenticated()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        private int GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId") ?? 0;
        }

        private string GetCurrentUserRole()
        {
            return HttpContext.Session.GetString("UserRole") ?? "";
        }

        //_______________________________________ End _____________________________________//

        // GET: HR/ManageLecturers
        public async Task<IActionResult> ManageLecturers()
        {
            if (!IsAuthenticated() || GetCurrentUserRole() != "HR")
            {
                TempData["Error"] = "Access denied. HR access required.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var lecturers = await _context.Users
                    .Where(u => u.UserRole == "Lecturer")
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .ToListAsync();

                return View(lecturers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading lecturers for HR user {UserId}", GetCurrentUserId());
                TempData["Error"] = "An error occurred while loading lecturers.";
                return View(new List<User>());
            }
        }

        // GET: HR/CreateLecturer
        public IActionResult CreateLecturer()
        {
            if (!IsAuthenticated() || GetCurrentUserRole() != "HR")
            {
                TempData["Error"] = "Access denied. HR access required.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: HR/CreateLecturer
        [HttpPost]
        public async Task<IActionResult> CreateLecturer(string firstName, string lastName, string email, string password, decimal hourlyRate)
        {
            if (!IsAuthenticated() || GetCurrentUserRole() != "HR")
            {
                TempData["Error"] = "Access denied. HR access required.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                    string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    TempData["Error"] = "All fields are required.";
                    return View();
                }

                // Validate hourly rate
                if (hourlyRate <= 0 || hourlyRate > 9999.99m)
                {
                    TempData["Error"] = "Hourly rate must be between 0.01 and 9999.99.";
                    return View();
                }

                // Check if email already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (existingUser != null)
                {
                    TempData["Error"] = "A user with this email already exists.";
                    return View();
                }

                // Create new lecturer
                var lecturer = new User
                {
                    FirstName = firstName.Trim(),
                    LastName = lastName.Trim(),
                    Email = email.Trim().ToLower(),
                    UserRole = "Lecturer",
                    Password = password,  
                    HourlyRate = hourlyRate,
                    CreatedDate = DateTime.Now
                };

                _context.Users.Add(lecturer);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Lecturer {LecturerId} created by HR user {HRUserId}", lecturer.UserId, GetCurrentUserId());
                TempData["Success"] = $"Lecturer {firstName} {lastName} created successfully!";
                return RedirectToAction("ManageLecturers");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lecturer by HR user {UserId}", GetCurrentUserId());
                TempData["Error"] = "An error occurred while creating the lecturer. Please try again.";
                return View();
            }
        }

        // GET: HR/EditLecturer
        public async Task<IActionResult> EditLecturer(int id)
        {
            if (!IsAuthenticated() || GetCurrentUserRole() != "HR")
            {
                TempData["Error"] = "Access denied. HR access required.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var lecturer = await _context.Users.FindAsync(id);

                if (lecturer == null || lecturer.UserRole != "Lecturer")
                {
                    TempData["Error"] = "Lecturer not found.";
                    return RedirectToAction("ManageLecturers");
                }

                return View(lecturer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading lecturer {LecturerId} for editing", id);
                TempData["Error"] = "An error occurred while loading lecturer details.";
                return RedirectToAction("ManageLecturers");
            }
        }

        // POST: HR/EditLecturer
        [HttpPost]
        public async Task<IActionResult> EditLecturer(int id, string firstName, string lastName, string email, decimal hourlyRate)
        {
            if (!IsAuthenticated() || GetCurrentUserRole() != "HR")
            {
                TempData["Error"] = "Access denied. HR access required.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var lecturer = await _context.Users.FindAsync(id);

                if (lecturer == null || lecturer.UserRole != "Lecturer")
                {
                    TempData["Error"] = "Lecturer not found.";
                    return RedirectToAction("ManageLecturers");
                }

                // Validate inputs
                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(email))
                {
                    TempData["Error"] = "First name, last name, and email are required.";
                    return View(lecturer);
                }

                // Validate hourly rate
                if (hourlyRate <= 0 || hourlyRate > 9999.99m)
                {
                    TempData["Error"] = "Hourly rate must be between 0.01 and 9999.99.";
                    return View(lecturer);
                }

                // Check if email is being changed and if new email already exists
                if (lecturer.Email != email.Trim().ToLower())
                {
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == email.Trim().ToLower());
                    if (existingUser != null)
                    {
                        TempData["Error"] = "A user with this email already exists.";
                        return View(lecturer);
                    }
                }

                // Update lecturer details
                lecturer.FirstName = firstName.Trim();
                lecturer.LastName = lastName.Trim();
                lecturer.Email = email.Trim().ToLower();
                lecturer.HourlyRate = hourlyRate;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Lecturer {LecturerId} updated by HR user {HRUserId}", id, GetCurrentUserId());
                TempData["Success"] = $"Lecturer {firstName} {lastName} updated successfully!";
                return RedirectToAction("ManageLecturers");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lecturer {LecturerId}", id);
                TempData["Error"] = "An error occurred while updating the lecturer. Please try again.";
                return RedirectToAction("ManageLecturers");
            }
        }

        // GET: HR/ApprovedClaims
        public async Task<IActionResult> ApprovedClaims(string claimMonth = null)
        {
            if (!IsAuthenticated() || GetCurrentUserRole() != "HR")
            {
                TempData["Error"] = "Access denied. HR access required.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var query = _context.Claims
                    .Include(c => c.Lecturer)
                    .Where(c => c.Status == "Approved");

                // Apply filter if claimMonth is provided
                if (!string.IsNullOrEmpty(claimMonth))
                {
                    query = query.Where(c => c.ClaimMonth == claimMonth);
                }

                var approvedClaims = await query
                    .OrderByDescending(c => c.SubmittedDate)
                    .ToListAsync();

                // Pass filter value to view
                ViewBag.ClaimMonthFilter = claimMonth;

                // Get distinct claim months for filter dropdown
                var claimMonths = await _context.Claims
                    .Where(c => c.Status == "Approved")
                    .Select(c => c.ClaimMonth)
                    .Distinct()
                    .OrderByDescending(m => m)
                    .ToListAsync();

                ViewBag.ClaimMonths = claimMonths;

                return View(approvedClaims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading approved claims for HR user {UserId}", GetCurrentUserId());
                TempData["Error"] = "An error occurred while loading approved claims.";
                return View(new List<Claim>());
            }
        }

        //_______________________________ In-Text Referencing _____________________________//
        //The following layout was taken from OpenAI (2025) Example of how to generate a report

        // GET: HR/GenerateReport
        public async Task<IActionResult> GenerateReport(string claimMonth = null)
        {
            if (!IsAuthenticated() || GetCurrentUserRole() != "HR")
            {
                TempData["Error"] = "Access denied. HR access required.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var query = _context.Claims
                    .Include(c => c.Lecturer)
                    .Where(c => c.Status == "Approved");

                // Apply filter if claimMonth is provided
                if (!string.IsNullOrEmpty(claimMonth))
                {
                    query = query.Where(c => c.ClaimMonth == claimMonth);
                }

                var approvedClaims = await query
                    .OrderBy(c => c.Lecturer.LastName)
                    .ThenBy(c => c.Lecturer.FirstName)
                    .ThenBy(c => c.ClaimMonth)
                    .ToListAsync();

                // Calculate grand total
                var grandTotal = approvedClaims.Sum(c => c.TotalAmount);

                ViewBag.GrandTotal = grandTotal;
                ViewBag.ClaimMonthFilter = claimMonth;
                ViewBag.ReportDate = DateTime.Now;

                return View(approvedClaims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report for HR user {UserId}", GetCurrentUserId());
                TempData["Error"] = "An error occurred while generating the report.";
                return RedirectToAction("ApprovedClaims");
            }
        }

        //_______________________________________ End _____________________________________//
    }
}
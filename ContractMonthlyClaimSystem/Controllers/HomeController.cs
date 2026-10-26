/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: w3schools
 * Date: 2025
 * Title: C# Tutorial
 * Type: [Source Code]
 * Available at: <https://www.w3schools.com/cs/index.php> [Accessed 13 October 2025]
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
 * Available at: <https://learn.microsoft.com/en-us/azure/app-service/quickstart-dotnetcore?tabs=net80&pivots=development-environment-vs> [Accessed 13 October 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2025
 * Title: Get started with ASP.NET Core MVC
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-9.0&tabs=visual-studio> [Accessed 13 October 2025]
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
 * Title: HTTP sessions in MVC
 * Type: [Source Code]
 * Link to chat: <https://chatgpt.com/share/68f7341c-16a8-8009-8685-e53bcd6a1ddd> [Accessed 13 October 2025]
 **************************************************************************************************************/

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly CmcsDatabaseContext _context;

        public HomeController(CmcsDatabaseContext context)
        {
            _context = context;
        }

        //_______________________________ In-Text Referencing _____________________________//
        //The following layout was taken from OpenAI (2025) HTTP sessions in MVC

        // GET: Home/Index 
        public async Task<IActionResult> Index()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Get user info from session
            ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            // Get dashboard data based on role
            if (ViewBag.UserRole == "Lecturer")
            {
                var lecturerClaims = await _context.Claims
                    .Where(c => c.LecturerId == userId.Value)
                    .ToListAsync();

                ViewBag.PendingClaims = lecturerClaims.Count(c => c.Status == "Pending");
                ViewBag.ApprovedClaims = lecturerClaims.Count(c => c.Status == "Approved");
                ViewBag.RejectedClaims = lecturerClaims.Count(c => c.Status == "Rejected");
                ViewBag.TotalEarnings = lecturerClaims
                    .Where(c => c.Status == "Approved")
                    .Sum(c => c.TotalAmount ?? 0);
            }
            else if (ViewBag.UserRole == "Coordinator")
            {
                ViewBag.PendingClaims = await _context.Claims
                    .CountAsync(c => c.Status == "Pending");
            }
            else if (ViewBag.UserRole == "Manager")
            {
                ViewBag.PendingClaims = await _context.Claims
                    .CountAsync(c => c.Status == "Verified");
            }

            return View();
        }

        //_______________________________________ End _____________________________________//

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

    }
}
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
 * Author: Microsoft
 * Date: 2025
 * Title: Use cookie authentication without ASP.NET Core Identity
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-10.0> [Accessed 17 November 2025]
 **************************************************************************************************************/

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly CmcsDatabaseContext _context;

        public AccountController(CmcsDatabaseContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            // If already logged in, redirect to home
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //_______________________________ In-Text Referencing _____________________________//
        //The following layout was taken from The IIE (2025) Session Management – Roles [PROG6212]
        // and Microsoft (2025) Use cookie authentication without ASP.NET Core Identity

        // POST: Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string userRole)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    TempData["Error"] = "Email and password are required.";
                    return View();
                }

                // Find user in database
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == email
                                           && u.Password == password
                                           && u.UserRole == userRole);

                if (user != null)
                {
                    // Store user info in session
                    HttpContext.Session.SetInt32("UserId", user.UserId);
                    HttpContext.Session.SetString("UserName", $"{user.FirstName} {user.LastName}");
                    HttpContext.Session.SetString("UserRole", user.UserRole);
                    HttpContext.Session.SetString("UserEmail", user.Email);

                    // Create claims for cookie authentication
                    var claims = new List<System.Security.Claims.Claim>
                      {
                          new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.UserId.ToString()),
                          new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                          new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
                          new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.UserRole)
                      };

                    var claimsIdentity = new System.Security.Claims.ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true, // Remember login across browser sessions
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new System.Security.Claims.ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    TempData["Success"] = $"Welcome back, {user.FirstName}!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Error"] = "Invalid email, password, or role selection.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Login error: " + ex.Message;
                return View();
            }
        }

        //_______________________________________ End _____________________________________//

        // GET: Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            TempData["Message"] = "You have been logged out successfully.";
            return RedirectToAction("Login");
        }

        // GET: Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            ViewBag.UserRole = userRole ?? "Unknown";
            ViewBag.Message = $"Access Denied. You are logged in as {userRole ?? "Unknown"}, but this page requires different permissions.";
            return View();
        }
    }
}
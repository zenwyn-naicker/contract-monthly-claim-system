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
 * Author: Microsoft
 * Date: 2025
 * Title: Get started with Razor Pages in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/tutorials/razor-pages/razor-pages-start?view=aspnetcore-9.0&tabs=visual-studio> [Accessed 13 October 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Hefaz, H
 * Date: 2025
 * Title: Customizing Navbar In MVC 5 With Bootstrap
 * Type: [Source Code]
 * Available at: <https://www.csharp.com/article/customizing-navbar-in-mvc-5-with-bootstrap/> [Accessed 13 October 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Dentistkiller Yusuf
 * Date: 2025
 * Title: HTML, CSS, and C# in Web Development
 * Type: [Source Code]
 * Available at: <https://github.com/Dentistkiller/CSS> [Accessed 13 October 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Nikunj Satasiya
 * Date: 2025
 * Title: Encrypting and Decrypting Files using .NET Core
 * Type: [Source Code]
 * Available at: <https://www.c-sharpcorner.com/article/encrypting-and-decrypting-files-using-net-core/> [Accessed 17 October 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Sardar Mudassar Ali Khan
 * Date: 2025
 * Title: Complete File Upload and Download in ASP.NET Core MVC
 * Type: [Source Code]
 * Available at: <https://www.c-sharpcorner.com/article/complete-file-upload-and-download-in-asp-net-core-mvc/> [Accessed 17 October 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: The IIE
 * Date: 2025
 * Title: Session Management – Roles [PROG6212]
 * Type: [Source Code]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: The IIE
 * Date: 2025
 * Title: How to do Unit Testing in Visual Studio MVC with a DB [PROG6212]
 * Type: [Source Code]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: OpenAI
 * Date: 2025
 * Title: Mock ISession implementation
 * Type: [Source Code]
 * Link to chat: <https://chatgpt.com/share/68f72e47-6e1c-8009-9171-a86b97686226> [Accessed 13 October 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: OpenAI
 * Date: 2025
 * Title: HTTP sessions in MVC
 * Type: [Source Code]
 * Link to chat: <https://chatgpt.com/share/68f7341c-16a8-8009-8685-e53bcd6a1ddd> [Accessed 13 October 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: OpenAI
 * Date: 2025
 * Title: Example of how to generate a report
 * Type: [Source Code]
 * Link to chat: <https://chatgpt.com/share/6909ff85-f7d4-8009-9d12-a0cc7d2b6598> [Accessed 2 November 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: OpenAI
 * Date: 2025
 * Title: Example of automatic checks on high values
 * Type: [Source Code]
 * Link to chat: <https://chatgpt.com/share/690a05f2-e544-8009-a6b8-46fa612b78d6> [Accessed 2 November 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2025
 * Title: Policy-based authorization in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-10.0> [Accessed 16 November 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2025
 * Title: Use cookie authentication without ASP.NET Core Identity
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-10.0> [Accessed 17 November 2025]
 **************************************************************************************************************/

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Models;

namespace ContractMonthlyClaimSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            //_______________________________ In-Text Referencing _____________________________//
            //The following layout was taken from The IIE (2025) HOW TO MAKE AN MVC [CLDV6211]

            // Add DbContext
            builder.Services.AddDbContext<CmcsDatabaseContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            //_______________________________________ End _____________________________________//

            //_______________________________ In-Text Referencing _____________________________//
            //The following layout was taken from Microsoft (2025) Policy-based authorization in ASP.NET Core
            // and Microsoft (2025) Use cookie authentication without ASP.NET Core Identity

            // Add HttpContextAccessor for session access in authorization handler
            builder.Services.AddHttpContextAccessor();

            // Add Cookie Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add Authorization services with role-based policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("HROnly", policy =>
                    policy.Requirements.Add(new ContractMonthlyClaimSystem.Authorization.RoleRequirement("HR")));

                options.AddPolicy("LecturerOnly", policy =>
                    policy.Requirements.Add(new ContractMonthlyClaimSystem.Authorization.RoleRequirement("Lecturer")));

                options.AddPolicy("CoordinatorOnly", policy =>
                    policy.Requirements.Add(new ContractMonthlyClaimSystem.Authorization.RoleRequirement("Coordinator")));

                options.AddPolicy("ManagerOnly", policy =>
                    policy.Requirements.Add(new ContractMonthlyClaimSystem.Authorization.RoleRequirement("Manager")));

                // Fallback policy: require authentication for all requests
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAssertion(_ => true) // Allow by default, let controllers decide
                    .Build();
            });

            // Register the authorization handler
            builder.Services.AddSingleton<IAuthorizationHandler, ContractMonthlyClaimSystem.Authorization.SessionAuthorizationHandler>();

            //_______________________________________ End _____________________________________//

            //_______________________________ In-Text Referencing _____________________________//
            //The following layout was taken from The IIE (2025) Session Management – Roles [PROG6212]

            // Add Session support for authentication
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //_______________________________________ End _____________________________________//

            // Add services to the container.
            builder.Services.AddControllersWithViews();

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

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            // Handle authorization failures
            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 403)
                {
                    context.Response.Redirect("/Account/AccessDenied");
                }
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

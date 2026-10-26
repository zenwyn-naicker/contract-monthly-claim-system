/*Author: Zenwyn Naicker*/
/*Code Attribution: */
/**************************************************************************************************************
 * Author: w3schools
 * Date: 2025
 * Title: C# Tutorial
 * Type: [Source Code]
 * Available at: <https://www.w3schools.com/cs/index.php> [Accessed 10 September 2025]
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
 * Available at: <https://learn.microsoft.com/en-us/azure/app-service/quickstart-dotnetcore?tabs=net80&pivots=development-environment-vs> [Accessed 10 September 2025]
 **************************************************************************************************************/
/**************************************************************************************************************
 * Author: Microsoft
 * Date: 2025
 * Title: Get started with ASP.NET Core MVC
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-9.0&tabs=visual-studio> [Accessed 10 September 2025]
 **************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContractMonthlyClaimSystem.Models
{
    public partial class User
    {
        public int UserId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        [Display(Name = "Role")]
        public string UserRole { get; set; } = null!;

        public string Password { get; set; } = null!;

        [Range(0.01, 9999.99, ErrorMessage = "Hourly rate must be between 0.01 and 9999.99")]
        public decimal? HourlyRate { get; set; }

        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }

        // Navigation properties
        public virtual ICollection<Claim> ClaimLecturers { get; set; } = new List<Claim>();
        public virtual ICollection<Claim> ClaimReviewedByNavigations { get; set; } = new List<Claim>();
    }
}
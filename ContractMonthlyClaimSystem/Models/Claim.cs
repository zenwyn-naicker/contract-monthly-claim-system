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
    public partial class Claim
    {
        public int ClaimId { get; set; }

        public int LecturerId { get; set; }

        [Display(Name = "Claim Month")]
        public string ClaimMonth { get; set; } = null!;

        [Display(Name = "Hours Worked")]
        public decimal HoursWorked { get; set; }

        [Display(Name = "Hourly Rate")]
        public decimal HourlyRate { get; set; }

        [Display(Name = "Total Amount")]
        public decimal? TotalAmount { get; set; }

        public string? Notes { get; set; }

        public string? Status { get; set; }

        [Display(Name = "Submitted Date")]
        public DateTime? SubmittedDate { get; set; }

        [Display(Name = "Reviewed By")]
        public int? ReviewedBy { get; set; }

        [Display(Name = "Reviewed Date")]
        public DateTime? ReviewedDate { get; set; }

        // Navigation properties
        public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
        public virtual User Lecturer { get; set; } = null!;
        public virtual User? ReviewedByNavigation { get; set; }
    }
}
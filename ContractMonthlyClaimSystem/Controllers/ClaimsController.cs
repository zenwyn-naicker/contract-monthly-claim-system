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
 * Author: Microsoft
 * Date: 2025
 * Title: Policy-based authorization in ASP.NET Core
 * Type: [Source Code]
 * Available at: <https://learn.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-10.0> [Accessed 16 November 2025]
 **************************************************************************************************************/

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystem.Models;
using ContractMonthlyClaimSystem.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly CmcsDatabaseContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ClaimsController> _logger;

        // Allowed file extensions for security
        private static readonly string[] PermittedExtensions = { ".pdf", ".docx", ".xlsx", ".doc", ".xls", ".jpg", ".png" };

        // Maximum file size: 5MB
        private const long MaxFileSize = 5 * 1024 * 1024;

        public ClaimsController(
            CmcsDatabaseContext context,
            IWebHostEnvironment webHostEnvironment,
            ILogger<ClaimsController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

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

        //_______________________________ In-Text Referencing _____________________________//
        //The following layout was taken from The IIE (2025) Session Management – Roles [PROG6212]

        // GET: Claims/Submit
        [Authorize(Policy = "LecturerOnly")]
        public async Task<IActionResult> Submit()
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if user is a Lecturer
            if (GetCurrentUserRole() != "Lecturer")
            {
                TempData["Error"] = "Access denied. Only lecturers can submit claims.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var userId = GetCurrentUserId();
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.LecturerName = $"{user.FirstName} {user.LastName}";

                ViewBag.HourlyRate = user.HourlyRate ?? 0;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading submit claim page for user {UserId}", GetCurrentUserId());
                TempData["Error"] = "An error occurred while loading the page. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        //_______________________________________ End _____________________________________//

        // POST: Claims/Submit
        [Authorize(Policy = "LecturerOnly")]
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 20 * 1024 * 1024)] // 20 MB total
        public async Task<IActionResult> Submit(string claimMonth, decimal hoursWorked, decimal hourlyRate, string notes, List<IFormFile> documents)
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if user is a Lecturer
            if (GetCurrentUserRole() != "Lecturer")
            {
                TempData["Error"] = "Access denied. Only lecturers can submit claims.";
                _logger.LogWarning("Unauthorized claim submission attempt by user {UserId} with role {Role}",
                    GetCurrentUserId(), GetCurrentUserRole());
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Get lecturer's current hourly rate from profile 
                var userId = GetCurrentUserId();
                var lecturer = await _context.Users.FindAsync(userId);

                if (lecturer == null || !lecturer.HourlyRate.HasValue)
                {
                    TempData["Error"] = "Your hourly rate has not been set. Please contact HR.";
                    return View();
                }

                decimal lecturerHourlyRate = lecturer.HourlyRate.Value;

                // Validate inputs
                if (string.IsNullOrEmpty(claimMonth))
                {
                    TempData["Error"] = "Claim month is required.";
                    return View();
                }

                if (hoursWorked < 0.5m || hoursWorked > 744m)
                {
                    TempData["Error"] = "Hours worked must be between 0.5 and 744 hours per month.";
                    return View();
                }

                // Validate hourly rate matches profile 
                if (Math.Abs(hourlyRate - lecturerHourlyRate) > 0.01m)
                {
                    _logger.LogWarning("Hourly rate mismatch for user {UserId}. Posted: {Posted}, Profile: {Profile}",
                        userId, hourlyRate, lecturerHourlyRate);
                    TempData["Error"] = "Hourly rate does not match your profile. Please refresh and try again.";
                    return View();
                }

                // Validate documents before processing claim
                if (documents != null && documents.Any())
                {
                    var validationError = ValidateDocuments(documents);
                    if (!string.IsNullOrEmpty(validationError))
                    {
                        TempData["Error"] = validationError;
                        return View();
                    }
                }

                // Create new claim with lecturer's profile hourly rate
                var claim = new Claim
                {
                    LecturerId = userId,
                    ClaimMonth = claimMonth,
                    HoursWorked = hoursWorked,
                    HourlyRate = lecturerHourlyRate, 
                    TotalAmount = hoursWorked * lecturerHourlyRate,
                    Notes = notes,
                    Status = "Pending",
                    SubmittedDate = DateTime.Now
                };

                // Add claim to database
                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                // Handle document uploads with encryption
                if (documents != null && documents.Count > 0)
                {
                    await SaveDocumentsSecurelyAsync(claim.ClaimId, documents);
                }

                _logger.LogInformation("Claim {ClaimId} submitted successfully by lecturer {UserId}", claim.ClaimId, userId);
                TempData["Success"] = "Claim submitted successfully! Your documents have been securely stored.";
                return RedirectToAction("Track");
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Validation error submitting claim for user {UserId}", GetCurrentUserId());
                TempData["Error"] = ex.Message;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting claim for user {UserId}", GetCurrentUserId());
                TempData["Error"] = "An unexpected error occurred while submitting your claim. Please try again or contact support.";
                return View();
            }
        }

        // Validates uploaded documents for security
        private string? ValidateDocuments(List<IFormFile> documents)
        {
            foreach (var file in documents)
            {
                if (file.Length == 0)
                {
                    continue; // Skip empty files
                }

                // Validate file extension
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(fileExtension) || !PermittedExtensions.Contains(fileExtension))
                {
                    return $"File type '{fileExtension}' is not allowed. Permitted types: {string.Join(", ", PermittedExtensions)}";
                }

                // Validate file size
                if (file.Length > MaxFileSize)
                {
                    return $"File '{file.FileName}' exceeds the maximum size of 5MB. Please compress or split the file.";
                }

                // Validate filename
                if (file.FileName.Length > 255)
                {
                    return "Filename is too long. Please use a shorter filename.";
                }
            }

            return null; // All validations passed
        }

        // Saves documents with encryption for secure storage
        private async Task SaveDocumentsSecurelyAsync(int claimId, List<IFormFile> documents)
        {
            // Create secure uploads folder outside wwwroot
            string secureFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "SecureUploads");
            if (!Directory.Exists(secureFolder))
            {
                Directory.CreateDirectory(secureFolder);
                _logger.LogInformation("Created SecureUploads directory at {Path}", secureFolder);
            }

            foreach (var file in documents)
            {
                if (file.Length == 0)
                {
                    continue; // Skip empty files
                }

                try
                {
                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

                    // Generate secure random filename
                    var trustedFileName = Path.GetRandomFileName() + fileExtension;
                    var tempPath = Path.Combine(secureFolder, "temp_" + trustedFileName);
                    var encryptedPath = Path.Combine(secureFolder, trustedFileName);

                    // Save original file temporarily
                    using (var stream = new FileStream(tempPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Encrypt the file
                    EncryptionHelper.EncryptFile(tempPath, encryptedPath);

                    // Delete temporary unencrypted file
                    if (System.IO.File.Exists(tempPath))
                    {
                        System.IO.File.Delete(tempPath);
                    }

                    // Save document record to database
                    var document = new Document
                    {
                        ClaimId = claimId,
                        FileName = file.FileName, // Store original filename for display
                        FilePath = trustedFileName, // Store encrypted filename
                        UploadedDate = DateTime.Now
                    };

                    _context.Documents.Add(document);

                    _logger.LogInformation("Document {FileName} encrypted and saved for claim {ClaimId}", file.FileName, claimId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error encrypting file {FileName} for claim {ClaimId}", file.FileName, claimId);
                    throw new InvalidOperationException($"Failed to securely save document '{file.FileName}'. Please try again.", ex);
                }
            }

            await _context.SaveChangesAsync();
        }

        // GET: Claims/Track 
        [Authorize(Policy = "LecturerOnly")]
        public async Task<IActionResult> Track()
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var userId = GetCurrentUserId();
                var claims = await _context.Claims
                    .Where(c => c.LecturerId == userId)
                    .OrderByDescending(c => c.SubmittedDate)
                    .ToListAsync();

                return View(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading claims for user {UserId}", GetCurrentUserId());
                TempData["Error"] = "An error occurred while loading your claims. Please try again.";
                return View(new List<Claim>());
            }
        }

        // GET: Claims/Details 
        public async Task<IActionResult> Details(int id)
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var claim = await _context.Claims
                    .Include(c => c.Lecturer)
                    .Include(c => c.ReviewedByNavigation)
                    .FirstOrDefaultAsync(c => c.ClaimId == id);

                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction("Track");
                }

                // Authorization check: Users can only view their own claims or if they're Coordinator/Manager
                var currentRole = GetCurrentUserRole();
                if (claim.LecturerId != GetCurrentUserId() && currentRole != "Coordinator" && currentRole != "Manager")
                {
                    TempData["Error"] = "You are not authorized to view this claim.";
                    return RedirectToAction("Track");
                }

                // Get documents for this claim
                var documents = await _context.Documents
                    .Where(d => d.ClaimId == id)
                    .ToListAsync();

                ViewBag.Documents = documents;
                ViewBag.UserRole = currentRole;

                return View(claim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading claim details for claim {ClaimId}", id);
                TempData["Error"] = "An error occurred while loading claim details. Please try again.";
                return RedirectToAction("Track");
            }
        }

        // GET: Download encrypted document
        public async Task<IActionResult> DownloadDocument(int id)
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var document = await _context.Documents
                    .Include(d => d.Claim)
                    .FirstOrDefaultAsync(d => d.DocumentId == id);

                if (document == null)
                {
                    TempData["Error"] = "Document not found.";
                    return RedirectToAction("Track");
                }

                // Authorization check
                var currentRole = GetCurrentUserRole();
                if (document.Claim.LecturerId != GetCurrentUserId() && currentRole != "Coordinator" && currentRole != "Manager")
                {
                    _logger.LogWarning("Unauthorized download attempt for document {DocumentId} by user {UserId}", id, GetCurrentUserId());
                    TempData["Error"] = "You are not authorized to download this document.";
                    return RedirectToAction("Track");
                }

                string secureFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "SecureUploads");
                string encryptedPath = Path.Combine(secureFolder, document.FilePath);

                if (!System.IO.File.Exists(encryptedPath))
                {
                    _logger.LogError("Encrypted file not found at path: {Path}", encryptedPath);
                    TempData["Error"] = "File not found on server. Please contact support.";
                    return RedirectToAction("Details", new { id = document.ClaimId });
                }

                // Decrypt to temporary location
                string tempDecryptedPath = Path.Combine(secureFolder, "decrypted_" + document.FilePath);

                try
                {
                    EncryptionHelper.DecryptFile(encryptedPath, tempDecryptedPath);

                    // Read decrypted file into memory
                    var fileBytes = await System.IO.File.ReadAllBytesAsync(tempDecryptedPath);

                    // Delete temporary decrypted file
                    if (System.IO.File.Exists(tempDecryptedPath))
                    {
                        System.IO.File.Delete(tempDecryptedPath);
                    }

                    _logger.LogInformation("Document {DocumentId} downloaded by user {UserId}", id, GetCurrentUserId());

                    // Return file with original filename
                    return File(fileBytes, GetContentType(document.FileName), document.FileName);
                }
                catch (Exception ex)
                {
                    // Clean up temporary file if it exists
                    if (System.IO.File.Exists(tempDecryptedPath))
                    {
                        System.IO.File.Delete(tempDecryptedPath);
                    }
                    throw new InvalidOperationException("Failed to decrypt document.", ex);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading document {DocumentId}", id);
                TempData["Error"] = "An error occurred while downloading the document. Please try again or contact support.";
                return RedirectToAction("Track");
            }
        }

        // GET: Claims/PendingVerification 
        [Authorize(Policy = "CoordinatorOnly")]
        public async Task<IActionResult> PendingVerification()
        {
            if (!IsAuthenticated() || GetCurrentUserRole() != "Coordinator")
            {
                TempData["Error"] = "Access denied. Coordinator access required.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var pendingClaims = await _context.Claims
                    .Include(c => c.Lecturer)
                    .Where(c => c.Status == "Pending")
                    .OrderBy(c => c.SubmittedDate)
                    .ToListAsync();

                ViewBag.UserRole = "Coordinator";
                return View(pendingClaims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading pending claims");
                TempData["Error"] = "An error occurred while loading pending claims.";
                return View(new List<Claim>());
            }
        }

        // GET: Claims/PendingApproval 
        [Authorize(Policy = "ManagerOnly")]
        public async Task<IActionResult> PendingApproval()
        {
            if (!IsAuthenticated() || GetCurrentUserRole() != "Manager")
            {
                TempData["Error"] = "Access denied. Manager access required.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var verifiedClaims = await _context.Claims
                    .Include(c => c.Lecturer)
                    .Where(c => c.Status == "Verified")
                    .OrderBy(c => c.SubmittedDate)
                    .ToListAsync();

                ViewBag.UserRole = "Manager";
                return View(verifiedClaims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading verified claims for approval");
                TempData["Error"] = "An error occurred while loading claims for approval.";
                return View(new List<Claim>());
            }
        }

        // POST: Claims/Approve 
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var claim = await _context.Claims.FindAsync(id);

                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction("PendingVerification");
                }

                var userRole = GetCurrentUserRole();

                if (userRole == "Coordinator")
                {
                    claim.Status = "Verified";
                }
                else if (userRole == "Manager")
                {
                    claim.Status = "Approved";
                }
                else
                {
                    TempData["Error"] = "You are not authorized to approve claims.";
                    return RedirectToAction("Index", "Home");
                }

                claim.ReviewedBy = GetCurrentUserId();
                claim.ReviewedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Claim {ClaimId} approved by {Role} user {UserId}", id, userRole, GetCurrentUserId());
                TempData["Success"] = "Claim approved successfully!";

                return userRole == "Coordinator"
                    ? RedirectToAction("PendingVerification")
                    : RedirectToAction("PendingApproval");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving claim {ClaimId}", id);
                TempData["Error"] = "An error occurred while approving the claim. Please try again.";
                return RedirectToAction("PendingVerification");
            }
        }

        // POST: Claims/Reject
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var claim = await _context.Claims.FindAsync(id);

                if (claim == null)
                {
                    TempData["Error"] = "Claim not found.";
                    return RedirectToAction("Index", "Home");
                }

                var userRole = GetCurrentUserRole();

                // Only Coordinator and Manager can reject
                if (userRole != "Coordinator" && userRole != "Manager")
                {
                    TempData["Error"] = "You are not authorized to reject claims.";
                    return RedirectToAction("Index", "Home");
                }

                // Update claim status to Rejected
                claim.Status = "Rejected";
                claim.ReviewedBy = GetCurrentUserId();
                claim.ReviewedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Claim {ClaimId} rejected by {Role} user {UserId}", id, userRole, GetCurrentUserId());
                TempData["Success"] = "Claim rejected successfully.";

                return userRole == "Coordinator"
                    ? RedirectToAction("PendingVerification")
                    : RedirectToAction("PendingApproval");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting claim {ClaimId}", id);
                TempData["Error"] = "An error occurred while rejecting the claim. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        private string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".doc" => "application/msword",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".xls" => "application/vnd.ms-excel",
                ".jpg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream",
            };
        }
    }
}
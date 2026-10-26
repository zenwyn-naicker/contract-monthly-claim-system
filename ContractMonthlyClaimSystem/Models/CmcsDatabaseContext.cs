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
/**************************************************************************************************************
 * Author: The IIE
 * Date: 2025
 * Title: How to do Unit Testing in Visual Studio MVC with a DB [PROG6212]
 * Type: [Source Code]
 **************************************************************************************************************/

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ContractMonthlyClaimSystem.Models;

public partial class CmcsDatabaseContext : DbContext
{
    public CmcsDatabaseContext()
    {
    }

    public CmcsDatabaseContext(DbContextOptions<CmcsDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Claim> Claims { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<User> Users { get; set; }

    //_______________________________ In-Text Referencing _____________________________//
    //The following layout was taken from The IIE (2025) How to do Unit Testing in Visual Studio MVC with a DB [PROG6212]

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Set ConnectionStrings__DefaultConnection or supply configured DbContextOptions.");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    //_______________________________________ End _____________________________________//

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Claim>(entity =>
        {
            entity.HasKey(e => e.ClaimId).HasName("PK__Claims__EF2E13BB83FD5B6B");

            entity.Property(e => e.ClaimId).HasColumnName("ClaimID");
            entity.Property(e => e.ClaimMonth).HasMaxLength(7);
            entity.Property(e => e.HourlyRate).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.HoursWorked).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.LecturerId).HasColumnName("LecturerID");
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");
            entity.Property(e => e.SubmittedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(14, 4)");

            entity.HasOne(d => d.Lecturer).WithMany(p => p.ClaimLecturers)
                .HasForeignKey(d => d.LecturerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Claims__Lecturer__3F466844");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.ClaimReviewedByNavigations)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("FK__Claims__Reviewed__403A8C7D");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__Document__1ABEEF6FA35AE3B9");

            entity.Property(e => e.DocumentId).HasColumnName("DocumentID");
            entity.Property(e => e.ClaimId).HasColumnName("ClaimID");
            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.UploadedDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Claim).WithMany(p => p.Documents)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Documents__Claim__440B1D61");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC74F9ED03");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105345BD3561A").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.UserRole).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

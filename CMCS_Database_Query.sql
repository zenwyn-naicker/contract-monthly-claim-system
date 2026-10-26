-- ===================================================
-- Contract Monthly Claim System (CMCS) Database
-- Author: Zenwyn Naicker
-- ===================================================

/*Code Attribution: */
/**************************************************************************************************************
* Title: Database Design 2nd Edition
* Author: Watt, A. and N. Eng
* Date: 2014
* Availability: https://collection.bccampus.ca/textbooks/database-design-2nd-edition-bccampus-96/
**************************************************************************************************************/

-- Users Table
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    UserRole NVARCHAR(20) NOT NULL CHECK (UserRole IN ('Lecturer', 'Coordinator', 'Manager', 'HR')),
    Password NVARCHAR(255) NOT NULL,
    HourlyRate DECIMAL(8,2) NULL,
    CreatedDate DATETIME2 DEFAULT GETDATE()
);

-- Claims Table
CREATE TABLE Claims (
    ClaimID INT IDENTITY(1,1) PRIMARY KEY,
    LecturerID INT NOT NULL,
    ClaimMonth NVARCHAR(7) NOT NULL,
    HoursWorked DECIMAL(5,2) NOT NULL,
    HourlyRate DECIMAL(8,2) NOT NULL,
    TotalAmount DECIMAL(10,2) NULL,
    Notes NVARCHAR(500),
    Status NVARCHAR(20) DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Verified', 'Approved', 'Rejected')),
    SubmittedDate DATETIME2 DEFAULT GETDATE(),
    ReviewedBy INT NULL,
    ReviewedDate DATETIME2 NULL,
    FOREIGN KEY (LecturerID) REFERENCES Users(UserID),
    FOREIGN KEY (ReviewedBy) REFERENCES Users(UserID)
);

-- Documents Table
CREATE TABLE Documents (
    DocumentID INT IDENTITY(1,1) PRIMARY KEY,
    ClaimID INT NOT NULL,
    FileName NVARCHAR(255) NOT NULL,
    FilePath NVARCHAR(500) NOT NULL,
    UploadedDate DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (ClaimID) REFERENCES Claims(ClaimID)
);

-- User credentials are intentionally not seeded into this source snapshot.
-- Create local users with the required Lecturer, Coordinator, Manager, or HR role
-- in your own database before signing in. Keep any credential-bearing seed script
-- in an ignored *.local.sql file.

-- Display Users
SELECT * FROM Users;

-- Display Claims
SELECT * FROM Claims;

-- Display Documents
SELECT * FROM Documents;
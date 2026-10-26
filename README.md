# Contract Monthly Claim System

An ASP.NET Core MVC web application that automates monthly payment claims for contract lecturers. Lecturers submit claims with supporting documents, programme coordinators verify them, academic managers approve them, and HR manages lecturer profiles and produces payment reports. Supporting documents are encrypted at rest with AES.

Built for PROG6212 at Emeris Durban North (2025).

**Videos:** [Demo](https://youtu.be/A-wQiHrCJJo) · [Presentation](https://youtu.be/KLdkQXhGNr0) · [Code walkthrough](https://youtu.be/iU_xNgnIpjE)

## Features

- **Claim submission:** lecturers enter hours worked, and the total is calculated from the hourly rate on their profile, which is validated on the server.
- **Validation:** hours must be between 0.5 and 744 per month, and supporting documents must be PDF, Word, Excel or image files under 5 MB.
- **Two-stage approval:** coordinators verify claims and managers approve or reject them, with reviewer names and timestamps recorded.
- **Risk flags:** claims over 160 hours or R50,000 are highlighted for reviewers, with summary counts on the review dashboards.
- **Status tracking:** lecturers follow each claim from submission to approval.
- **HR tools:** create and edit lecturer profiles and hourly rates, view approved claims by month, and generate print-ready payment reports.
- **Role-based access:** session-based sign-in with Lecturer, Coordinator, Manager and HR roles enforced by a custom authorisation handler.
- **Encrypted documents:** uploaded files are encrypted with AES and decrypted only for authorised downloads.

## Tech stack

| Area | Technologies |
| --- | --- |
| Web app | C#, ASP.NET Core MVC (.NET 8), Razor, Bootstrap, JavaScript |
| Data | SQL Server with Entity Framework Core |
| Security | Session-based authentication, policy-based authorisation, AES file encryption |

## Architecture

The app follows MVC. Entity Framework models (`User`, `Claim`, `Document`) hold the data and validation rules, controllers handle each role's workflow (`ClaimsController`, `HRController`, `AccountController`), and Razor views render role-specific dashboards. An authorisation handler checks the role stored in the session, and `EncryptionHelper` encrypts and decrypts documents.

## Getting started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB works for development)

### Run locally

1. Create a database and run `CMCS_Database_Query.sql` against it, then add users for the Lecturer, Coordinator, Manager and HR roles.
2. In `ContractMonthlyClaimSystem`, copy `appsettings.Local.example.json` to `appsettings.Local.json` and set `ConnectionStrings:DefaultConnection`.
3. Set the document encryption key and IV:

```bash
export CMCS_ENCRYPTION_KEY="$(openssl rand -base64 32)"
export CMCS_ENCRYPTION_IV="$(openssl rand -base64 16)"
```

4. Run the app:

```bash
dotnet run --project ContractMonthlyClaimSystem
```

Keep the same key and IV for as long as you need existing uploads, because changing them makes encrypted files unreadable. `appsettings.Local.json` is ignored by Git.

## Project structure

```text
ContractMonthlyClaimSystem/
  Controllers/     Account, claims, HR and home controllers
  Models/          EF Core entities and database context
  Views/           Role-specific Razor views
  Authorization/   Session-based role authorisation handler
  Helpers/         AES document encryption
CMCS_Database_Query.sql   SQL Server schema
```

## Acknowledgements

Frameworks, documentation and adapted code patterns are credited in [CODE_ATTRIBUTION.txt](CODE_ATTRIBUTION.txt).

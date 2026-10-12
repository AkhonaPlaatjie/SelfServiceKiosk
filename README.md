    # Self-Service Kiosk

Starter solution for the Singular Academy Software Developer Learnership project.

It gives every learner the same clean starting point: the solution structure, the build and Git configuration, and **one fully worked example** — login for a `User` and for a `SuperUser`. Everything else in the project brief is yours to build, following the same pattern.

- New here? Read [CONTRIBUTING.md](CONTRIBUTING.md) for how to branch, commit and open pull requests.
- Setting up the GitHub repository? Read [docs/GITHUB_SETUP.md](docs/GITHUB_SETUP.md).
- Want to understand the code? Read [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

## Solution layout

```
self-service-kiosk/
├── .github/
│   ├── workflows/ci.yml              Builds and tests every pull request
│   ├── pull_request_template.md
│   └── CODEOWNERS
├── docs/
│   ├── ARCHITECTURE.md               How a request moves through the layers
│   └── GITHUB_SETUP.md               Branch rules and learner branches
├── scripts/
│   ├── learners.txt                  One learner per line
│   ├── create-learner-branches.*     Creates learner/<name> branches
│   ├── apply-branch-rules.*          Applies the GitHub branch rules
│   └── github/                       The rules themselves, as JSON
├── src/
│   └── SelfServiceKiosk.Api/
│       ├── Constants/                Values shared across the API (claim names)
│       ├── Controllers/              HTTP endpoints. No business logic.
│       ├── Data/                     DbContext, seeder, EF configurations, migrations
│       ├── DTOs/                     Request and response shapes
│       ├── Enums/                    UserRole (User, SuperUser)
│       ├── Exceptions/               Business exceptions mapped to HTTP status codes
│       ├── Extensions/               Dependency-injection registrations
│       ├── Interfaces/               Contracts for services and repositories
│       ├── Mappers/                  Entity to DTO conversion
│       ├── Middleware/               Global exception handling
│       ├── Models/                   Database entities
│       ├── Repositories/             Database queries. No business logic.
│       ├── Services/                 Business rules. Unit tested.
│       ├── Settings/                 Strongly typed configuration
│       └── Program.cs
├── tests/
│   └── SelfServiceKiosk.Api.Tests/   xUnit + Moq
├── .editorconfig
├── .gitattributes
├── .gitignore
├── Directory.Build.props             Settings shared by every project
├── Directory.Packages.props          Every NuGet version, in one place
├── global.json
└── SelfServiceKiosk.sln
```

The React front end is not included. Create it in a `client/` folder at the root when you reach it.

## Prerequisites

| Tool | Version | Check |
| --- | --- | --- |
| .NET SDK | 8.0 or later | `dotnet --version` |
| SQL Server | LocalDB or Express | `sqllocaldb info` |
| EF Core tools | 8.x | `dotnet ef --version` |
| Git | any recent | `git --version` |

Install the EF Core tools once per machine:

```
dotnet tool install --global dotnet-ef --version "8.*"
```

## Getting started

Run every command from the repository root unless it says otherwise.

### 1. Set your local secrets

No connection string, key or password is stored in Git (NFR-06). Each developer keeps them in .NET User Secrets, which live outside the repository.

```
cd src/SelfServiceKiosk.Api

dotnet user-secrets set "ConnectionStrings:KioskDb" "Server=(localdb)\MSSQLLocalDB;Database=SelfServiceKiosk;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:Key" "<paste a random key of at least 32 characters>"
dotnet user-secrets set "Seed:UserPassword" "<choose a password for the test User>"
dotnet user-secrets set "Seed:SuperUserPassword" "<choose a password for the test SuperUser>"

cd ../..
```

To generate a random key:

```
# PowerShell
$bytes = New-Object byte[] 48; [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes); [Convert]::ToBase64String($bytes)

# Git Bash, macOS or Linux
openssl rand -base64 48
```

### 2. Trust the development HTTPS certificate

```
dotnet dev-certs https --trust
```

### 3. Build and test

```
dotnet restore
dotnet build
dotnet test
```

### 4. Run the API

```
dotnet run --project src/SelfServiceKiosk.Api
```

On start-up in Development the API applies any pending migrations and creates the two test accounts. Swagger opens at <https://localhost:7243/swagger>.

> If the API stops with "No EF Core migrations were found", the first migration has not been committed yet. See step 2 of [docs/GITHUB_SETUP.md](docs/GITHUB_SETUP.md).

## Test accounts

The seeder creates one account per role. All details are fake (NFR-08).

| Role | Email | Password |
| --- | --- | --- |
| User | `user@kiosk.test` | the value you set for `Seed:UserPassword` |
| SuperUser | `superuser@kiosk.test` | the value you set for `Seed:SuperUserPassword` |

## The login API

| Method | Route | Who it is for |
| --- | --- | --- |
| POST | `/api/auth/login/user` | Accounts with the `User` role |
| POST | `/api/auth/login/superuser` | Accounts with the `SuperUser` role |

Request body (both endpoints):

```json
{
  "email": "user@kiosk.test",
  "password": "your-test-password"
}
```

Successful response (200):

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAtUtc": "2026-10-05T09:30:00Z",
  "user": {
    "id": 1,
    "firstName": "Test",
    "surname": "User",
    "email": "user@kiosk.test",
    "role": "User"
  }
}
```

Error responses all use the same problem-details shape (NFR-11):

| Status | When |
| --- | --- |
| 400 | The body failed validation (missing or malformed email, missing password) |
| 401 | Unknown email, wrong password, or the account does not hold the role for that endpoint |
| 403 | The password is correct but the account is deactivated (BR-09) |
| 500 | Anything unexpected. The detail is logged; the client sees a friendly message only |

To call a protected endpoint in Swagger, log in, copy the `accessToken`, click **Authorize** and paste it.

## Protecting your own endpoints

Every endpoint already requires a valid token unless it is marked `[AllowAnonymous]`. To restrict one to a role, use the enum's name:

```csharp
[Authorize(Roles = nameof(UserRole.SuperUser))]
[HttpPost]
public async Task<ActionResult<ProductDto>> Create(CreateProductDto request, CancellationToken cancellationToken)
```

To read who is calling:

```csharp
var userId = int.Parse(User.FindFirstValue(KioskClaimTypes.UserId)!, CultureInfo.InvariantCulture);
```

## Tests

```
dotnet test
dotnet test --collect:"XPlat Code Coverage"
```

The coverage file is written under `tests/SelfServiceKiosk.Api.Tests/TestResults/`.

## Database migrations

```
dotnet ef migrations add <Name> --project src/SelfServiceKiosk.Api --output-dir Data/Migrations
dotnet ef database update --project src/SelfServiceKiosk.Api
```

## Design notes

- **Roles are an enum** (`UserRole.User`, `UserRole.SuperUser`), stored as text on the `Users` table. The project brief calls the second role "Administrator"; `SuperUser` is the same role under this starter's name.
- **Passwords are hashed with ASP.NET Core Identity's built-in hasher** (NFR-01). The starter uses its own `User` entity instead of Identity's user and role tables, because the role is an enum.
- **The token carries only an Id and a role**, never personal information.
- **Target framework** is set once, in `Directory.Build.props`. It is `net8.0`, the minimum the brief allows. Microsoft's support for .NET 8 ends in November 2026; moving up means changing that one line and the package versions in `Directory.Packages.props`..

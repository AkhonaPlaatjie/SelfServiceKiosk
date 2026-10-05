# Architecture

The API is one project with a folder per responsibility. A request always moves through the same layers, in the same direction:

```
HTTP request
    │
    ▼
Controller      receives a DTO, calls one service method, returns a DTO
    │
    ▼
Service         applies the business rules; knows nothing about HTTP or SQL
    │
    ▼
Repository      runs the database query; knows nothing about business rules
    │
    ▼
DbContext       Entity Framework Core and SQL Server
```

Each layer depends on the **interface** of the layer below it, never on the class. That is what lets the unit tests replace a repository with a mock and test a service without a database.

## The rules

1. **Controllers hold no business logic.** If a controller method has an `if` that is about the business, it belongs in a service.
2. **Entities never leave the API.** Controllers accept and return DTOs. A mapper converts between the two. This is why the password hash can never appear in a response.
3. **Services do not use `KioskDbContext` directly.** They ask a repository.
4. **Errors are thrown, not returned.** A service throws a specific exception; `ExceptionHandlingMiddleware` turns it into the right HTTP status code. Controllers contain no `try`/`catch`.
5. **Everything that touches the database is `async`** and accepts a `CancellationToken`.
6. **Nothing personal is logged.** Log the user's Id, never the email, name or password.

## Worked example: logging in

`POST /api/auth/login/superuser` with an email and password.

| Step | File | What happens |
| --- | --- | --- |
| 1 | `Controllers/AuthController.cs` | ASP.NET Core validates the `LoginRequestDto` attributes. An invalid body returns 400 before your code runs. The controller calls `IAuthService.LoginAsync(request, UserRole.SuperUser)`. |
| 2 | `Services/AuthService.cs` | Asks `IUserRepository` for the account with that email. |
| 3 | `Repositories/UserRepository.cs` | Normalises the email and queries the `Users` table. |
| 4 | `Services/AuthService.cs` | Verifies the password with Identity's hasher, checks the account holds the required role, and checks it is active. Any failure throws. |
| 5 | `Services/TokenService.cs` | Builds a JWT containing the user's Id and role, signed with the secret key. |
| 6 | `Mappers/UserMapper.cs` | Converts the `User` entity and the token into a `LoginResponseDto`. |
| 7 | `Controllers/AuthController.cs` | Returns `200 OK` with the DTO. |

If step 4 throws, `Middleware/ExceptionHandlingMiddleware.cs` catches it and writes a 401 or 403 response.

The two login endpoints share one service method. The only difference is the `UserRole` value the controller passes in.

## Where the role enum is used

| Place | Use |
| --- | --- |
| `Enums/UserRole.cs` | The definition: `User`, `SuperUser` |
| `Models/User.cs` | The `Role` property on the entity |
| `Data/Configurations/UserConfiguration.cs` | Saved in the database as text |
| `Controllers/AuthController.cs` | Each endpoint states which role it accepts |
| `Services/TokenService.cs` | Written into the token as the `role` claim |
| Your controllers | `[Authorize(Roles = nameof(UserRole.SuperUser))]` |

## Adding a feature

Take "list categories" as an example. Work from the database upwards:

1. **Model** — add `Models/Category.cs`.
2. **Configuration** — add `Data/Configurations/CategoryConfiguration.cs` and a `DbSet<Category>` on `KioskDbContext`.
3. **Migration** — `dotnet ef migrations add AddCategories --project src/SelfServiceKiosk.Api --output-dir Data/Migrations`.
4. **DTOs** — add `DTOs/Categories/CategoryDto.cs`.
5. **Mapper** — add `Mappers/CategoryMapper.cs`.
6. **Repository** — add `Interfaces/ICategoryRepository.cs` and `Repositories/CategoryRepository.cs`.
7. **Service** — add `Interfaces/ICategoryService.cs` and `Services/CategoryService.cs`.
8. **Register** — add both to `AddKioskServices` in `Extensions/ServiceCollectionExtensions.cs`.
9. **Controller** — add `Controllers/CategoriesController.cs`.
10. **Tests** — add `tests/SelfServiceKiosk.Api.Tests/Services/CategoryServiceTests.cs`.

If the service needs a new kind of failure, add an exception under `Exceptions/` and one line to the `switch` in `ExceptionHandlingMiddleware`.

## Configuration and secrets

| Setting | Where it lives | Why |
| --- | --- | --- |
| Log levels, JWT issuer and audience, token lifetime, allowed CORS origins | `appsettings.json` | Not secret |
| Connection string, JWT signing key, seed passwords | .NET User Secrets | Secret, so never in Git (NFR-06) |

The API refuses to start if the JWT key is missing or shorter than 32 characters, and says so in the error.

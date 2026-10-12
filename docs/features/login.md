# Feature: Login

**Status:** implemented in the starter. It logs in
**Requirement IDs referenced in the code:** BR-09, NFR-01, NFR-02, NFR-06, NFR-08, NFR-09, NFR-11, NFR-12, NFR-13, NFR-15. The functional requirement ID for login: check the brief.

## Purpose
Lets an existing account sign in and receive an access token used on later requests. There are two entry points, one per role.

## Endpoints
| Method | Route | Accepts accounts with role |
|---|---|---|
| POST | /api/auth/login/user | User |
| POST | /api/auth/login/superuser | SuperUser |

Both are anonymous (`[AllowAnonymous]`). Every other endpoint requires a token (NFR-02).

**Request:** `email` (required, valid email, max 256) and `password` (required, max 100).

**Responses**
| Status | When |
|---|---|
| 200 | Correct credentials. Returns `accessToken`, `expiresAtUtc` and the user's id, name, email and role |
| 400 | Body failed validation |
| 401 | Unknown email, wrong password, or account doesn't hold the role for that endpoint (all identical on purpose) |
| 403 | Password correct but the account is deactivated (BR-09) |
| 500 | Unexpected error. Generic message to the client, detail in the log |

## Flow
AuthController -> AuthService -> UserRepository -> KioskDbContext -> Users table.
Then TokenService builds the token and UserMapper builds the response.
ExceptionHandlingMiddleware turns thrown exceptions into 401/403/500.

## Business rules (in order, in AuthService)
1. Find the user by email (trimmed, lower-case). None found: 401.
2. Verify the password against the stored hash. Wrong: 401.
3. Check the role matches the endpoint. Wrong: 401.
4. Check the account is active. Not active: 403.
5. Create the token and return it.

Why 401 for all three early failures: so the endpoint doesn't reveal which emails exist. The 403 is only shown after the password is proven.

## Data
Table `Users`: Id (PK), FirstName, Surname, Email (unique), MobileNumber, PasswordHash, Role (text), IsActive, CreatedAtUtc.
Passwords are stored only as hashes (ASP.NET Core Identity hasher, NFR-01).
Logins are not recorded in the database.

## Token
JWT signed with HMAC-SHA256 using `Jwt:Key`. Claims: user id, role, unique id. Lifetime: `Jwt:ExpiryMinutes` (30 in appsettings.json). Contains no personal information. It is signed, not encrypted.

## Configuration (User Secrets, never in Git: NFR-06)
`ConnectionStrings:KioskDb`, `Jwt:Key` (at least 32 characters), `Seed:UserPassword`, `Seed:SuperUserPassword`.
Test accounts `user@kiosk.test` and `superuser@kiosk.test` are created by DbSeeder at startup in Development (NFR-08).

## Tests
AuthServiceTests (mocked repository and token service, no database): success for each role, unknown email, wrong password, role mismatch both ways, deactivated account, deactivated account with wrong password.

## Try it
Run the API, open /swagger, use POST /api/auth/login/user, then Authorize with the returned token.

## Limitations / open questions
- No registration endpoint yet; accounts come from the seeder.
- No logout: tokens stay valid until they expire.
- No lockout or rate limiting seen in the files reviewed.

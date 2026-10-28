using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SelfServiceKiosk.Api.Constants;
using SelfServiceKiosk.Api.Data;
using SelfServiceKiosk.Api.Interfaces;
using SelfServiceKiosk.Api.Models;
using SelfServiceKiosk.Api.Repositories;
using SelfServiceKiosk.Api.Services;
using SelfServiceKiosk.Api.Settings;
using SelfServiceKiosk.Api.Enums;

namespace SelfServiceKiosk.Api.Extensions;

/// <summary>
/// Dependency-injection registrations, grouped by concern so that Program.cs stays short.
/// When you add a new repository or service, register it in <see cref="AddKioskServices"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    public const string FrontendCorsPolicy = "Frontend";

    public static IServiceCollection AddKioskDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<KioskDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("KioskDb");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'KioskDb' is not configured. Set it with .NET User Secrets - see README.md.");
            }

            options.UseSqlServer(connectionString);
        });

        return services;
    }

    public static IServiceCollection AddKioskAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind and validate the "Jwt" section. A missing or short key stops the app at start-up
        // with a clear message instead of failing on the first login.
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        // Tell the JWT bearer handler how to check incoming tokens, using the validated settings.
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtSettings>>((options, jwtSettings) =>
            {
                var jwt = jwtSettings.Value;

                // Keep the claim names exactly as they are in the token ("sub", "role").
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = KioskClaimTypes.UserId,
                    RoleClaimType = KioskClaimTypes.Role,
                };
            });

        services.AddAuthorization(options =>
        {
            // NFR-02 (Non-Functional Requirement 02): every endpoint needs a valid token unless [AllowAnonymous].
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            // Administrators are the accounts with the SuperUser role.
            options.AddPolicy(KioskPolicies.Administrator, policy => policy.RequireRole(nameof(UserRole.SuperUser)));
        });

        return services;
    }

    public static IServiceCollection AddKioskServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SeedSettings>()
            .Bind(configuration.GetSection(SeedSettings.SectionName));

        // NFR-01: ASP.NET Core Identity's built-in password hasher.
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();

        services.AddScoped<DbSeeder>();

        return services;
    }

    public static IServiceCollection AddKioskCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            // Only the React app's address may call the API from a browser.
            options.AddPolicy(FrontendCorsPolicy, policy => policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod());
        });

        return services;
    }

    public static IServiceCollection AddKioskSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Self-Service Kiosk API",
                Version = "v1",
                Description = "Starter API for the Singular Academy Self-Service Kiosk project.",
            });

            // Adds the "Authorize" button so a token from a login endpoint can be tried in Swagger.
            var bearerScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Paste the accessToken returned by a login endpoint.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme,
                },
            };
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, bearerScheme);
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { bearerScheme, Array.Empty<string>() },
            });

            // Show the /// comments from controllers and DTOs in the Swagger page.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}

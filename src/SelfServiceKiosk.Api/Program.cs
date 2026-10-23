using System.Text.Json.Serialization;
using SelfServiceKiosk.Api.Data;
using SelfServiceKiosk.Api.Extensions;
using SelfServiceKiosk.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// 1. Register services (dependency injection). See Extensions/ServiceCollectionExtensions.cs
// ---------------------------------------------------------------------------
builder.Services
    .AddKioskDatabase(builder.Configuration)
    .AddKioskAuthentication(builder.Configuration)
    .AddKioskServices(builder.Configuration)
    .AddKioskCors(builder.Configuration)
    .AddKioskSwagger();

builder.Services.AddHsts(options => options.MaxAge = TimeSpan.FromDays(365));

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        // Send enums as their names ("SuperUser") instead of numbers (2).
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddProblemDetails();

var app = builder.Build();

// ---------------------------------------------------------------------------
// 2. Build the request pipeline. The order of these lines matters.
// ---------------------------------------------------------------------------
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseStatusCodePages();


//app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    // HSTS tells browsers to use HTTPS only. Skipped in Development because browsers remember it per host.
    app.UseHsts();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    // Swagger sits before authentication so the page itself needs no token.
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(ServiceCollectionExtensions.FrontendCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ---------------------------------------------------------------------------
// 3. Development only: apply migrations and create the two test accounts.
// ---------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await seeder.SeedAsync();
}

await app.RunAsync();

/// <summary>Exposed so integration tests can start the API with WebApplicationFactory.</summary>
public partial class Program
{
}

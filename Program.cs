using SecureSessionApi.Data;
using SecureSessionApi.Services.Implementations;
using SecureSessionApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTER CORE HTTP CONTROLLER SERVICES
builder.Services.AddControllers();

// 2. REGISTER HEALTH CHECKS ENGINE (Crucial: This fixes your current crash!)
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("SQL_Server_Database_Check");

// 3. CONFIGURE INTERACTIVE SWAGGER DOCUMENTATION GEN
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Secure Session API", Version = "v1" });
});

// 4. CONNECT DATABASE PIPELINE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 5. REGISTER CUSTOM BUSINESS SERVICES 
builder.Services.AddScoped<IAuthService, AuthService>();

// 6. CONFIGURE STATEFUL COOKIE-BASED SESSION ENGINE
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "SecureAuthSessionApp";
        options.Cookie.HttpOnly = true;        // Defense against XSS script access
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Requires HTTPS pathways in production
        options.Cookie.SameSite = SameSiteMode.Strict;   // Defense against CSRF vectors
        
        // Return standard raw API JSON responses instead of automated MVC webpage redirects
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// 7. SWAGGER MIDDLEWARE PIPELINE INITIALIZATION
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Secure Session API v1");
        
        // Attaches session cookies inside Swagger testing contexts automatically
        options.ConfigObject.AdditionalItems["withCredentials"] = true;
    });
}

// 8. MIDDLEWARE SECURITY PIPELINE EXECUTION SEQUENCE
// (Notice: app.UseHttpsRedirection() is removed to match your http://localhost:5050 output profile)
app.UseAuthentication();
app.UseAuthorization();

// 9. EXPOSE API ROUTES & TELEMETRY SYSTEMS
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
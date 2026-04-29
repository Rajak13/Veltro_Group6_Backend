// ═══════════════════════════════════════════════════════════════════════════
// Veltro API — Program.cs
// Library Justifications:
//
// Npgsql.EntityFrameworkCore.PostgreSQL — EF Core provider for PostgreSQL.
//   Chosen for its first-class .NET support, LINQ-to-SQL translation, and
//   migration tooling that keeps schema changes version-controlled.
//
// Microsoft.AspNetCore.Authentication.JwtBearer — Industry-standard stateless
//   auth via signed JWT tokens. No server-side session storage needed; tokens
//   carry role claims for fine-grained authorization.
//
// AutoMapper — Eliminates repetitive DTO ↔ Model mapping boilerplate, keeping
//   service methods focused on business logic rather than property assignment.
//
// MailKit — The recommended .NET SMTP library (replaces deprecated SmtpClient).
//   Supports TLS, OAuth2, and HTML emails needed for invoice delivery and
//   overdue credit reminders.
//
// Swashbuckle (Swagger) — Auto-generates interactive API documentation from
//   controller XML comments. Essential for team collaboration and grader review.
//
// Serilog — Structured logging with sink support (console + file). Structured
//   logs are machine-readable, making debugging and audit trails far easier
//   than plain string logs.
//
// BCrypt.Net-Next — Adaptive hashing algorithm with built-in salting. The
//   industry standard for password storage; work factor can be increased over
//   time as hardware improves.
// ═══════════════════════════════════════════════════════════════════════════

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Veltro.Data;
using Veltro.Helpers;
using Veltro.Middleware;
using Veltro.Repositories;
using Veltro.Repositories.Interfaces;
using Veltro.Services;
using Veltro.Services.Interfaces;

// ─── Serilog bootstrap ───────────────────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/veltro-.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// ─── Database ────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─── JWT Authentication ───────────────────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey)
        };
    });

builder.Services.AddAuthorization();

// ─── AutoMapper ───────────────────────────────────────────────────────────────
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ─── Repositories ─────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPartRepository, PartRepository>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

// ─── Services ─────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPartService, PartService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<ILoyaltyService, LoyaltyService>();
builder.Services.AddScoped<JwtHelper>();

// ─── Background Services ──────────────────────────────────────────────────────
builder.Services.AddHostedService<OverdueCreditReminderService>();

// ─── Controllers + Swagger ────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Veltro API",
        Version = "v1",
        Description = "Vehicle Parts Selling & Inventory Management System"
    });

    // Enable JWT auth in Swagger UI
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments for Swagger descriptions
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath)) c.IncludeXmlComments(xmlPath);
});

// ─── CORS (allow frontend dev server) ────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.SetIsOriginAllowed(origin =>
        {
            if (string.IsNullOrEmpty(origin)) return false;
            var uri = new Uri(origin);
            // Allow any localhost port 3000-3009 for local dev
            if (uri.Host == "localhost" && uri.Port >= 3000 && uri.Port <= 3009)
                return true;
            // Allow production URL via environment variable
            var prodUrl = builder.Configuration["AllowedOrigin"];
            if (!string.IsNullOrEmpty(prodUrl) && origin.StartsWith(prodUrl))
                return true;
            return false;
        })
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// ─── Seed database ────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await context.Database.MigrateAsync();
    }
    catch (Npgsql.PostgresException ex) when (ex.SqlState == "42P07")
    {
        // Table already exists - likely created manually or migration history is missing
        Log.Warning("Migration failed because tables already exist. Skipping migration.");
        Log.Information("If this is a fresh setup, consider running the fix-migration-history.sql script.");
    }
    await Seeder.SeedAsync(context);
}

// ─── Middleware pipeline ──────────────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Veltro API v1"));

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text;
using AgriLedger.Application.Interfaces;
using AgriLedger.Application.Mappings;
using AgriLedger.Application.Services;
using AgriLedger.Application.Validators;
using AgriLedger.Infrastructure.Identity;
using AgriLedger.Infrastructure.Persistence;
using AgriLedger.Infrastructure.Repositories;
using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// ---------- Serilog ----------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/agriledger-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// ---------- EF Core / SQL Server ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------- Repository Pattern / Unit of Work ----------
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ---------- Identity / Security ----------
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// ---------- Application services ----------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFarmService, FarmService>();
builder.Services.AddScoped<ICropService, CropService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ILaborService, LaborService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IFileStorageService, AgriLedger.Infrastructure.Identity.FileStorageService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReportExportService, AgriLedger.Infrastructure.Reporting.ReportExportService>();
builder.Services.AddScoped<ISearchService, SearchService>();

// ---------- AutoMapper ----------
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ---------- FluentValidation ----------
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

// ---------- Controllers ----------
builder.Services.AddControllers();

// ---------- JWT Authentication ----------
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidAudience = jwtSection["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!)),
        ClockSkew = TimeSpan.FromMinutes(2)
    };
});
builder.Services.AddAuthorization();

// ---------- CORS (for the React frontend) ----------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AgriLedgerCors", policy =>
        policy.SetIsOriginAllowed(origin => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ---------- Swagger ----------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AgriLedger API", Version = "v1", Description = "Smart Farm Expense & Farm Management System API" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ---------- Forwarded Headers (Render / Reverse Proxy) ----------
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
});

// ---------- Apply Migrations at startup if requested ----------
if (builder.Configuration.GetValue<bool>("Migrate:Database"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        int retryCount = 0;
        while (true)
        {
            try
            {
                await dbContext.Database.MigrateAsync();
                Console.WriteLine("Database migration applied successfully.");
                break;
            }
            catch (Exception ex) when (retryCount < 12)
            {
                retryCount++;
                Console.WriteLine($"Database migration failed, retrying in 5 seconds... ({retryCount}/12). Error: {ex.Message}");
                await Task.Delay(5000);
            }
        }
    }
}

// Opt-in demo/sample data (spec deliverable: "Sample Data"). Never runs unless
// explicitly enabled — set Seed:Demo=true in appsettings/env for a local demo run.
if (builder.Configuration.GetValue<bool>("Seed:Demo"))
{
    await AgriLedger.Infrastructure.SampleData.DemoDataSeeder.SeedAsync(app.Services);
}

// ---------- Middleware pipeline ----------

// Render / Reverse Proxy support
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
});

// Swagger
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AgriLedger API v1");
    c.RoutePrefix = "swagger";
});

app.UseMiddleware<AgriLedger.API.Middleware.ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AgriLedgerCors");

app.UseAuthentication();

app.UseAuthorization();

// Health endpoint
app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        Application = "AgriLedger API",
        Status = "Running",
        Environment = app.Environment.EnvironmentName,
        Time = DateTime.UtcNow
    });
});

app.MapControllers();

app.Run();

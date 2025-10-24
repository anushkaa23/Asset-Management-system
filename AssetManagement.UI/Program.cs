using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data.Context;
using AssetManagement.Data.Repositories;
using AssetManagement.Data.Interfaces;
using AssetManagement.Business.Services;
using AssetManagement.Business.Interfaces;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Database Context - PostgreSQL for Production, SQL Server for Local
builder.Services.AddDbContext<AssetDbContext>(options =>
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    
    Console.WriteLine($"DATABASE_URL environment variable: {(string.IsNullOrEmpty(databaseUrl) ? "NOT SET" : "SET")}");
    
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        // Production: PostgreSQL on Render.com
        Console.WriteLine("Using PostgreSQL database...");
        
        try
        {
            // Handle both postgres:// and postgresql:// formats
            var uriString = databaseUrl.Replace("postgres://", "postgresql://");
            var uri = new Uri(uriString);
            
            var username = uri.UserInfo.Split(':')[0];
            var password = uri.UserInfo.Split(':')[1];
            var host = uri.Host;
            var port = uri.Port > 0 ? uri.Port : 5432; // Default to 5432 if not specified
            var database = uri.AbsolutePath.TrimStart('/');
            
            var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
            
            Console.WriteLine($"PostgreSQL connection: Host={host}, Port={port}, Database={database}, Username={username}");
            
            options.UseNpgsql(connectionString)
                   .ConfigureWarnings(warnings => 
                       warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing DATABASE_URL: {ex.Message}");
            throw;
        }
    }
    else
    {
        // Local Development: SQL Server
        Console.WriteLine("Using SQL Server database...");
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        options.UseSqlServer(connectionString);
    }
});

// Repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<DapperRepository>();

// Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<AuthService>();

// Session storage for authentication
builder.Services.AddScoped<ProtectedSessionStorage>();

var app = builder.Build();

// Run database migrations automatically
try
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AssetDbContext>();
        
        Console.WriteLine("Running database migrations...");
        dbContext.Database.Migrate();
        Console.WriteLine("Migrations completed successfully.");
        
        // Ensure admin user exists
        var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        
        var adminUsername = config["AdminCredentials:Username"] 
            ?? Environment.GetEnvironmentVariable("AdminCredentials__Username") 
            ?? "admin";
        var adminPassword = config["AdminCredentials:Password"] 
            ?? Environment.GetEnvironmentVariable("AdminCredentials__Password") 
            ?? "Admin@123";
        
        Console.WriteLine($"Ensuring admin user '{adminUsername}' exists...");
        await authService.EnsureAdminExistsAsync(adminUsername, adminPassword);
        Console.WriteLine("Admin user setup completed.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error during startup: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    throw;
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Configure port for deployment
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

Console.WriteLine($"Application starting on port {port}...");

app.Run();
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

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Database context with detailed error logging
builder.Services.AddDbContext<AssetDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableDetailedErrors()
           .EnableSensitiveDataLogging());

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

// Use built-in ProtectedSessionStorage (works for Blazor Server)
builder.Services.AddScoped<ProtectedSessionStorage>();

var app = builder.Build();

// Ensure admin user exists
using (var scope = app.Services.CreateScope())
{
    var authService = scope.ServiceProvider.GetRequiredService<AuthService>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    var adminUsername = config["AdminCredentials:Username"] ?? "admin";
    var adminPassword = config["AdminCredentials:Password"] ?? "Admin@123";

    await authService.EnsureAdminExistsAsync(adminUsername, adminPassword);
}

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

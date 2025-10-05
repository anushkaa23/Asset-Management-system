# Asset Management System - Automated Setup Script
# Run this in PowerShell to create the entire project structure

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Asset Management System Setup" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Create solution directory
$solutionName = "AssetManagementSystem"
New-Item -ItemType Directory -Path $solutionName -Force | Out-Null
Set-Location $solutionName

Write-Host "Creating solution..." -ForegroundColor Yellow

# Create solution
dotnet new sln -n AssetManagementSystem

# Create projects
Write-Host "Creating Blazor Server project..." -ForegroundColor Yellow
dotnet new blazorserver -n AssetManagement.UI --no-https $false

Write-Host "Creating Business Logic Library..." -ForegroundColor Yellow
dotnet new classlib -n AssetManagement.Business

Write-Host "Creating Data Access Library..." -ForegroundColor Yellow
dotnet new classlib -n AssetManagement.Data

# Add projects to solution
Write-Host "Adding projects to solution..." -ForegroundColor Yellow
dotnet sln add AssetManagement.UI/AssetManagement.UI.csproj
dotnet sln add AssetManagement.Business/AssetManagement.Business.csproj
dotnet sln add AssetManagement.Data/AssetManagement.Data.csproj

# Add project references
Write-Host "Adding project references..." -ForegroundColor Yellow
Set-Location AssetManagement.UI
dotnet add reference ../AssetManagement.Business/AssetManagement.Business.csproj
Set-Location ../AssetManagement.Business
dotnet add reference ../AssetManagement.Data/AssetManagement.Data.csproj
Set-Location ..

# Install NuGet packages
Write-Host "Installing NuGet packages..." -ForegroundColor Yellow

Set-Location AssetManagement.Data
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
dotnet add package Dapper --version 2.1.35
Set-Location ..

Set-Location AssetManagement.UI
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
Set-Location ..

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Project structure created successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "1. Copy the code files from the artifacts to their respective locations" -ForegroundColor White
Write-Host "2. Update appsettings.json with your connection string" -ForegroundColor White
Write-Host "3. Run migrations: dotnet ef database update --startup-project AssetManagement.UI" -ForegroundColor White
Write-Host "4. Run the app: cd AssetManagement.UI && dotnet run" -ForegroundColor White
Write-Host ""
Write-Host "Project location: $PWD" -ForegroundColor Yellow

# Create folder structure
Write-Host ""
Write-Host "Creating folder structure..." -ForegroundColor Yellow

# Data Layer folders
New-Item -ItemType Directory -Path "AssetManagement.Data/Context" -Force | Out-Null
New-Item -ItemType Directory -Path "AssetManagement.Data/Entities" -Force | Out-Null
New-Item -ItemType Directory -Path "AssetManagement.Data/Repositories" -Force | Out-Null
New-Item -ItemType Directory -Path "AssetManagement.Data/Interfaces" -Force | Out-Null

# Business Layer folders
New-Item -ItemType Directory -Path "AssetManagement.Business/Services" -Force | Out-Null
New-Item -ItemType Directory -Path "AssetManagement.Business/DTOs" -Force | Out-Null
New-Item -ItemType Directory -Path "AssetManagement.Business/Interfaces" -Force | Out-Null

# UI Layer folders
New-Item -ItemType Directory -Path "AssetManagement.UI/Pages/Employees" -Force | Out-Null
New-Item -ItemType Directory -Path "AssetManagement.UI/Pages/Assets" -Force | Out-Null
New-Item -ItemType Directory -Path "AssetManagement.UI/Pages/Assignments" -Force | Out-Null
New-Item -ItemType Directory -Path "AssetManagement.UI/Pages/Reports" -Force | Out-Null

Write-Host "Folder structure created!" -ForegroundColor Green
Write-Host ""
Write-Host "IMPORTANT: Now you need to add the code files." -ForegroundColor Yellow
Write-Host "Refer to the artifacts for all the code implementations." -ForegroundColor Yellow
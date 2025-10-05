# Asset Management System - Blazor Server Application

## Overview

A comprehensive Asset Management System built with **ASP.NET Blazor Server**, implementing a clean layered architecture with Entity Framework Core for CRUD operations and Dapper for high-performance reporting queries.

**Key Feature:** This application starts with a **completely empty database** (except for the admin user). All data must be manually entered by users through the web interface, providing a real-world experience of building an asset management system from scratch.

---

## Technology Stack

- **Frontend/Backend:** ASP.NET Blazor Server (.NET 8.0)
- **ORM:** Entity Framework Core (for CRUD operations)
- **Micro ORM:** Dapper (for high-performance read queries)
- **Database:** Microsoft SQL Server
- **Authentication:** Session-based with ProtectedSessionStorage
- **Architecture:** Clean Layered Architecture (UI → Business → Data)

---

## Architecture

```
┌─────────────────────────────────────┐
│      UI Layer (Blazor Server)       │
│  - Razor Components                 │
│  - User Input Forms                 │
│  - Data Display                     │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      Business Logic Layer           │
│  - Services                         │
│  - DTOs                             │
│  - Validation                       │
│  - Business Rules                   │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      Data Access Layer              │
│  - EF Core (Write Operations)       │
│  - Dapper (Read Operations)         │
│  - Repositories                     │
│  - DbContext                        │
└──────────────┬──────────────────────┘
               │
        ┌──────▼──────┐
        │  SQL Server │
        └─────────────┘
```

---

## Project Structure

```
AssetManagementSystem/
│
├── AssetManagement.sln
│
├── AssetManagement.UI/                    # Blazor Server (Presentation Layer)
│   ├── Pages/
│   │   ├── Index.razor                    # Dashboard
│   │   ├── Login.razor                    # Authentication
│   │   ├── Employees/
│   │   │   ├── EmployeeList.razor
│   │   │   ├── AddEmployee.razor
│   │   │   └── EditEmployee.razor
│   │   ├── Assets/
│   │   │   ├── AssetList.razor
│   │   │   ├── AddAsset.razor
│   │   │   └── EditAsset.razor
│   │   ├── Assignments/
│   │   │   ├── AssignmentList.razor
│   │   │   └── CreateAssignment.razor
│   │   └── Reports/
│   │       └── Reports.razor
│   ├── Shared/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── wwwroot/
│   │   └── site.js
│   ├── Program.cs
│   └── appsettings.json
│
├── AssetManagement.Business/              # Business Logic Layer
│   ├── Services/
│   │   ├── EmployeeService.cs
│   │   ├── AssetService.cs
│   │   ├── AssignmentService.cs
│   │   ├── DashboardService.cs
│   │   └── AuthService.cs
│   ├── DTOs/
│   │   ├── EmployeeDTO.cs
│   │   ├── AssetDTO.cs
│   │   ├── AssignmentDTO.cs
│   │   └── DashboardDTO.cs
│   └── Interfaces/
│       ├── IEmployeeService.cs
│       ├── IAssetService.cs
│       ├── IAssignmentService.cs
│       └── IDashboardService.cs
│
└── AssetManagement.Data/                  # Data Access Layer
    ├── Context/
    │   └── AssetDbContext.cs
    ├── Entities/
    │   ├── Employee.cs
    │   ├── Asset.cs
    │   ├── Assignment.cs
    │   └── User.cs
    ├── Repositories/
    │   ├── EmployeeRepository.cs
    │   ├── AssetRepository.cs
    │   ├── AssignmentRepository.cs
    │   └── DapperRepository.cs
    └── Interfaces/
        ├── IEmployeeRepository.cs
        ├── IAssetRepository.cs
        └── IAssignmentRepository.cs
```

---

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Microsoft SQL Server (LocalDB, Express, or Full Edition)
- Visual Studio 2022 or VS Code
- Git

### Installation Steps

1. **Clone the Repository**
   ```bash
   git clone <your-repo-url>
   cd AssetManagementSystem
   ```

2. **Update Connection String**
   
   Edit `AssetManagement.UI/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AssetManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

3. **Install Dependencies**
   ```bash
   dotnet restore
   ```

4. **Create Database**
   ```bash
   cd AssetManagement.Data
   dotnet ef migrations add InitialCreate --startup-project ../AssetManagement.UI
   dotnet ef database update --startup-project ../AssetManagement.UI
   ```

5. **Run the Application**
   ```bash
   cd ../AssetManagement.UI
   dotnet run
   ```

6. **Access the Application**
   - Open browser: `https://localhost:5001`
   - Login with default credentials:
     - **Username:** `admin`
     - **Password:** `Admin@123`

---

## Default Admin Credentials

```
Username: admin
Password: Admin@123
```

**Note:** The admin user is automatically created on first run from `appsettings.json`. You can modify these credentials in the configuration file before the first launch.

---

## Features

### 1. **Dashboard**
- Real-time asset statistics
- Total assets count
- Available vs Assigned assets
- Assets under repair
- Retired assets
- Spare assets count
- Active employees count
- Asset distribution by type
- **All statistics calculated from user-entered data**

### 2. **Employee Management**
- Add new employees
- Edit employee details
- Delete employees
- Search and filter employees
- Active/Inactive status management
- Email validation (unique constraint)

**Employee Fields:**
- Employee ID (Auto-generated)
- Full Name
- Department
- Email (unique)
- Phone Number
- Designation
- Status (Active/Inactive)

### 3. **Asset Management**
- Add new assets
- Edit asset details
- Delete assets (if not assigned)
- Search and filter assets
- Status management
- Spare asset tracking
- Warranty monitoring

**Asset Fields:**
- Asset ID (Auto-generated)
- Asset Name
- Asset Type
- Make/Model
- Serial Number
- Purchase Date
- Warranty Expiry Date
- Condition (New, Good, Needs Repair, Damaged)
- Status (Available, Assigned, Under Repair, Retired)
- Is Spare (Yes/No)
- Specifications/Details

### 4. **Asset Assignment**
- Assign available assets to active employees
- Full assignment history tracking
- Return asset functionality
- Assignment date and notes
- Prevents duplicate assignments
- Automatic asset status updates

**Assignment Features:**
- View active assignments
- View complete assignment history
- Return assets with notes
- Track assigned and returned dates

### 5. **Reports & Analytics**
- **Warranty Expiry Report:** Assets expiring in next 30 days
- **Assignment History Report:** Complete audit trail
- **Export to CSV:** Download reports
- **Powered by Dapper** for optimized performance

---

## The default credentials are:
### Username: admin
### Password: Admin@123

## Database Schema

### Tables

1. **Users**
   - UserId (PK)
   - Username (Unique)
   - PasswordHash
   - FullName
   - CreatedDate
   - LastLoginDate

2. **Employees**
   - EmployeeId (PK)
   - FullName
   - Department
   - Email (Unique)
   - PhoneNumber
   - Designation
   - IsActive
   - CreatedDate
   - ModifiedDate

3. **Assets**
   - AssetId (PK)
   - AssetName
   - AssetType
   - MakeModel
   - SerialNumber
   - PurchaseDate
   - WarrantyExpiryDate
   - Condition (Enum)
   - Status (Enum)
   - IsSpare
   - Specifications
   - CreatedDate
   - ModifiedDate

4. **Assignments**
   - AssignmentId (PK)
   - AssetId (FK)
   - EmployeeId (FK)
   - AssignedDate
   - ReturnedDate
   - Notes
   - IsActive

---

## User Workflow

### Initial Setup (Starting with Empty Database)

1. **Login** with admin credentials
2. **Add Employees:**
   - Navigate to "Employees"
   - Click "Add Employee"
   - Fill in employee details
   - Save
   - Repeat for all employees

3. **Add Assets:**
   - Navigate to "Assets"
   - Click "Add Asset"
   - Enter asset information
   - Set status to "Available" for assignable assets
   - Save
   - Repeat for all assets

4. **Create Assignments:**
   - Navigate to "Assignments"
   - Click "Create Assignment"
   - Select an available asset
   - Select an active employee
   - Add notes (optional)
   - Save

5. **View Dashboard:**
   - Return to Dashboard
   - See real-time statistics from your data

6. **Generate Reports:**
   - Navigate to "Reports"
   - View warranty expiry alerts
   - Check assignment history
   - Export to CSV

---

## Configuration

### Database Connection

Edit `appsettings.json` to configure your database:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your Connection String Here"
  },
  "AdminCredentials": {
    "Username": "admin",
    "Password": "Admin@123"
  }
}
```

### Common Connection Strings

**SQL Server LocalDB:**
```
Server=(localdb)\\mssqllocaldb;Database=AssetManagementDB;Trusted_Connection=True;
```

**SQL Server Express:**
```
Server=.\\SQLEXPRESS;Database=AssetManagementDB;Trusted_Connection=True;
```

**SQL Server with Authentication:**
```
Server=your-server;Database=AssetManagementDB;User Id=sa;Password=your-password;
```

---

## Testing Guide

### Manual Testing Checklist

#### Employee Management
- [ ] Add employee with all fields
- [ ] Add employee with duplicate email (should fail)
- [ ] Edit employee details
- [ ] Mark employee as inactive
- [ ] Delete employee without assignments
- [ ] Search employees by name/email/department

#### Asset Management
- [ ] Add asset with all fields
- [ ] Add asset with minimal fields
- [ ] Edit asset details
- [ ] Change asset status
- [ ] Mark asset as spare
- [ ] Delete unassigned asset
- [ ] Try to delete assigned asset (should fail)
- [ ] Filter assets by status

#### Assignment Management
- [ ] Assign available asset to active employee
- [ ] Try to assign unavailable asset (should fail)
- [ ] Try to assign to inactive employee (should fail)
- [ ] Return assigned asset
- [ ] View assignment history
- [ ] Filter active vs all assignments

#### Dashboard & Reports
- [ ] Verify dashboard statistics update after changes
- [ ] Check warranty expiry report
- [ ] View assignment history report
- [ ] Export reports to CSV

---

## API/Service Layer

### Key Services

#### EmployeeService
```csharp
- GetAllEmployeesAsync()
- GetEmployeeByIdAsync(int id)
- CreateEmployeeAsync(EmployeeDTO dto)
- UpdateEmployeeAsync(EmployeeDTO dto)
- DeleteEmployeeAsync(int id)
- GetActiveEmployeesAsync()
```

#### AssetService
```csharp
- GetAllAssetsAsync()
- GetAssetByIdAsync(int id)
- CreateAssetAsync(AssetDTO dto)
- UpdateAssetAsync(AssetDTO dto)
- DeleteAssetAsync(int id)
- GetAvailableAssetsAsync()
```

#### AssignmentService
```csharp
- GetAllAssignmentsAsync()
- CreateAssignmentAsync(AssignmentDTO dto)
- ReturnAssetAsync(int id, DateTime returnDate, string notes)
- GetAssignmentsByEmployeeAsync(int employeeId)
- GetAssignmentsByAssetAsync(int assetId)
```

---

## Security Features

- Session-based authentication
- Password hashing (SHA-256)
- SQL injection protection (parameterized queries)
- Input validation (client & server-side)
- Protected routing (requires login)
- CSRF protection (built-in Blazor)

---

## UI/UX Features

- Bootstrap 5 styling
- Responsive design (desktop-focused)
- Loading indicators
- Confirmation dialogs
- Form validation messages
- Success/Error alerts
- Search and filter functionality
- Sortable tables
- Badge indicators for status

---

## Dependencies

### AssetManagement.UI
```xml

```

### AssetManagement.Data
```xml




```

---

## Troubleshooting

### Database Connection Issues
```bash
# Check SQL Server is running
# Verify connection string
# Test connection in SSMS
```

### Migration Errors
```bash
# Delete Migrations folder
# Delete database
# Recreate migrations:
dotnet ef migrations add InitialCreate --startup-project ../AssetManagement.UI
dotnet ef database update --startup-project ../AssetManagement.UI
```

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet build
```

---

## License

This project is created for the Sciforn Solutions Offcampus Hiring Assessment.

---

##  Developer Notes

- **No Dummy Data:** Database starts completely empty
- **User-Driven:** All data is manually entered
- **Real-World Scenario:** Simulates actual asset management implementation
- **Clean Code:** Well-commented and maintainable
- **Best Practices:** Follows SOLID principles and clean architecture

---

## Support

For issues or questions regarding this assessment:
- Review the assignment document
- Check the troubleshooting section
- Ensure all requirements are met before submission

---

## Submission Checklist

- [x] Full source code
- [x] EF Core migrations
- [x] Clean database (no seed data)
- [x] Admin auto-creation from config
- [x] README documentation
- [x] Layered architecture
- [x] EF Core for CRUD
- [x] Dapper for reporting
- [x] All required features implemented
- [x] Input validation
- [x] Error handling

---

**Built for Sciforn Solutions Assessment**
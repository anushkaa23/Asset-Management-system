using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Data.Context;
using AssetManagement.Data.Entities;
using AssetManagement.Data.Interfaces;

namespace AssetManagement.Data.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AssetDbContext _context;

        public EmployeeRepository(AssetDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            // Show ALL employees (both active and inactive)
            return await _context.Employees
                .OrderBy(e => e.FullName)
                .ToListAsync();
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _context.Employees
                .AsNoTracking()  // Prevent tracking issues
                .Include(e => e.Assignments)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
        }

        public async Task<Employee> AddAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<Employee> UpdateAsync(Employee employee)
        {
            // FIXED: Fetch and update existing entity to avoid tracking conflicts
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employee.EmployeeId);
            
            if (existingEmployee == null)
            {
                throw new InvalidOperationException("Employee not found");
            }
            
            // Update all properties
            existingEmployee.FullName = employee.FullName;
            existingEmployee.Email = employee.Email;
            existingEmployee.PhoneNumber = employee.PhoneNumber;
            existingEmployee.Department = employee.Department;
            existingEmployee.Designation = employee.Designation;
            existingEmployee.IsActive = employee.IsActive;
            existingEmployee.ModifiedDate = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            return existingEmployee;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id);
                
            if (employee == null) return false;

            // FIXED: Check only for ACTIVE assignments
            var hasActiveAssignments = await _context.Assignments
                .AnyAsync(a => a.EmployeeId == id && a.IsActive);

            if (hasActiveAssignments)
            {
                throw new InvalidOperationException("Cannot delete employee with active asset assignments");
            }

            // HARD DELETE - Remove from database
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            
            return true;
        }

        public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
        {
            // This method specifically returns only active employees
            return await _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.FullName)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return await _context.Employees
                    .AnyAsync(e => e.Email == email && e.EmployeeId != excludeId.Value);
            }
            
            return await _context.Employees
                .AnyAsync(e => e.Email == email);
        }
    }
}
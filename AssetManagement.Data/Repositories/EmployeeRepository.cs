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
            return await _context.Employees
                .OrderByDescending(e => e.CreatedDate)
                .ToListAsync();
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _context.Employees
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
            // Detach any existing tracked entity
            var existingEntity = _context.Employees.Local
                .FirstOrDefault(e => e.EmployeeId == employee.EmployeeId);
            
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).State = EntityState.Detached;
            }

            employee.ModifiedDate = DateTime.UtcNow;
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return employee;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return false;

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
        {
            return await _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.FullName)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            var query = _context.Employees.Where(e => e.Email == email);
            if (excludeId.HasValue)
            {
                query = query.Where(e => e.EmployeeId != excludeId.Value);
            }
            return await query.AnyAsync();
        }
    }
}
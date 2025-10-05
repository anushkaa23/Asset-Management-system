using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Data.Entities;

namespace AssetManagement.Data.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetAllAsync();
        Task<Employee> GetByIdAsync(int id);
        Task<Employee> AddAsync(Employee employee);
        Task<Employee> UpdateAsync(Employee employee);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<Employee>> GetActiveEmployeesAsync();
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    }
}
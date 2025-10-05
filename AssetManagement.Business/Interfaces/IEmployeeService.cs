using System.Collections.Generic;
using System.Threading.Tasks;
using AssetManagement.Business.DTOs;

namespace AssetManagement.Business.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync();
        Task<EmployeeDTO> GetEmployeeByIdAsync(int id);
        Task<EmployeeDTO> CreateEmployeeAsync(EmployeeDTO employeeDto);
        Task<EmployeeDTO> UpdateEmployeeAsync(EmployeeDTO employeeDto);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<IEnumerable<EmployeeDTO>> GetActiveEmployeesAsync();
    }
}

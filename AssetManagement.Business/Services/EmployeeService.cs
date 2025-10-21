using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetManagement.Business.DTOs;
using AssetManagement.Business.Interfaces;
using AssetManagement.Data.Entities;
using AssetManagement.Data.Interfaces;

namespace AssetManagement.Business.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<EmployeeDTO>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();
            return employees.Select(MapToDTO);
        }

        public async Task<EmployeeDTO> GetEmployeeByIdAsync(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            return employee != null ? MapToDTO(employee) : null;
        }

        public async Task<EmployeeDTO> CreateEmployeeAsync(EmployeeDTO employeeDto)
        {
            // Check if email already exists
            if (await _employeeRepository.EmailExistsAsync(employeeDto.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // FIXED: Create entity with CreatedDate
            var employee = new Employee
            {
                FullName = employeeDto.FullName,
                Department = employeeDto.Department,
                Email = employeeDto.Email,
                PhoneNumber = employeeDto.PhoneNumber,
                Designation = employeeDto.Designation,
                IsActive = employeeDto.IsActive,
                CreatedDate = DateTime.UtcNow  // FIXED: Set CreatedDate
            };

            var created = await _employeeRepository.AddAsync(employee);
            return MapToDTO(created);
        }

        public async Task<EmployeeDTO> UpdateEmployeeAsync(EmployeeDTO employeeDto)
        {
            // Check if email already exists for another employee
            if (await _employeeRepository.EmailExistsAsync(employeeDto.Email, employeeDto.EmployeeId))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // FIXED: Create entity with EmployeeId for update
            var employee = new Employee
            {
                EmployeeId = employeeDto.EmployeeId,  // Important: Include the ID
                FullName = employeeDto.FullName,
                Department = employeeDto.Department,
                Email = employeeDto.Email,
                PhoneNumber = employeeDto.PhoneNumber,
                Designation = employeeDto.Designation,
                IsActive = employeeDto.IsActive
                // Note: CreatedDate and ModifiedDate are handled by the repository
            };

            var updated = await _employeeRepository.UpdateAsync(employee);
            return MapToDTO(updated);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                return await _employeeRepository.DeleteAsync(id);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeDTO>> GetActiveEmployeesAsync()
        {
            var employees = await _employeeRepository.GetActiveEmployeesAsync();
            return employees.Select(MapToDTO);
        }

        private EmployeeDTO MapToDTO(Employee employee)
        {
            return new EmployeeDTO
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                Department = employee.Department,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                Designation = employee.Designation,
                IsActive = employee.IsActive
            };
        }
    }
}
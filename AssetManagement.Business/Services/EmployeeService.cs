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

            var employee = MapToEntity(employeeDto);
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

            var employee = MapToEntity(employeeDto);
            var updated = await _employeeRepository.UpdateAsync(employee);
            return MapToDTO(updated);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            return await _employeeRepository.DeleteAsync(id);
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

        private Employee MapToEntity(EmployeeDTO dto)
        {
            return new Employee
            {
                EmployeeId = dto.EmployeeId,
                FullName = dto.FullName,
                Department = dto.Department,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Designation = dto.Designation,
                IsActive = dto.IsActive
            };
        }
    }
}
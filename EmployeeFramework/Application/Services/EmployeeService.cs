using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeFramework.Application.DTOs;
using EmployeeFramework.Domain.Entities;
using EmployeeFramework.Domain.Repositories;

namespace EmployeeFramework.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        }

        public async Task<EmployeeDto> GetByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return null;

            return MapToDto(employee);
        }

        public async Task<PagedResultDto<EmployeeDto>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var employees = await _employeeRepository.GetAllAsync(page, pageSize);
            var totalCount = await _employeeRepository.CountAsync();
            
            var employeeDtos = employees.Select(MapToDto);
            
            return new PagedResultDto<EmployeeDto>(employeeDtos, totalCount, page, pageSize);
        }

        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            ValidateCreateDto(createDto);

            var employee = new Employee
            {
                Name = createDto.Name,
                Address = createDto.Address,
                Age = createDto.Age,
                Salary = createDto.Salary
            };

            var createdEmployee = await _employeeRepository.CreateAsync(employee);
            return MapToDto(createdEmployee);
        }

        public async Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeDto updateDto)
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            var existingEmployee = await _employeeRepository.GetByIdAsync(id);
            if (existingEmployee == null)
                return null;

            ValidateUpdateDto(updateDto);

            existingEmployee.Name = updateDto.Name;
            existingEmployee.Address = updateDto.Address;
            existingEmployee.Age = updateDto.Age;
            existingEmployee.Salary = updateDto.Salary;
            existingEmployee.UpdatedAt = DateTime.UtcNow;

            var updatedEmployee = await _employeeRepository.UpdateAsync(existingEmployee);
            return MapToDto(updatedEmployee);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _employeeRepository.DeleteAsync(id);
        }

        private static EmployeeDto MapToDto(Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Address = employee.Address,
                Age = employee.Age,
                Salary = employee.Salary,
                CreatedAt = employee.CreatedAt,
                UpdatedAt = employee.UpdatedAt
            };
        }

        private static void ValidateCreateDto(CreateEmployeeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required", nameof(dto.Name));
            
            if (string.IsNullOrWhiteSpace(dto.Address))
                throw new ArgumentException("Address is required", nameof(dto.Address));
            
            if (dto.Age <= 0 || dto.Age > 120)
                throw new ArgumentException("Age must be between 1 and 120", nameof(dto.Age));
            
            if (dto.Salary < 0)
                throw new ArgumentException("Salary cannot be negative", nameof(dto.Salary));
        }

        private static void ValidateUpdateDto(UpdateEmployeeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Name is required", nameof(dto.Name));
            
            if (string.IsNullOrWhiteSpace(dto.Address))
                throw new ArgumentException("Address is required", nameof(dto.Address));
            
            if (dto.Age <= 0 || dto.Age > 120)
                throw new ArgumentException("Age must be between 1 and 120", nameof(dto.Age));
            
            if (dto.Salary < 0)
                throw new ArgumentException("Salary cannot be negative", nameof(dto.Salary));
        }
    }
}
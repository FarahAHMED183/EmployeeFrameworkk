using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeFramework.Application.DTOs;

namespace EmployeeFramework.Application.Services
{
    public interface IEmployeeService
    {
        Task<EmployeeDto> GetByIdAsync(Guid id);
        Task<PagedResultDto<EmployeeDto>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<EmployeeDto> CreateAsync(CreateEmployeeDto createDto);
        Task<EmployeeDto> UpdateAsync(Guid id, UpdateEmployeeDto updateDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
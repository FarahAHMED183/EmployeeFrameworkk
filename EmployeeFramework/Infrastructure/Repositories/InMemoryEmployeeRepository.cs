using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeFramework.Domain.Entities;
using EmployeeFramework.Domain.Repositories;

namespace EmployeeFramework.Infrastructure.Repositories
{
    public class InMemoryEmployeeRepository : IEmployeeRepository
    {
        private readonly List<Employee> _employees;

        public InMemoryEmployeeRepository()
        {
            _employees = new List<Employee>();
        }

        public Task<Employee> GetByIdAsync(Guid id)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(employee);
        }

        public Task<IEnumerable<Employee>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var skip = (page - 1) * pageSize;
            var employees = _employees
                .OrderBy(e => e.Name)
                .Skip(skip)
                .Take(pageSize);
            
            return Task.FromResult(employees);
        }

        public Task<Employee> CreateAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            _employees.Add(employee);
            return Task.FromResult(employee);
        }

        public Task<Employee> UpdateAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            var existingIndex = _employees.FindIndex(e => e.Id == employee.Id);
            if (existingIndex == -1)
                return Task.FromResult<Employee>(null);

            _employees[existingIndex] = employee;
            return Task.FromResult(employee);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var employee = _employees.FirstOrDefault(e => e.Id == id);
            if (employee == null)
                return Task.FromResult(false);

            _employees.Remove(employee);
            return Task.FromResult(true);
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            var exists = _employees.Any(e => e.Id == id);
            return Task.FromResult(exists);
        }

        public Task<int> CountAsync()
        {
            return Task.FromResult(_employees.Count);
        }
    }
}
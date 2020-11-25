using Entities.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindService.Repositories
{
    public interface IEmployeeRepository
    {
        Task<Employee> CreateAsync(Employee e);

        Task<IEnumerable<Employee>> RetireveAllAsync();   
        
        Task<Employee> RetrieveAsync(string id);  
        
        Task<Employee> UpdateAsync(string id, Employee c);

        Task<bool?> DeleteAsync(string id);
    }
}

using Entities.Shared;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindService.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        //using a thread safe dictionary to cache the employees
        private static ConcurrentDictionary<string, Employee> employeeCache;

        // use an instance of the db
        private Northwind db;

        public EmployeeRepository(Northwind db)
        {
            this.db = db;

            if (employeeCache==null)
            {
                employeeCache = new ConcurrentDictionary<string, Employee>(db.Employees.ToDictionary(c => c.EmployeeID.ToString()));
            }
        }

        public async Task<Employee> CreateAsync(Employee e)
        {
            EntityEntry<Employee> added = await db.Employees.AddAsync(e);

            int affected = db.SaveChanges();
            if (affected == 1)
            {
                return employeeCache.AddOrUpdate(e.EmployeeID.ToString(), e, UpdateCache);
            }
            else 
            {
                return null;
            }
        }

        private Employee UpdateCache(string id, Employee e)
        {
            Employee old;
            if (employeeCache.TryGetValue(id, out old))
            {
                if (employeeCache.TryUpdate(id,e,old))
                {
                    return e;
                }
            }
            return null;
        }

        public async Task<bool?> DeleteAsync(string id)
        {
            Employee e = db.Employees.Find(long.Parse(id));
            db.Employees.Remove(e);
            int affected = await db.SaveChangesAsync();
            if (affected ==1)
            {
                return employeeCache.TryRemove(id, out e);
            }
            return null;
        }

        public Task<IEnumerable<Employee>> RetireveAllAsync()
        {
            //get the result from the cached Employees for performance reasons
            return Task.Run<IEnumerable<Employee>>(() => employeeCache.Values);
        }

        public Task<Employee> RetrieveAsync(string id)
        {
            return Task.Run(() =>
            {
                employeeCache.TryGetValue(id, out Employee e);
                return e;
            });
        }

        public async Task<Employee> UpdateAsync(string id, Employee e)
        {
            string employeeID = e.EmployeeID.ToString();
            db.Employees.Update(e);
            int affected = await db.SaveChangesAsync();
            if (affected==1)
            {
                return UpdateCache(id, e);
            }
            return null;
        }
    }
}

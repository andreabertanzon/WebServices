using Microsoft.EntityFrameworkCore.ChangeTracking;
using Packt.Shared;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindService.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        //use a static thread safe dictionary field to cache the customers
        private static ConcurrentDictionary<string, Customer> customersCache;

        //use an instance data context field.
        private Northwind db;
        public CustomerRepository(Northwind db)
        {
            this.db = db;

            //preload customers from the database as a normal
            //dictionary with CustomerID as the key,
            //Convert then to a thread-safe ConcurrentDictionary
            if (customersCache== null)
            {
                customersCache = new ConcurrentDictionary<string, Customer>(db.Customers.ToDictionary(c => c.CustomerID));
            }

        }

        public async Task<Customer> CreateAsync(Customer c)
        {
            //normalize CustomerID into uppercase
            c.CustomerID = c.CustomerID.ToUpper();

            //add to database using EF core
            EntityEntry<Customer> added = await db.Customers.AddAsync(c);

            //number of interested row?
            int affected = await db.SaveChangesAsync();
            if (affected ==1)
            {
                //means that the customer is new so add it to the cache,
                //else update cache UpdateCache()
                return customersCache.AddOrUpdate(c.CustomerID, c, UpdateCache);
            }
            else
            {
                return null;
            }
        }

        private Customer UpdateCache(string id, Customer c)
        {
            Customer old;
            if(customersCache.TryGetValue(id, out old))
            {
                if(customersCache.TryUpdate(id, c, old))
                {
                    return c;
                }
            }
            return null;
        }

        public async Task<bool?> DeleteAsync(string id)
        {
            //normalization
            id = id.ToUpper();

            //remove from database
            Customer c = db.Customers.Find(id);
            db.Customers.Remove(c);
            int affected = await db.SaveChangesAsync();
            if (affected ==1)
            {
                return customersCache.TryRemove(id, out c);
            }
            return null;
        }

        public Task<IEnumerable<Customer>> RetireveAllAsync()
        {
            //get from cache for performances
            return Task.Run<IEnumerable<Customer>>(() => customersCache.Values);
        }

        public Task<Customer> RetrieveAsync(string id)
        {
            return Task.Run(() =>
            {
                id = id.ToUpper();
                customersCache.TryGetValue(id, out Customer c);
                return c;
            });
            
        }

        public async Task<Customer> UpdateAsync(string id, Customer c)
        {
            // normalize the customer ID
            id = id.ToUpper();
            c.CustomerID = c.CustomerID.ToUpper();

            //update in database
            db.Customers.Update(c);
            int affected = await db.SaveChangesAsync();
            if (affected ==1)
            {
                //update the cache
                return UpdateCache(id, c);
            }
            return null;
        }
    }
}

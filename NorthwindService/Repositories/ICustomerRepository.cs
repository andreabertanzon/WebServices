using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Shared;

namespace NorthwindService.Repositories
{
    public interface ICustomerRepository
    {
        /// <summary>
        /// Creates a customer isnide the database
        /// </summary>
        /// <param name="c">Customer c to insert in the database</param>
        /// <returns>Task to await</returns>
        Task<Customer> CreateAsync(Customer c);
        /// <summary>
        /// Retrieves all the customers inside the Customer Table
        /// </summary>
        /// <returns> a list of customers</returns>
        Task<IEnumerable<Customer>> RetireveAllAsync();
        /// <summary>
        /// Retireves a single customer using its id
        /// </summary>
        /// <param name="id">id of the customer that we need to retrieve</param>
        /// <returns>List of all the Customers</returns>
        Task<Customer> RetrieveAsync(string id);
        /// <summary>
        /// Updates a specified customer
        /// </summary>
        /// <param name="id">id of the customer to update</param>
        /// <param name="c">Customer instance to use as replacement for the stored customer</param>
        /// <returns>the customer updated</returns>
        Task<Customer> UpdateAsync(string id, Customer c);
        /// <summary>
        /// Deletes a customer from the Customer table
        /// </summary>
        /// <param name="id">id of the user to delete</param>
        /// <returns>bool that tells if the operation was successful or not</returns>
        Task<bool?> DeleteAsync(string id);
    }
}

using Microsoft.AspNetCore.Mvc;
using NorthwindService.Repositories;
using Packt.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindService.Controllers
{
    // base address: api/customers
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private ICustomerRepository repo;

        // constructor inhection of repository. registered in startup
        public CustomersController(ICustomerRepository repo)
        {
            this.repo = repo;
        }

        /// <summary>
        /// gets the list of all the customers even if it is empty
        /// </summary>
        /// <param name="country">optional parameter in the url to narrow the list</param>
        /// <returns>List of Customers</returns>
        // GET: api/customers
        // GET: api/customers/?country=[country]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
        public async Task<IEnumerable<Customer>> GetCustomers(string country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                return await repo.RetireveAllAsync();
            }
            else
            {
                return (await repo.RetireveAllAsync())
                    .Where((customer) => customer.Country == country);
            }
        }

        // GET: api/customers/[id]
        [HttpGet("{id}", Name = nameof(GetCustomer))]
        [ProducesResponseType(200, Type = typeof(Customer))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCustomer(string id)
        {
            Customer c = await repo.RetrieveAsync(id);
            if (c == null)
            {
                return NotFound(); //404 not found
            }
            return Ok(c); //200 Ok with customer in the body
        }

        //POST: api/customers
        //BODY: Customer(JSON, XML)
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Customer))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] Customer c)
        {
            if (c == null)
            {
                return BadRequest();//400 Bad Request
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); //400 Bad Request
            }

            Customer added = await repo.CreateAsync(c);
            return CreatedAtRoute(//201 Created
                routeName: nameof(GetCustomer),
                routeValues: new { id = added.CustomerID.ToLower() },
                value: added);
        }

        // PUT: api/customers/[id]
        // BODY: Customer (JSON, XML)
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(string id, [FromBody] Customer c)
        {
            id = id.ToUpper();

            if (c == null || c.CustomerID != id)
            {
                return BadRequest(); // 400 bad request
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(); //400 bad request
            }

            var existing = await repo.RetrieveAsync(id);
            if (existing == null)
            {
                return NotFound(); //404 resource not found
            }

            await repo.UpdateAsync(id, c);
            return new NoContentResult();// 204 done, but no content returned
        }

        // DELETE: api/customers/[id]
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await repo.RetrieveAsync(id);

            if(existing == null)
            {
                return NotFound(); // no customer with that ID
            }

            bool? deleted = await repo.DeleteAsync(id);
            if (deleted.HasValue && deleted.Value)
            {
                return new NoContentResult(); //204 no content
            }
            else
            {
                return BadRequest($"Customer {id} was found but failed to delete.");
            }
        }
    }
}

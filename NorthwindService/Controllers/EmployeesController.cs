using Entities.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorthwindService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NorthwindService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private IEmployeeRepository repo;

        public EmployeesController(IEmployeeRepository repo)
        {
            this.repo = repo;
        }

        // GET: api/<EmployeesController>
        [HttpGet]
        [ProducesResponseType(200, Type=typeof(IEnumerable<Employee>))]
        public async Task<IEnumerable<Employee>> GetmEployees(string country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                return await repo.RetireveAllAsync();
            }
            else
            {
                return (await repo.RetireveAllAsync())
                    .Where(employee => employee.Country == country);
            }
        }

        // GET api/<EmployeesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<EmployeesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<EmployeesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<EmployeesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

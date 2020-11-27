using Entities.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NorthwindService.Helpers;
using NorthwindService.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace NorthwindService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private IEmployeeRepository repo; // cached repository, Could be changed with Redis
        private readonly JWTSettings jwtsettings;


        public EmployeesController(IEmployeeRepository repo, IOptions<JWTSettings> jwtsettings)
        {
            this.repo = repo;
            this.jwtsettings = jwtsettings.Value;
        }

        [AllowAnonymous]
        [HttpGet("login")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200, Type=typeof(IEnumerable<Employee>))]

        public async Task<ActionResult<Employee>> Login([FromBody] Employee employee) 
        {
            employee = await repo.RetrieveAsync(employee.EmployeeID.ToString());

            if (employee==null)
            {
                return NotFound();
            }

            // sign the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtsettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor 
            {
                Subject=new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, employee.EmployeeID.ToString())
                }),
                Expires=DateTime.Now.AddDays(2),
                SigningCredentials= new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            employee.Token = tokenHandler.WriteToken(token);
            
            return employee;
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

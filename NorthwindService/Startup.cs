using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Entities.Shared;
using Microsoft.AspNetCore.Mvc.Formatters;
using NorthwindService.Repositories;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication;
using NorthwindService.Handlers;

namespace NorthwindService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container. DI
        public void ConfigureServices(IServiceCollection services)
        {
            //setting up the database connection
            string databasePath = Path.Combine("..", "Northwind.db");

            // configuring dependency injection
            services.AddDbContext<Northwind>(options => options.UseSqlite($"Data Source={databasePath}"));
            services.AddControllers(options =>
            {
                Console.WriteLine("Default Output Formatters:");
                foreach (IOutputFormatter formatter in options.OutputFormatters)
                {
                    OutputFormatter mediaFomatter = formatter as OutputFormatter;
                    if (mediaFomatter == null)
                    {
                        Console.WriteLine($" {formatter.GetType().Name}");
                    }
                    else // OutputFormatter class has SupportedMediaTypes
                    {
                        Console.WriteLine("{0}, Media Types:{1}", arg0: mediaFomatter.GetType().Name, arg1: string.Join(", ", mediaFomatter.SupportedMediaTypes));
                    }
                }
            })
            .AddXmlDataContractSerializerFormatters()
            .AddXmlSerializerFormatters()
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions,BasicAuthenticationHandler>("BasicAuthentication",null);
            
            // Registering the swagger generator and define a 
            // swagger document for Northwind Service
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(name: "v1", info: new OpenApiInfo { Title="Northwind Service API", Version="v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Northwind Service Api Version 1");

                c.SupportedSubmitMethods(new[]
                {
                    SubmitMethod.Get, SubmitMethod.Post,
                    SubmitMethod.Put,SubmitMethod.Delete
                });
            });
        }
    }
}

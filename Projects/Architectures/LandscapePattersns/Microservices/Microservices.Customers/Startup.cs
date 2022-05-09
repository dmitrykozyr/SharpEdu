﻿using Microservices.Customers.DB;
using Microservices.Customers.Interfaces;
using Microservices.Customers.Providers;
using Microsoft.EntityFrameworkCore;

namespace Microservices.Customers
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CustomersDbContext>(options =>
            {
                options.UseInMemoryDatabase("Customers");
            });
            services.AddScoped<ICustomersProvider, CustomersProvider>();
            services.AddAutoMapper(typeof(Startup));
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Entities;
using RestaurantAPI.Middleware;
using RestaurantAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddControllers();
            services.AddDbContext<RestaurantDBContext>();
            services.AddScoped<RestaurantSeeder>();

            //adding auto mapping
            services.AddAutoMapper(this.GetType().Assembly);
            //add services for every entity
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<IDishService, DishService>();

            //adding middleware error handler
            services.AddScoped<ErrorHandlingMiddleware>();

            //adding middleware timer
            services.AddScoped<RequestTimeMiddleware>();

            //generating API documentation by swagger
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RestaurantSeeder seeder)
        {
            seeder.Seed();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //use created error handler middleware
            app.UseMiddleware<ErrorHandlingMiddleware>();

            //use created and registered request time middleware
            app.UseMiddleware<RequestTimeMiddleware>();
            app.UseHttpsRedirection();

            //generating API documentation by swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant_API_Documentation");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
  {
      endpoints.MapControllers();
  });
        }
    }
}

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
using Microsoft.EntityFrameworkCore;
using ReservationProject.Data;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.IO;
using AutoMapper;
using ReservationProject.Mappings;

namespace ReservationProject
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
            services.AddCors(options => options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())) ; // Make sure you call this previous to AddMvc
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
                    
                    services.AddDbContext<ReservationProejctDbContext>(options =>
                    options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("ReservationProject")));
                    //options.UseSqlServer(Configuration["ConnectionStrings"]));
            services.AddSwaggerGen(c =>
            {
                OpenApiInfo info = new OpenApiInfo();
                info.Title = "My First API";
                info.Version = "Version 1";

                c.SwaggerDoc("v1", info);
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            }
          );
            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {// It should be one of your very first registrations
            app.UseExceptionHandler("/error"); // Add this
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Make sure you call this before calling app.UseMvc()
            app.UseCors();
            //    options => options.WithOrigins("http://localhost:3000").AllowAnyMethod()
            //);
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

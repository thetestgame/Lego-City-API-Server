// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api
{
    using HealthChecks.UI.Client;
    using LegoCity.Api.Utils;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.OpenApi.Models;

    /// <summary>Startup class for the Eco Website.</summary>
    public class Startup
    {
        public Startup(IConfiguration configuration) 
        { 
            this.Configuration = configuration; 
        }

        public IConfiguration Configuration { get; }
        
        /// <summary>This method gets called by the runtime. Use this method to add services to the container.</summary>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddRestApiControllers();

            services.AddLegoPoweredUpServices();
            services.AddDiscordBotSupport(this.Configuration);

            // Enable Swashbuckle
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Lego City API",
                    Description = "A simple API to control a Lego City's trains. First iteration",
                });
            });
        }

        /// <summary>This method gets called by the runtime. Use this method to configure the HTTP request pipeline.</summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseExceptionHandler("/error-development");

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });

            }
            else
            {
                app.UseExceptionHandler("/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions { RequestPath = "/assets" });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
                endpoints.MapControllers();

                endpoints.MapHealthChecks("/api/health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }
    }
}

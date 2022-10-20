﻿// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api
{
    using LegoCity.Api.Utils;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;

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
            services.AddTimeOfDayServices();
            services.AddDiscordBotSupport(this.Configuration);

            services.AddApiVersioning();
            services.ConfigureApiVersioning(new()
            {
                { "v1", "Lego City control server V1." }
            });
        }

        /// <summary>This method gets called by the runtime. Use this method to configure the HTTP request pipeline.</summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment()) // Enable our development options if running in a development environment
            {
                app.UseDeveloperExceptionPage();
                app.AddSwaggerDocumentation(provider);
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions { RequestPath = "/assets" });

            app.UseRouting();
            app.AddApiEndpoints();
        }
    }
}

// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace LegoCity.Api.Utils
{
    /// <summary>Extension methods for <see cref="IApplicationBuilder"/>.</summary>
    public static class AppBuilderExtensions
    {
        /// <summary>Adds Swagger docs and UI support to an <see cref="IApplicationBuilder"/> instance.</summary>
        /// <param name="app"><see cref="IApplicationBuilder"/> to configure.</param>
        /// <param name="provider">Api version provider to confingure swagger ui options for.</param>
        public static void AddSwaggerDocumentation(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                    options.SwaggerEndpoint(                                 // Iterate through our defined versions and add a Swagger endpoint for each
                        $"/swagger/{description.GroupName}/swagger.json",    // Define a swagger document per version group name.
                        description.GroupName.ToUpperInvariant());
            });
        }

        /// <summary>Adds api endpoint support for our MVC controllers and the health check endpoint.</summary>
        /// <param name="app"><see cref="IApplicationBuilder"/> to configure.</param>
        public static void AddApiEndpoints(this IApplicationBuilder app) =>
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

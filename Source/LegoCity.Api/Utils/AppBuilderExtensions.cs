// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api.Utils
{
    /// <summary>
    /// Extension methods for <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class AppBuilderExtensions
    {
        /// <summary>Adds Swagger docs and UI support to an <see cref="IApplicationBuilder"/> instance.</summary>
        /// <param name="app"><see cref="IApplicationBuilder"/> to configure.</param>
        public static void AddSwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));
        }
    }
}

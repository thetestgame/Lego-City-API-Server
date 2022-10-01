// Copyright (c) Jordan Maxwell. All rights reserved.
// See LICENSE file in the project root for full license information.

namespace LegoCity.Api
{
    using Microsoft.AspNetCore;
    using NLog.Web;

    /// <summary>Main entry class into the LegoCity api service</summary>
    public class Program
    {
        /// <summary>The entry point of the application.</summary>
        public static void Main(string[] args) => CreateWebHostBuilder(args).Build().Run();
        
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(cfg => {
                    cfg.SetBasePath(Directory.GetCurrentDirectory());
                    cfg.AddJsonFile("appsettings.Local.json", optional: true);
                })
                .UseStartup<Startup>()
                .ConfigureLogging(logging => {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();

        /// <summary>Checks if the application is running as a DEBUG build.</summary>
        /// <returns>True if the application is a DEBUG build. Otherwise false</returns>
        public static bool IsDebug()
        {
            #if DEBUG
                return true;
            #else
                return false;
            #endif
        }
    }
}
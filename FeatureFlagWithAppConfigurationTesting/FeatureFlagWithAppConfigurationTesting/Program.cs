using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.Logging;

namespace FeatureFlagWithAppConfigurationTesting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var settings = config.Build();
                    config.AddAzureAppConfiguration(actions => 
                    {
                        actions.Connect(Environment.GetEnvironmentVariable("ConnectionString"))
                            .UseFeatureFlags(configure => 
                            {
                                //configure.Label = ""
                                configure.PollInterval = TimeSpan.FromSeconds(5);
                            });
                    });
                })
                .UseStartup<Startup>();
    }
}

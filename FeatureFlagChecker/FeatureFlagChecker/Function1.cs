using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;

namespace FeatureFlagChecker
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["customer"];
            if (string.IsNullOrWhiteSpace(name))
            {
                return new OkObjectResult(false);
            }

            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(action =>
            {
                action.Connect("CONFIGGOESTOHERE")
                    .UseFeatureFlags(configure =>
                    {
                        configure.PollInterval = TimeSpan.FromSeconds(5);
                    });
            });

            var config = builder.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>(config);
            serviceCollection.AddSingleton<ICustomerInfo>(new CustomerInfo
            {
                Value = name
            });
            serviceCollection.AddFeatureManagement()
                .AddFeatureFilter<DeploymentRingFilter>()
                .AddFeatureFilter<TimeWindowFilter>();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            IFeatureManager featureManager = serviceProvider.GetService<IFeatureManagerSnapshot>();

            return new OkObjectResult(featureManager.IsEnabled("WeatherForecasts"));
        }
    }
}

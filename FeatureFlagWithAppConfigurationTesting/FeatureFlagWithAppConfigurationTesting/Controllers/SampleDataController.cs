using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeatureFlagWithAppConfigurationTesting.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;

namespace FeatureFlagWithAppConfigurationTesting.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly IFeatureManager _featureManager;

        public SampleDataController(IFeatureManagerSnapshot featureManager)
        {
            _featureManager = featureManager;
        }

        [HttpGet("[action]")]
        public IActionResult WeatherForecasts(int startDateIndex)
        {
            if (!_featureManager.IsEnabled(nameof(MyFeatureFlags.WeatherForecasts)))
            {
                return NotFound();
            }

            var rng = new Random();
            return Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                DateFormatted = DateTime.Now.AddDays(index + startDateIndex).ToString("d"),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }));
        }

        public class WeatherForecast
        {
            public string DateFormatted { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; }

            public int TemperatureF
            {
                get
                {
                    return 32 + (int)(TemperatureC / 0.5556);
                }
            }
        }
    }
}

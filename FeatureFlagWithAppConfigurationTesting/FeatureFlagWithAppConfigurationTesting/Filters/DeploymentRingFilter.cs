using FeatureFlagWithAppConfigurationTesting.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement;
using System.Collections.Generic;
using System.Linq;

namespace FeatureFlagWithAppConfigurationTesting.Filters
{
    [FilterAlias("Panviva.DeploymentRingFilter")]
    public class DeploymentRingFilter : IFeatureFilter
    {
        /// <summary>
        /// This can be changed into a DB, or others
        /// </summary>
        private readonly Dictionary<int, List<string>> _ringCustomers = new Dictionary<int, List<string>>
        {
            { 0, new List<string> { "CorpsDb" } },
            { 1, new List<string> { "Telstra", "Westpack" } },
            { 2, new List<string> { "Foxtel" } },
            { 3, new List<string> { "*" } }
        };
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeploymentRingFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Evaluate(FeatureFilterEvaluationContext context)
        {
            var deploymentRingSettings = context.Parameters.Get<DeploymentRingSettings>();

            // Get info such as which customer made the request from httpContextAccessor
            // Let's just assume this info comes from httpContextAccessor
            var customer = "Telstra";

            if (!_ringCustomers.ContainsKey(deploymentRingSettings.Value))
            {
                return false;
            }

            if (_ringCustomers[deploymentRingSettings.Value].FirstOrDefault() == "*")
            {
                return true;
            }

            foreach (var ring in _ringCustomers.Keys)
            {
                if (_ringCustomers[ring].Any(x => x == customer) && deploymentRingSettings.Value >= ring)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

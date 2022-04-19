using Microsoft.Extensions.Configuration;
using System;

namespace WpfDotNetFrameworkWithFeatureFlags.FeatureManagement
{
    /// <summary>
    /// Used to evaluate whether a feature is enabled or disabled.
    /// </summary>
    class FeatureManager : IFeatureManager
    {
        private const string SectionName = "FeatureManagement";

        private readonly IConfigurationSection section;

        public FeatureManager(IConfiguration configuration)
        {
            section = configuration.GetSection(SectionName);
        }

        public bool IsEnabled(object feature)
        {
            var name = GetFeatureName(feature);
            return bool.TryParse(section[name], out bool enabled) && enabled;
        }

        private static string GetFeatureName(object feature)
        {
            var featureType = feature.GetType();
            if (!featureType.IsEnum)
            {
                throw new ArgumentException($"{nameof(feature)} must be an enum.");
            }

            return Enum.GetName(featureType, feature);
        }
    }
}
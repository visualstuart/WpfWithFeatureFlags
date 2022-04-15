using Microsoft.Extensions.Configuration;

namespace WpfDotNetFrameworkWithFeatureFlags.FeatureManagement
{
    /// <summary>
    /// Used to evaluate whether a feature is enabled or disabled.
    /// </summary>
    class FeatureManager : IFeatureManager
    {
        private const string SectionName = "FeatureManagement";

        private readonly IConfiguration configuration;
        private readonly IConfigurationSection section;

        public FeatureManager(IConfiguration configuration)
        {
            this.configuration = configuration;
            section = configuration.GetSection(SectionName);
        }

        public bool IsEnabled(string feature) =>
            bool.TryParse(section[feature], out bool enabled) && enabled;
    }
}
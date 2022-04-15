using Microsoft.Extensions.Configuration;

namespace WpfDotNetFrameworkWithFeatureFlags
{
    /// <summary>
    /// Load configuration from a JSON file 
    /// </summary>
    public class ConfigurationFromJson
    {
        private readonly string path;

        public ConfigurationFromJson(string path) => 
            this.path = path;

        public IConfiguration Build() =>
            new ConfigurationBuilder()
                .AddJsonFile(path, optional: true, reloadOnChange: false)
                .Build();
    }
}

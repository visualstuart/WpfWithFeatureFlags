using Microsoft.Extensions.Configuration;
using System.Windows;

namespace WpfDotNetFrameworkWithFeatureFlags
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // load configuration from appsettings.json file
            IConfigurationRoot config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .Build();

            var title = config["Position:Title"];
            var name = config["Position:Name"];
        }
    }
}

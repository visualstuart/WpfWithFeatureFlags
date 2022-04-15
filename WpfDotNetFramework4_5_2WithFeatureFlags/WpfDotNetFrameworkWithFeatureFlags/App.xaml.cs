using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using WpfDotNetFrameworkWithFeatureFlags.FeatureManagement;

namespace WpfDotNetFrameworkWithFeatureFlags
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Host host = new Host();

        public App()
        {
            // add services to container
            ServiceCollection services = new ServiceCollection();
            services
                .AddSingleton(new ConfigurationFromJson("appsettings.json").Build())
                .AddFeatureManagement();
            
            services.AddSingleton<MainWindow>();

            host.Services = services.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            host.Services.GetService<MainWindow>()?.Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
        }
    }

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

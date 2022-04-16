using Microsoft.Extensions.DependencyInjection;
using System.Windows;

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
                .AddConfigurationFromJson("appsettings.json")
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
}

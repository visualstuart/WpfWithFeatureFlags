using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using System;
using System.Windows;

namespace WpfDotNet6WithFeatureFlags
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;

        public App()
        {
            var builder = Host.CreateDefaultBuilder();

            // configure configuration
            builder
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                    var connectionString =
                        configBuilder.Build().GetConnectionString("AppConfig");

                // configure loading configuration from Azure App Configuration
                //  and specify the option to use feature flags from that configuration
                configBuilder
                        .AddAzureAppConfiguration(options => options
                            .Connect(connectionString)
                            .UseFeatureFlags());
                });

            // configure services
            builder
                .ConfigureServices((context, services) =>
                    {
                        services
                            .AddAzureAppConfiguration()
                            .AddFeatureManagement();

                        services.AddSingleton<MainWindow>();
                    });

            // build the host
            host = builder.Build();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            await host.StartAsync();

            host.Services.GetService<MainWindow>()?.Show();
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            await host.StopAsync(TimeSpan.FromSeconds(5));
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;
using System;
using System.Reflection;
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
            var builder = new HostBuilder();

            // configure configuration
            builder
                .UseEnvironment("Development")      // cheating...
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                    LoadUserSecrets(hostingContext, configBuilder);

                    var connectionString =
                        configBuilder.Build().GetConnectionString("AppConfig");

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

        /// <summary>
        /// Load user secrets if executing in a development environment.
        /// </summary>
        /// <param name="hostingContext"></param>
        /// <param name="configBuilder"></param>
        /// <remarks>
        /// Following the example in CreateDefaultBuilder(string[] args) in WebHost
        /// https://github.com/dotnet/aspnetcore/blob/f08285d0b6918fbb2b485d97f4e411dc9ea9a94f/src/DefaultBuilder/src/WebHost.cs
        /// </remarks>
        private static void LoadUserSecrets(HostBuilderContext hostingContext, IConfigurationBuilder configBuilder)
        {
            var environment = hostingContext.HostingEnvironment;
            if (environment.IsDevelopment())
            {
                if (!string.IsNullOrEmpty(environment.ApplicationName))
                {
                    var appAssembly = Assembly.Load(new AssemblyName(environment.ApplicationName));
                    if (appAssembly != null)
                    {
                        configBuilder.AddUserSecrets(appAssembly, optional: true);
                    }
                }
            }
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

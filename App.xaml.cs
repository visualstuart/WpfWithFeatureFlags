using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            host = new HostBuilder()
                 .ConfigureServices((context, services) =>
                     services.AddSingleton<MainWindow>())
                .Build();
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

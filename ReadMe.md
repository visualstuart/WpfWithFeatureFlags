# Sample: WPF applications with feature flags

Feature management, including programming with feature flags, is part of the .NET (Core) Platform.
The objective of these samples is to provide the same model for programming with feature flags in a WPF application
that targets .NET Framework 4.5.2. 

The goals of this sample are to:

* provide familiar .NET Platform concepts for feature flags in a .NET Framework;
* provide a good migration path if the application is modernized to the .NET Platform; and
* use existing Microsoft NuGet packages to avoid writing additional code.

# Contents

This sample consists of two Visual Studio solutions:

* **WpfDotNet6WithFeatureFlags** adds .NET 6 support feature flags to a WPF .NET 6 application.
This sample illustrates using the latest _Microsoft.Extensions_ NuGet packages for .NET 6
to load and use feature flags. This sample loads feature flags from Azure App Configuration.
This sample functions as a reference for the next solution.

* **WpfDotNetFramework4_5_2WithFeatureFlags** adds support for feature flags to a WPF application that targets
.NET Framework 4.5.2. This is accomplished using earlier _Microsoft.Extensions_ NuGet packages that are compatilble
with either .NET Framework 4.5.2 or .NET Standard 1.2, the latter being the highest version of 
.NET Standard supported by .NET Framework 4.5.2.
For more information on .NET Platforms supported by different versions of .NET Standard see 
[.NET Standard | Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).
Additionally, this sample loads feature flags from an `appsettings.json` file,
as opposed to (e.g.) Azure App Configuration to illustrate not depending on Azure services.

# Adding feature flags to a WPF .NET 6 application

As mentioned in the "Contents" section, a purpose for this application is to be a reference for
the programmatic use of feature flags in a .NET (Core) application which sets the standard for their use in
a .NET Framework application.

Feature flagging in .NET (Core) depends on the following platform capabilities
which are made available as Microsoft NuGet packages:
* *Microsoft.Extensions.Configuration* for loading configuration including feature flags;
* *Microsoft.Extensions.DependencyInjection* in order to access feature management within any class; and 
* *Microsoft.FeatureManagement* for programmatic access to feature flags.

## Setting up the WPF application to use feature flags

In Visual Studio, a newly created "WPF App" project (or using `dotnew new wfc` at the command line) 
has a minimal `App.xaml` file and an empty `App` class in the `App.xaml.cs` code-behind.

The first step is to add `Startup` and `Exit` attributes to the `<Application>` XAML in `App.xaml` as shown here,

````xml
<Application xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             ...
             Startup="Application_Startup"
             Exit="Application_Exit">
````

and add the methods named in these XAML attributes, along with a default constructor, in the code-behind, as shown.

````cs
public partial class App : Application
{
    public App()
    {
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
    }
}
````

This creates places for initializing configuration, dependency injection, and feature flag support.

Next, use the NuGet Package Manager to add these packages to the project's References. 
The version specified in parentheses indicate the latest stable version at the time of writing 
which is what is used here.

* Microsoft.Extensions.Hosting (6.0.1)
* Microsoft.Extensions.Configuration.AzureAppConfiguration (5.0.0)
* Microsoft.FeatureManagement (2.5.1)

The Microsoft.Extensions.Hosting package contains the 
[.NET Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host)
(simply named the `Host` class, and its interface `IHost`), which is similar to the `WebHost` class 
and `IWebHost` interface used in ASP.NET Core applications,
with the difference being that `Host` is used in 
all other types of .NET application.
The .NET Generic Host includes 
.NET Core configuration, dependency injection, logging, and other capabilities.

In the next code fragment, the `Host` class is used by having the `App` class
aggregate an instance of type `IHost` so that the host instance lifetime matches
the application lifetime, and use a `HostBuilder` to 
configure the configuration, add services to the DI container,
and then build the `Host` instance.

````cs
public partial class App : Application
{
    private readonly IHost host;

    public App()
    {
        var builder = new HostBuilder();

        // configure configuration
        builder
            // HACK: should load Environment from an environment variable or similar
            .UseEnvironment("Development")      
                
            .ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                LoadUserSecrets(hostingContext, configBuilder);

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

    // other members elided...
}
````

The `LoadUserSecrets` method is used to load application secrets from a local user-secrets store into the application configuration
if the host environment is "Development". 
This code is based on the implementation of [`WebHost.CreateDefaultBuilder(string[] args)`](https://github.com/dotnet/aspnetcore/blob/f08285d0b6918fbb2b485d97f4e411dc9ea9a94f/src/DefaultBuilder/src/WebHost.cs)
in the dotnet / aspnetcore GitHub repo.
Also, see the `HACK` comment for how the host environment is currently hard coded to `Development`.

For an introduction to user-secrets, see 
[Safe storage of app secrets in development in ASP.NET Core | Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets).
The purpose it to keep application secrets from being included in source code.

Specifically, this code requires that the connection string to an Azure App Configuration instance
is stored in user-secrets in the `ConnectionStrings` section with a name of `AppConfig`.
This is set up in a local development environment using the following commands
from the application project folder.

````
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:AppConfig" "<connectionString>"
````

The Azure App Configuration connection strings are obtained from the Azure Portal page for the
App Configuration instance, in the left navigation under **Settings | Access keys**.

The main attraction &mdash; configuring the application to load configuration from Azure App Configuration
and specify the option to use that configuration for feature flags &mdash; is done by calling the
`configBuilder.AddAzureAppConfiguration` method, as noted with a comment.

The final modification to the `App` class is to implement the methods for application startup and exit as shown here.
Notice how the singleton instance of the `MainWindow` class is being retrieved directly from the DI container,
and that was added to the DI container in the `App` constructor.

````cs
private async void Application_Startup(object sender, StartupEventArgs e)
{
    await host.StartAsync();

    host.Services.GetService<MainWindow>()?.Show();
}

private async void Application_Exit(object sender, ExitEventArgs e)
{
    await host.StopAsync(TimeSpan.FromSeconds(5));
}
````

## Using feature flags in the WPF application

One use of feature flags is to hide work in progress (WIP) from users who
should not be exposed to that work in progress, for example only member of my team
should see the work until it is ready for their consumption.
Use of filtering on feature flags to, for example, target only a specific set of users,
is beyond the current scope of this sample
in order to focus on the mechanism of feature flagging.
For more information on feature flag filtering see
[Use feature filters to enable conditional feature flags | Microsoft Docs](https://docs.microsoft.com/en-us/azure/azure-app-configuration/howto-feature-filters-aspnet-core).
In this sample, feature flags are configured to be either enabled or disabled
without additional logic for determining the conditions under which 
a feature flag is enabled or disabled.

Two key scenarios for using feature flags in a WPF are:
* to show or hide a set of UI elements in the application presentation; and
* to use as a condition in logic in the the code-behind that affects the behavior of the application.

Illustrating the first scenario, the code-behind for the `MainWindow` class uses
dependency injection in the constructor to inject a `featureManager` and use it to call
the `IFeatureManager.IsEnabledAsync` method to get the state of the specified feature flag.
The state of the feature flag is used to set a public property, `FeatureXVisibility`, of type `System.Windows.Visibility`.
Also note how the `MainWindow.DataContext` property is being set to `this` 
so that the `FeatureXVisibility` public property is available for data binding in the default
data context for `MainWindow`.

````cs
public partial class MainWindow : Window
{
    private readonly IFeatureManager featureManager;

    public Visibility FeatureXVisibility { get; set; }

    public MainWindow(IFeatureManagerSnapshot featureManager)
    {
        this.featureManager = featureManager;
        FeatureXVisibility = VisibilityFrom(featureManager.IsEnabledAsync("FeatureX").Result); 

        InitializeComponent();
        DataContext = this;
    }

    private static Visibility VisibilityFrom(bool isFeatureEnabled) =>
        isFeatureEnabled ? Visibility.Visible : Visibility.Hidden;

    // additional members elided...
}
````

Here is the corresponding XAML in `MainWindow.xaml`.
The `StackPanel` uses the Binding extension to data bind the `Visibility` property to the
`FeatureXVisibility` property defined in the data context.
As a result, the contents of the `StackPanel` will be shown if the feature flag was true,
and otherwise it will not be shown.

````xml
<Window x:Class="WpfDotNet6WithFeatureFlags.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        ...
        Title="MainWindow" Height="450" Width="800">
    <Grid>
    
        <StackPanel Visibility="{Binding Path=FeatureXVisibility}">
            <Button Click="button_Click">Start Feature X</Button>
        </StackPanel>

    </Grid>
</Window>
````

The sample does not demonstrate the second scenario mentioned earlier,
that of using feature flags in code-behind logic,
however it is straightforward to see how additional uses of `featureManager.IsEnabledAsync`
can be combined throughout the `MainWindow` class.

# Adding feature flags to a WPF application targeting .NET Framework 4.5.2

Let's start with a bit of relevant history in order to understand some of the limitation on options
available for this solution and how they might affect your particular situation.
* WPF 3.0 was the original version of WPF which was part of
[.NET Framework 3.0 released November 21, 2006](https://en.wikipedia.org/wiki/.NET_Framework_version_history#.NET_Framework_3.0).
* WPF 4.0 was part of [.NET Framework 4.0 which released April 12, 2010](https://en.wikipedia.org/wiki/.NET_Framework_version_history#.NET_Framework_4.0).
* In Visual Studio 2022, when creating a "WPF Application (.NET Framework)" project,
the earliest available target framework is .NET Framework 4.5.2,
and the project references PresentationFramework library version 4.0.0.0 (i.e., WPF 4.0).
* .NET Framework 4.5.2 was [released on May 5, 2014](https://en.wikipedia.org/wiki/.NET_Framework_version_history#.NET_Framework_4.5.2) and 
  [end of support is April 24, 2022](https://devblogs.microsoft.com/dotnet/net-framework-4-5-2-4-6-4-6-1-will-reach-end-of-support-on-april-26-2022/).

**Note:** I recall that there were significant breaking changes moving from WPF 3.0 to WPF 4.0,
however the details appear to have become shrouded in the mists of time.
If you are working with an earlier version of WPF or .NET Framework,
then your experiences may be different.

As a reminder, one objective of this sample is to provide a programming model for
a WPF application that targets .NET Framework that is fairly close to the 
programming model used in a modern WPF .NET application.
Additionally, we'd like to have that without trying to recreate a lot of the .NET (Core)
capabilities from scratch.

The strategy is to use some of the early Microsoft NuGet packages for .NET (Core)
that are compatible with either .NET Framework 4.5.2 or 
with .NET Standard 1.2 which, in turn, is compatible with .NET Framework 4.5.2.

**Note.** It must be understood from the outset that as early NuGet packages,
they do not have the full feature set of the latest packages, may have different APIs,
and may have bug, performance issues, and other limitations addressed in later versions of the packages.
Therefore we strive to keep the use of these packages light and unsophisticated.

## Setting up the WPF (.NET Framework) application to use feature flags

As in the WPF .NET sample, add `Startup` and `Exit` properties to the `<Application>` element in the
`App.xaml`

````xml
<Application xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             ...
             Startup="Application_Startup"
             Exit="Application_Exit">
````

and add the methods named in these XAML attributes, along with a default constructor, in the code-behind, as shown.

````cs
public partial class App : Application
{
    public App()
    {
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
    }

    private void Application_Exit(object sender, ExitEventArgs e)
    {
    }
}
````

Next, use the NuGet Package Manager to add these packages to the project's References. 
**Use the exact package versions specified in parentheses** as these are the last
versions of these packages compatible with .NET Framework 4.2.5.
If you try to upgrade the packages, the upgrade should fail with a message that they are
not compatible with the target framework.

* Microsoft.Extensions.Configuration (1.1.2)
* Microsoft.Extensions.Configuration.Json (1.1.2)
* Microsoft.Extensions.DependencyInjection (1.1.1)

Next, we're adding some small classes to help bridge some gaps between the available Microsoft.Extensions packages
and what we need to get feature flags working.

I've created a `Host` class to act like the .NET Generic Host, in this example all it does is
aggregate an `IServiceProvider`:

````cs
public class Host
{
    public IServiceProvider Services { get; set; }
}
````

Inspect the four replacement types for feature management `FeatureManagement` namespace (folder).

The `IFeatureManager` defines a single method, `IsEnabled`, with a parameter type of `object`.
The implementation will restrict this object to being a member of an enum; 
there is no way to express that in static typing in C#.
The purpose of using a enum member instead of a string is to base the feature flag usage on
a programmatic type instead of an arbitrary string, making it easier to audit for use and
prevent errors due to typos.

The `FeatureManager` class implements the `IFeatureManager` interface and gets the
application configuration using dependency injection.
The implementation looks up the feature name (which must be an enum member) in the "FeatureManagement" 
section of the configuration.

The `IFeatureManagerBuilder` interface and its corresponding `FeatureManagerBuilder` class are both empty.
Their only purpose is to keep the signature of the `AddFeatureManagement` extension method
on IServiceCollection identical to the one found in `Microsoft.FeatureManagement` NuGet packages.

And I've added a `ServiceCollectionExtensions` class with extension methods to the IServiceCollection:

````cs
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds required feature management services.
    /// </summary>
    /// <param name="services">The service collection that feature management services are added to.</param>
    /// <returns>A <see cref="IFeatureManagementBuilder"/> that can be used to customize feature management functionality.</returns>
    public static IFeatureManagementBuilder AddFeatureManagement(this IServiceCollection services)
    {
        // Add required services
        services.AddSingleton<IFeatureManager, FeatureManager>();

        return new FeatureManagementBuilder(services);
    }

    public static IServiceCollection AddConfigurationFromJson(this IServiceCollection services, string path)
    {
        services.AddSingleton(new ConfigurationFromJson(path).Build());
        return services;
    }
}
````

With this in place, the implementation of the `App` constructor follows the same structure as before, albeit simplified.

````cs
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

    // other members elided...
}
````

Instead of loading feature flag configuration from Azure App Configuration,
this sample loads configuration from `appsettings.json`.
In part this is to simplify the custom implementation in this sample
because there is no version of 
Microsoft.Extensions.Configuration that is compatible with .NET Framework 4.5.2.
In part this also helps to exemplify using an alternative and equally valid source
for configuration data. Multiple sources of configuration can be combined to create fallback
scenarios when one source isn't available or could be overridden by another source.

## Using feature flags in the WPF application

This section is the punch line of this sample: the XAML and code behind that use this are **_exactly the same_**
as those in the previous WPF .NET 6 application.
As in tha previous sample, we use dependency injection to obtain an IFeatureManager in the `MainWindow` constructor,
use it to set public properties in the data context, and use those to target data binding on the `Visibility` attributes
of various controls in the XAML.

# References

* Microsoft.FeatureManagement
  * "...standardized APIs for enabling feature flags within applications.
  Utilize this library to secure a consistent experience when developing applications that use patterns such as beta access, rollout, dark deployments, and more."
  * [Feature management overview | Microsoft Docs](https://docs.microsoft.com/en-us/azure/azure-app-configuration/concept-feature-management)
  * [GitHub source code](https://github.com/microsoft/FeatureManagement-Dotnet/tree/423501a59d8d086bbe60baa89dedcded1133213f)

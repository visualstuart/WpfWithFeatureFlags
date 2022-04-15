using Microsoft.Extensions.DependencyInjection;

namespace WpfDotNetFrameworkWithFeatureFlags.FeatureManagement
{
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
    }
}

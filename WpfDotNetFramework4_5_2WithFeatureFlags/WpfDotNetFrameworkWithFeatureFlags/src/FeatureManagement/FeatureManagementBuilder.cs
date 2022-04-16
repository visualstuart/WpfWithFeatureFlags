using Microsoft.Extensions.DependencyInjection;

namespace WpfDotNetFrameworkWithFeatureFlags.FeatureManagement
{
    internal class FeatureManagementBuilder : IFeatureManagementBuilder
    {
        public FeatureManagementBuilder(IServiceCollection _)
        {
        }
    }
}
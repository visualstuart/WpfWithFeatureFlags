namespace WpfDotNetFrameworkWithFeatureFlags.FeatureManagement
{
    public interface IFeatureManager
    {
        bool IsEnabled(object feature);
    }
}
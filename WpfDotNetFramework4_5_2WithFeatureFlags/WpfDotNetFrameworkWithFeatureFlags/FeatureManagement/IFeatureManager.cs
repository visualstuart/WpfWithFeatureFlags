namespace WpfDotNetFrameworkWithFeatureFlags.FeatureManagement
{
    public interface IFeatureManager
    {
        bool IsEnabled(string feature);
    }
}
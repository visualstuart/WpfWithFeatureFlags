using System.Windows;
using WpfDotNetFrameworkWithFeatureFlags.FeatureManagement;

namespace WpfDotNetFrameworkWithFeatureFlags
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Visibility FeatureAVisibility { get; set; }
        public Visibility FeatureBVisibility { get; set; }
        public Visibility FeatureCVisibility { get; set; }

        public MainWindow(IFeatureManager featureManager)
        {
            FeatureAVisibility = ConvertToVisibility(featureManager.IsEnabled("FeatureA"));
            FeatureBVisibility = ConvertToVisibility(featureManager.IsEnabled("FeatureB"));
            FeatureCVisibility = ConvertToVisibility(featureManager.IsEnabled("FeatureC"));

            InitializeComponent();
            DataContext = this;
        }

        private static Visibility ConvertToVisibility(bool isFeatureEnabled) =>
            isFeatureEnabled ? Visibility.Visible : Visibility.Hidden;
    }
}

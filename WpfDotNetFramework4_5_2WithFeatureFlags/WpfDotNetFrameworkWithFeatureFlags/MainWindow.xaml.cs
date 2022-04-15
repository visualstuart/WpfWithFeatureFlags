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
            FeatureAVisibility = VisibilityFrom(featureManager.IsEnabled(FeatureFlags.FeatureA));
            FeatureBVisibility = VisibilityFrom(featureManager.IsEnabled(FeatureFlags.FeatureB));
            FeatureCVisibility = VisibilityFrom(featureManager.IsEnabled(FeatureFlags.FeatureC));

            InitializeComponent();
            DataContext = this;
        }

        private static Visibility VisibilityFrom(bool isFeatureEnabled) =>
            isFeatureEnabled ? Visibility.Visible : Visibility.Hidden;
    }
}

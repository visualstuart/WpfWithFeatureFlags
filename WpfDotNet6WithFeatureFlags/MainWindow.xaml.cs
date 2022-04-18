using Microsoft.FeatureManagement;
using System.Windows;

namespace WpfDotNet6WithFeatureFlags
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IFeatureManager featureManager;

        // public properties which are exposed in the data context
        public Visibility FeatureAVisibility { get; set; }
        public Visibility FeatureBVisibility { get; set; }
        public Visibility FeatureCVisibility { get; set; }

        /// <summary>
        /// Instantiate a MainWindow object.
        /// </summary>
        /// <param name="featureManager">The feature manager.</param>
        public MainWindow(IFeatureManager featureManager)
        {
            this.featureManager = featureManager;
            FeatureAVisibility =
                VisibilityFrom(featureManager.IsEnabledAsync(FeatureFlags.FeatureA).Result);
            FeatureBVisibility =
                VisibilityFrom(featureManager.IsEnabledAsync(FeatureFlags.FeatureB).Result);
            FeatureCVisibility =
                VisibilityFrom(featureManager.IsEnabledAsync(FeatureFlags.FeatureC).Result);

            InitializeComponent();
            DataContext = this;
        }

        private static Visibility VisibilityFrom(bool isFeatureEnabled) =>
            isFeatureEnabled ? Visibility.Visible : Visibility.Hidden;
    }
}

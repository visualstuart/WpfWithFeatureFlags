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

        public Visibility FeatureXVisibility { get; set; }

        public MainWindow(IFeatureManager featureManager)
        {
            this.featureManager = featureManager;
            FeatureXVisibility = VisibilityFrom(featureManager.IsEnabledAsync("FeatureX").Result);

            InitializeComponent();
            DataContext = this;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
        }

        private static Visibility VisibilityFrom(bool isFeatureEnabled) =>
            isFeatureEnabled ? Visibility.Visible : Visibility.Hidden;
    }
}

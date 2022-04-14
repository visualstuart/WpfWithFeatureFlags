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

        public MainWindow(IFeatureManagerSnapshot featureManager)
        {
            this.featureManager = featureManager;
            FeatureXVisibility = ConvertToVisibility(featureManager.IsEnabledAsync("FeatureX").Result); 

            InitializeComponent();
            DataContext = this;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
        }

        private static Visibility ConvertToVisibility(bool isFeatureEnabled) =>
            isFeatureEnabled ? Visibility.Visible : Visibility.Hidden;
    }
}

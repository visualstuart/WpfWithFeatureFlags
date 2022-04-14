using System.Windows;

namespace WpfDotNet6DataBinding
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Greeting { get => "Hello"; set { } }
        public Visibility GreetingVisibility { get => Visibility.Hidden; set { } }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}

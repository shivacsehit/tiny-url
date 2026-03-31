using System.Windows;
using TinyUrl.WPF.Services;
using TinyUrl.WPF.ViewModels;

namespace TinyUrl.WPF
{
    public partial class MainWindow : Window
    {
        public MainWindow(UrlService svc)
        {
            InitializeComponent();
            DataContext = new MainViewModel(svc);
        }
    }
}
using System.Windows;
using TinyUrl.WPF.Services;

namespace TinyUrl.WPF
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var svc = new UrlService();

            //  Auto login on startup
            var result = await svc.LoginAsync("admin", "Admin@123");
            if (result != null)
            {
                svc.SetToken(result.Token);
            }

            // Create MainWindow with UrlService
            var mainWindow = new MainWindow(svc);
            mainWindow.Show();
        }
    }
}
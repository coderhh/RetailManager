using System.Windows;

using WPF.Pure.ViewModels;

namespace WPF.Pure
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Window window = new MainWindow
            {
                DataContext = new MainViewModel()
            };
            window.Show();
            base.OnStartup(e);
        }
    }
}

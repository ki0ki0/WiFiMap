using System;
using System.Windows;

namespace WiFiMapCore
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (e.Args.Length == 1 && e.Args[0] == "--diagnostics")
            {
                StartupUri = new Uri("Views\\DiagnosticsView.xaml",UriKind.Relative);
            }
        }
    }
}
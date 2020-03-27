using log4net;
using MaterialDesignThemes.Wpf;
using RemoteService.Properties;
using System;
using System.Windows;
using System.Windows.Threading;

namespace RemoteService
{
    /// <summary>
    /// Interação lógica para App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));
        protected override void OnStartup(StartupEventArgs e)
        {
            
            SetTheme(Settings.Default.Theme);
            base.OnStartup(e);
        }


        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string friendlyMsg = string.Format("Sinto muito que algo deu errado. O erro foi: {0}", e.Exception.Message);
            string caption = "Error";
            MessageBox.Show(friendlyMsg, caption, MessageBoxButton.OK, MessageBoxImage.Error);
            log.Error(e.Exception.Message);
            // Signal that we handled things--prevents Application from exiting
            e.Handled = true;
        }

        public void SetTheme(BaseTheme theme)
        {
            //Copied from the existing ThemeAssist class
            //https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/blob/master/MaterialDesignThemes.Wpf/ThemeAssist.cs

            string lightSource = "Theme/Theme.Light.xaml";
            string darkSource = "Theme/Theme.Dark.xaml";

            foreach (ResourceDictionary resourceDictionary in Resources.MergedDictionaries)
            {
                if (string.Equals(resourceDictionary.Source?.ToString(), lightSource, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(resourceDictionary.Source?.ToString(), darkSource, StringComparison.OrdinalIgnoreCase))
                {
                    Resources.MergedDictionaries.Remove(resourceDictionary);
                    break;
                }
            }

            if (theme == BaseTheme.Dark)
            {
                Resources.MergedDictionaries.Insert(0, new ResourceDictionary { Source = new Uri(darkSource, UriKind.Relative) });
            }
            else
            {
                //This handles both Light and Inherit
                Resources.MergedDictionaries.Insert(0, new ResourceDictionary { Source = new Uri(lightSource, UriKind.Relative) });
            }
        }
    }
}

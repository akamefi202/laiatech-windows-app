using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace laiatech_wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {
        public App()
        {

        }

        /*
        public void runWindow(String pTitle)
        {
            if (pTitle == "Preview")
            {
                //PreviewWindow previewWindow = new PreviewWindow();
                //this.Run(previewWindow);
            }
            else
            {
                //SettingsWindow settingsWindow = new SettingsWindow();
                //this.Run(settingsWindow);
            }
        }

        public void CloseWindowSafe(Window w)
        {
            if (w.Dispatcher.CheckAccess())
                w.Close();
            else
                w.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(w.Close));
        }
        */
    }
}

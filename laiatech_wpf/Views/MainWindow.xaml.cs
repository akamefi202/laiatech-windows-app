using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace laiatech_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PreviewWindow previewWindow = null;
        SettingsWindow settingsWindow = null;

        public MainWindow()
        {
            InitializeComponent();

            setContentView("Preview");
            //setContentView("Settings");
        }

        public void setContentView(String pTitle)
        {
            if (pTitle == "Preview")
            {
                if (settingsWindow != null)
                {
                    settingsWindow.Close();
                    settingsWindow = null;
                }

                previewWindow = new PreviewWindow();
                this.Content = previewWindow.Content;
                this.Title = "Camera Preview";
            }
            else
            {
                if (previewWindow != null)
                {
                    previewWindow.Close();
                    previewWindow = null;
                }
                        
                settingsWindow = new SettingsWindow();
                this.Content = settingsWindow.Content;
                this.Title = "Camera Settings";
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (previewWindow != null)
                previewWindow.Close();
            if (settingsWindow != null)
                settingsWindow.Close();
        }
    }
}
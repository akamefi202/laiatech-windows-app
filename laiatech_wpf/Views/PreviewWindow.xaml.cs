using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DirectShowLib;
    
namespace laiatech_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        CaptureDevice captureDevice = null;
        //bool cameraConnected = false;

        public PreviewWindow()
        {
            InitializeComponent();

            // initialize controls
            dayModeInit();

            // initialize devices
            Globals.deviceController = new DeviceController();
            Globals.deviceController.focus = new SliderProperty(CameraControlProperty.Focus, focusSlider);
            Globals.deviceController.zoom = new SliderProperty(CameraControlProperty.Zoom, zoomSlider);
            Globals.deviceController.pan = new SliderProperty(CameraControlProperty.Pan, panSlider);
            Globals.deviceController.tilt = new SliderProperty(CameraControlProperty.Tilt, tiltSlider);
            Globals.deviceController.init();

            // set capture view
            var panel = FindName("cameraViewPanel") as System.Windows.Forms.Panel;
            captureDevice = new CaptureDevice(panel);

            this.Closing += PreviewWindow_Closing;
        }

        private void PreviewWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            captureDevice.CloseInterfaces();
            captureDevice = null;
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).setContentView("Settings");
        }

        private void dayModeButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.dayMode = !Globals.dayMode;
            dayModeInit();
        }

        private void setDefButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.deviceController.reset();
        }

        public void dayModeInit()
        {

            System.Windows.Application app = System.Windows.Application.Current;

            if (Globals.dayMode == true)
            {
                // night mode
                app.Resources["BackgroundBrush"] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x24, 0x24, 0x24));

                app.Resources["DayModeButtonBackgroundBrush"] = new SolidColorBrush(Colors.White);
                app.Resources["DayModeButtonForegroundBrush"] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x24, 0x24, 0x24));
                app.Resources["DayModeButtonText"] = "Day Mode";
                app.Resources["DayModeButtonImage"] = new BitmapImage(new Uri("pack://application:,,,/laiatech_wpf;component/Images/day_mode.png"));

                app.Resources["SettingsButtonBackgroundBrush"] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xE7, 0x32, 0x41));

                app.Resources["LogoImage"] = new BitmapImage(new Uri("pack://application:,,,/laiatech_wpf;component/Images/laia_night_logo.png"));

                app.Resources["LabelForegroundBrush"] = new SolidColorBrush(Colors.White);

                app.Resources["SliderForegroundBrush"] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xE7, 0x32, 0x41));
                app.Resources["SliderBackgroundBrush"] = new SolidColorBrush(Colors.White);
            }
            else
            {
                // day mode
                app.Resources["BackgroundBrush"] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xf5, 0xf5, 0xf5));

                app.Resources["DayModeButtonBackgroundBrush"] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x24, 0x24, 0x24));
                app.Resources["DayModeButtonForegroundBrush"] = new SolidColorBrush(Colors.White);
                app.Resources["DayModeButtonText"] = "Night Mode";
                app.Resources["DayModeButtonImage"] = new BitmapImage(new Uri("pack://application:,,,/laiatech_wpf;component/Images/night_mode.png"));

                app.Resources["SettingsButtonBackgroundBrush"] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x24, 0x24, 0x24));

                app.Resources["LogoImage"] = new BitmapImage(new Uri("pack://application:,,,/laiatech_wpf;component/Images/laia_logo.png"));

                app.Resources["LabelForegroundBrush"] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x24, 0x24, 0x24));

                app.Resources["SliderForegroundBrush"] = new SolidColorBrush(Colors.White);
                app.Resources["SliderBackgroundBrush"] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x24, 0x24, 0x24));
            }
        }

        private void autoFocusCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Globals.deviceController.focus.setPropertyFlag(PropertyFlags.Auto);
        }

        private void autoFocusCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Globals.deviceController.focus.setPropertyFlag(PropertyFlags.Manual);
        }
    }
}

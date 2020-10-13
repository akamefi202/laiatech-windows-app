using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DirectShowLib;

namespace laiatech_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public ProfileManager profileManager = new ProfileManager();
        //public System.Windows.Forms.NotifyIcon notifyIcon = null;

        public SettingsWindow()
        {
            InitializeComponent();

            // initialize controls
            readSettingsComboBox.SelectedIndex = 0;
            writeSettingsComboBox.SelectedIndex = 0;

            profileComboBox.Items.Add("Camera Default");
            foreach (Profile p in profileManager.profileList)
                profileComboBox.Items.Add(p.title);
            profileComboBox.SelectedIndex = 0;

            //saveProfileButton.IsEnabled = false;
            //updateProfileButton.IsEnabled = false;
            //deleteProfileButton.IsEnabled = false;
            //loadProfileButton.IsEnabled = false;

            expoModeSelection.Props = "Manual*Auto";
            expoModeSelection.init();
            powerLineFreqSelection.Props = "Disabled*50 Hz*60 Hz*Auto";
            powerLineFreqSelection.init();
            backLightCompSelection.Props = "Off*On";
            backLightCompSelection.init();

            dayModeInit();

            Globals.deviceController = new DeviceController();
            Globals.deviceController.exposureMode = new ModeProperty(CameraControlProperty.Exposure, expoModeSelection);
            Globals.deviceController.exposure = new SliderProperty(CameraControlProperty.Exposure, expoSlider);
            Globals.deviceController.gain = new SliderProperty(VideoProcAmpProperty.Gain, gainSlider);
            Globals.deviceController.brightness = new SliderProperty(VideoProcAmpProperty.Brightness, brightSlider);
            Globals.deviceController.contrast = new SliderProperty(VideoProcAmpProperty.Contrast, contSlider);
            Globals.deviceController.saturation = new SliderProperty(VideoProcAmpProperty.Saturation, saturSlider);
            Globals.deviceController.sharpness = new SliderProperty(VideoProcAmpProperty.Sharpness, sharpSlider);
            Globals.deviceController.gamma = new SliderProperty(VideoProcAmpProperty.Gamma, gammaSlider);
            Globals.deviceController.whiteBalance = new SliderProperty(VideoProcAmpProperty.WhiteBalance, whiteBalSlider);
            Globals.deviceController.backlightComp = new SelectionProperty(VideoProcAmpProperty.BacklightCompensation, backLightCompSelection);
            Globals.deviceController.focus = new SliderProperty(CameraControlProperty.Focus, focusSlider);
            Globals.deviceController.zoom = new SliderProperty(CameraControlProperty.Zoom, zoomSlider);
            Globals.deviceController.pan = new SliderProperty(CameraControlProperty.Pan, panSlider);
            Globals.deviceController.tilt = new SliderProperty(CameraControlProperty.Tilt, tiltSlider);

            // initialize devices
            foreach (DsDevice dev in Globals.deviceManager.deviceList)
                cameraComboBox.Items.Add(dev.Name);
            if (Globals.deviceManager.selectedDevice != null)
                cameraComboBox.SelectedItem = Globals.deviceManager.selectedDevice.Name;

            Globals.deviceManager.DeviceAdded += new EventHandler(deviceAdded);
            Globals.deviceManager.DeviceRemoved += new EventHandler(deviceRemoved);
        }

        private void saveProfileButton_Click(object sender, RoutedEventArgs e)
        {
            Profile pp = Globals.deviceController.getCurrentSettings();
            profileManager.addProfile(pp);

            profileComboBox.Items.Add(pp.title);

            //saveProfileButton.IsEnabled = false;
        }

        private void updateProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (profileManager.selectedProfile == null)
                return;

            String orgTitle = profileManager.selectedProfile.title;
            Profile pp = Globals.deviceController.getCurrentSettings();
            pp.title = orgTitle;

            profileManager.selectedProfile = pp;
            profileManager.updateProfile(pp);

            //updateProfileButton.IsEnabled = false;
        }

        private void deleteProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (profileManager.selectedProfile == null)
                return;

            for (int i = 0; i < profileManager.profileList.Count; i++)
            {
                if (profileManager.selectedProfile.title == profileManager.profileList[i].title)
                {
                    profileManager.profileList.RemoveAt(i);
                    profileComboBox.Items.RemoveAt(i + 1);
                    break;
                }
            }

            profileManager.selectedProfile = null;
            profileComboBox.SelectedIndex = 0;

            //deleteProfileButton.IsEnabled = false;
            //loadProfileButton.IsEnabled = false;
            //saveProfileButton.IsEnabled = false;
            //updateProfileButton.IsEnabled = false;
        }

        private void loadProfileButton_Click(object sender, RoutedEventArgs e)
        {
            String pTitle = profileComboBox.SelectedItem as String;

            if (pTitle == "Camera Default")
            {
                profileManager.selectedProfile = null;
                //loadProfileButton.IsEnabled = false;
                //deleteProfileButton.IsEnabled = false;
                Globals.deviceController.reset();
            }
            else
            {
                foreach (Profile p in profileManager.profileList)
                {
                    if (p.title == pTitle)
                    {
                        profileManager.selectedProfile = p;
                        //loadProfileButton.IsEnabled = false;
                        //deleteProfileButton.IsEnabled = true;
                        Globals.deviceController.setCurrentSetting(p);
                    }
                }
            }

            //saveProfileButton.IsEnabled = false;
            //updateProfileButton.IsEnabled = false;
        }

        private void profileComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*
            if (profileManager.selectedProfile == null)
            {
                if (profileComboBox.SelectedItem as String != "Camera Default")
                    loadProfileButton.IsEnabled = true;
                else
                    loadProfileButton.IsEnabled = false;
            }
            else
            {
                if (profileComboBox.SelectedItem as String != profileManager.selectedProfile.title)
                    loadProfileButton.IsEnabled = true;
                else
                    loadProfileButton.IsEnabled = false;
            }
            */
        }

        private void cameraComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selIndex = ((ComboBox)sender).SelectedIndex;
            if (selIndex == -1) return;

            if (Globals.deviceManager.selectedDevice.DevicePath != Globals.deviceManager.deviceList[selIndex].DevicePath)
                Globals.deviceManager.selectDevice(selIndex);

            Globals.deviceController.init();
        }

        private void dayModeButton_Click(object sender, RoutedEventArgs e)
        {
            Globals.dayMode = !Globals.dayMode;
            dayModeInit();
        }

        private void cameraPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)Application.Current.MainWindow).setContentView("Preview");
        }

        private void readSettingsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void writeSettingsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void hideIconCheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void hideIconCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void deviceAdded(object sender, EventArgs e)
        {
            //this.Dispatcher.Invoke(() =>
            //{
                String devName = sender as String;
                cameraComboBox.Items.Add(devName);

                if (cameraComboBox.SelectedItem == null)
                    cameraComboBox.SelectedItem = devName;
            //});
        }

        private void deviceRemoved(object sender, EventArgs e)
        {
            //this.Dispatcher.Invoke(() =>
            //{
                String devName = sender as String;
                cameraComboBox.Items.Remove(devName);

                if (cameraComboBox.SelectedItem == null && cameraComboBox.Items.Count > 0)
                    cameraComboBox.SelectedItem = cameraComboBox.Items[cameraComboBox.Items.Count - 1];
            //});
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

                app.Resources["TabControlBackgroundBrush"] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xE7, 0x32, 0x41));
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

                app.Resources["TabControlBackgroundBrush"] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x24, 0x24, 0x24));
            }
        }
    }
}

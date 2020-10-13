using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using DirectShowLib;

namespace laiatech_wpf
{
    class DeviceManager
    {
        public List<DsDevice> deviceList = new List<DsDevice>();
        public DsDevice selectedDevice = null;

        public IAMCameraControl cameraControl = null;
        public IAMVideoProcAmp videoProcAmp = null;

        public EventHandler DeviceAdded;
        public EventHandler DeviceRemoved;

        public DeviceManager()
        {
            Globals.deviceMonitor.DShowConnected += new EventHandler(dsDeviceConnect);
            Globals.deviceMonitor.DShowDisconnected += new EventHandler(dsDeviceDisconnect);

            // add already connected devices to device list
            foreach (DsDevice dev in Globals.deviceMonitor.GetDsDevices())
                dsDeviceAdd(dev);
        }

        public void selectDevice(int pIndex)
        {
            if (pIndex == -1)
                this.selectedDevice = null;
            else
                this.selectedDevice = deviceList[pIndex];
            
            getControl();
        }

        private void dsDeviceAdd(DsDevice dev)
        {
            deviceList.Add(dev);

            if (selectedDevice == null)
            {
                selectedDevice = deviceList[0];
                getControl();
            }

            if (DeviceAdded != null)
                DeviceAdded.Invoke(dev.Name, new EventArgs());
        }

        public void dsDeviceRemove(DsDevice dev)
        {
            deviceList.Remove(dev);

            if (selectedDevice.DevicePath == dev.DevicePath)
            {
                if (deviceList.Count > 0)
                    selectedDevice = deviceList[deviceList.Count - 1];
                else
                    selectedDevice = null;

                getControl();
            }

            if (DeviceRemoved != null)
                DeviceRemoved.Invoke(dev.Name, new EventArgs());
        }

        private void dsDeviceConnect(object sender, EventArgs e)
        {
            if (!(sender is DsDevice))
                return;
            
            DsDevice dev = (DsDevice)sender;

            if (!App.Current.Dispatcher.CheckAccess())
                App.Current.Dispatcher.Invoke((MethodInvoker)(() => { dsDeviceConnectImpl(dev); }));
            else
                dsDeviceConnectImpl(dev);
        }

        private void dsDeviceConnectImpl(DsDevice devIn)
        {
            foreach (DsDevice dev in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
            {
                if (dev.DevicePath.Equals(devIn.DevicePath))
                {
                    bool flag = true;

                    foreach (DsDevice camera in this.deviceList)
                    {
                        if (dev.DevicePath.Equals(camera.DevicePath))
                        {
                            //notifyIcon.ShowBalloonTip(1000, "Webcam connected", dev.Name, ToolTipIcon.Info);
                            flag = false;
                        }
                    }

                    if (flag) 
                        dsDeviceAdd(dev);
                    break;
                }
            }
        }

        private void dsDeviceDisconnect(object sender, EventArgs e)
        {
            if (!(sender is DsDevice dev))
                return;
            
            foreach (DsDevice camera in deviceList.ToList())
            {
                if (dev.DevicePath.Equals(camera.DevicePath))
                {
                    if (!App.Current.Dispatcher.CheckAccess())
                        App.Current.Dispatcher.Invoke((MethodInvoker)(() => { dsDeviceRemove(camera); }));
                    else
                        dsDeviceRemove(camera);
                    //notifyIcon.ShowBalloonTip(2000, "Webcam disconnected", dev.Name, ToolTipIcon.Warning);
                }
            }
        }

        public void getControl()
        {
            if (selectedDevice == null)
            {
                cameraControl = null;
                videoProcAmp = null;
            }
            else
            {     
                Guid iid = typeof(IBaseFilter).GUID;
                this.selectedDevice.Mon.BindToObject(null, null, ref iid, out object camDevice);

                IBaseFilter camFilter = camDevice as IBaseFilter;
                this.cameraControl = camFilter as IAMCameraControl;
                this.videoProcAmp = camFilter as IAMVideoProcAmp;
            }
        }
    }
}

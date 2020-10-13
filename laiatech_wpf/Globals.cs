using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laiatech_wpf
{
    public enum PropertyFlags
    {
        None,
        Auto,
        Manual
    }

    static class Globals
    {
        public static DeviceMonitor deviceMonitor = new DeviceMonitor();
        public static DeviceManager deviceManager = new DeviceManager();
        public static DeviceController deviceController = new DeviceController();

        public static bool dayMode = false;

        public static int cameraViewWidth = 480;
        public static int cameraViewHeight = 320;
    }
}
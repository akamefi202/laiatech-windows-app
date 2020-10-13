using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace laiatech_wpf
{
    class SliderProperty
    {
        public Slider control = null;
        public object property = null;
        public int defaultValue = 0;
        public PropertyFlags perpertyFlag = PropertyFlags.None;

        public SliderProperty(object pProp = null, Slider pControl = null)
        {
            this.property = pProp;
            this.control = pControl;

            if (control != null)
                control.ValueChanged += Control_ValueChanged;    
        }

        private void Control_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            write();
        }

        public void init()
        {
            if (control == null)
                return;

            bool none, autoSupport, manualSupport, auto, manual;
            int pMax, pMin, pValue, pSteppingDelta, defaultValue;

            if (Object.ReferenceEquals(property.GetType(), new CameraControlProperty().GetType()) && Globals.deviceManager.cameraControl != null)
            {
                CameraControlFlags cameraFlags;

                Globals.deviceManager.cameraControl.GetRange((CameraControlProperty)property, out pMin, out pMax, out pSteppingDelta, out defaultValue, out cameraFlags);
                none = cameraFlags == CameraControlFlags.None;
                autoSupport = (cameraFlags & CameraControlFlags.Auto) == CameraControlFlags.Auto;
                manualSupport = (cameraFlags & CameraControlFlags.Manual) == CameraControlFlags.Manual;

                Globals.deviceManager.cameraControl.Get((CameraControlProperty)property, out pValue, out cameraFlags);
                auto = (cameraFlags & CameraControlFlags.Auto) == CameraControlFlags.Auto;
                manual = (cameraFlags & CameraControlFlags.Manual) == CameraControlFlags.Manual;
            }            
            else if (Globals.deviceManager.videoProcAmp != null)
            {
                VideoProcAmpFlags cameraFlags;

                Globals.deviceManager.videoProcAmp.GetRange((VideoProcAmpProperty)property, out pMin, out pMax, out pSteppingDelta, out defaultValue, out cameraFlags);
                none = cameraFlags == VideoProcAmpFlags.None;
                autoSupport = (cameraFlags & VideoProcAmpFlags.Auto) == VideoProcAmpFlags.Auto;
                manualSupport = (cameraFlags & VideoProcAmpFlags.Manual) == VideoProcAmpFlags.Manual;

                Globals.deviceManager.videoProcAmp.Get((VideoProcAmpProperty)property, out pValue, out cameraFlags);
                auto = (cameraFlags & VideoProcAmpFlags.Auto) == VideoProcAmpFlags.Auto;
                manual = (cameraFlags & VideoProcAmpFlags.Manual) == VideoProcAmpFlags.Manual;
            }
            else
            {
                none = true;
                autoSupport = manualSupport = auto = manual = false;
                pMax = pMin = pValue = pSteppingDelta = defaultValue = 0;
            }

            this.control.IsEnabled = !none;
            if (!none)
            {
                this.control.IsEnabled = auto ? false : manual;
                this.control.Minimum = pMin;
                this.control.Maximum = pMax;
                this.control.Value = pValue;
                this.control.TickFrequency = pSteppingDelta;
                this.defaultValue = defaultValue;
                this.perpertyFlag = auto ? PropertyFlags.Auto : PropertyFlags.Manual;
            }
        }

        public void reset()
        {
            if (control == null)
                return;

            control.Value = defaultValue;
            write();
        }

        public int getValue()
        {
            if (control == null)
                return 0;
            return (int)control.Value;
        }

        public void setValue(int pValue)
        {
            if (control == null)
                return;

            control.Value = pValue;
            write();
        }

        public void write()
        {
            if (Object.ReferenceEquals(property.GetType(), new CameraControlProperty().GetType()) && Globals.deviceManager.cameraControl != null)
                Globals.deviceManager.cameraControl.Set((CameraControlProperty)property, (int)control.Value, (this.perpertyFlag == PropertyFlags.Auto? CameraControlFlags.Auto: CameraControlFlags.Manual));
            else if (Globals.deviceManager.videoProcAmp != null)
                Globals.deviceManager.videoProcAmp.Set((VideoProcAmpProperty)property, (int)control.Value, (this.perpertyFlag == PropertyFlags.Auto? VideoProcAmpFlags.Auto: VideoProcAmpFlags.Manual));
        }

        public void setPropertyFlag(PropertyFlags pFlag)
        {
            if (perpertyFlag == PropertyFlags.None)
                return;

            if (pFlag == PropertyFlags.Auto)
            {
                control.IsEnabled = false;
                control.Value = control.Minimum;
            }
            else
            {
                control.IsEnabled = true;
                control.Value = defaultValue;
            }
        }
    }
}

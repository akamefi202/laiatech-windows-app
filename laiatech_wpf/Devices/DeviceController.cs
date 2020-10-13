using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laiatech_wpf
{
    class DeviceController
    {
        public ModeProperty exposureMode = new ModeProperty();
        public SliderProperty exposure = new SliderProperty();
        public SliderProperty gain = new SliderProperty();
        public SliderProperty brightness = new SliderProperty();
        public SliderProperty contrast = new SliderProperty();
        public SliderProperty saturation = new SliderProperty();
        public SliderProperty sharpness = new SliderProperty();
        public SliderProperty gamma = new SliderProperty();
        public SliderProperty whiteBalance = new SliderProperty();

        public SelectionProperty powerLineFreq = new SelectionProperty();
        public SelectionProperty backlightComp = new SelectionProperty();
        public SliderProperty focus = new SliderProperty();
        public SliderProperty zoom = new SliderProperty();
        public SliderProperty pan = new SliderProperty();
        public SliderProperty tilt = new SliderProperty();

        public void init()
        {
            exposureMode.init();
            exposure.init();
            gain.init();
            brightness.init();
            contrast.init();
            saturation.init();
            sharpness.init();
            gamma.init();
            whiteBalance.init();
            powerLineFreq.init();
            backlightComp.init();
            focus.init();
            zoom.init();
            pan.init();
            tilt.init();
        }

        public void reset()
        {
            exposureMode.reset();
            exposure.reset();
            gain.reset();
            brightness.reset();
            contrast.reset();
            saturation.reset();
            sharpness.reset();
            gamma.reset();
            whiteBalance.reset();
            powerLineFreq.reset();
            backlightComp.reset();
            focus.reset();
            zoom.reset();
            pan.reset();
            tilt.reset();
        }

        public Profile getCurrentSettings()
        {
            Profile profile = new Profile();
            profile.title = DateTime.Now.ToString();
            profile.expo = exposure.getValue();
            profile.gain = gain.getValue();
            profile.bright = brightness.getValue();
            profile.cont = contrast.getValue();
            profile.satur = saturation.getValue();
            profile.sharp = sharpness.getValue();
            profile.gamma = gamma.getValue();
            profile.whiteBal = whiteBalance.getValue();
            //profile.expo = backlightComp.getValue();
            profile.focus = focus.getValue();
            profile.zoom = zoom.getValue();
            profile.pan = pan.getValue();
            profile.tilt = tilt.getValue();

            return profile;
        }

        public void setCurrentSetting(Profile profile)
        {
            exposure.setValue(profile.expo);
            gain.setValue(profile.gain);
            brightness.setValue(profile.bright);
            contrast.setValue(profile.cont);
            saturation.setValue(profile.satur);
            sharpness.setValue(profile.sharp);
            gamma.setValue(profile.gamma);
            whiteBalance.setValue(profile.whiteBal);
            focus.setValue(profile.focus);
            zoom.setValue(profile.zoom);
            pan.setValue(profile.pan);
            tilt.setValue(profile.tilt);
        }
    }
}

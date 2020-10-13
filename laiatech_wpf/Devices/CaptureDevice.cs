using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using DirectShowLib;

namespace laiatech_wpf
{
    public class CaptureDevice
    {
        // a small enum to record the graph state
        enum PlayState
        {
            Stopped,
            Paused,
            Running,
            Init
        };

        // Application-defined message to notify app of filtergraph events
        public const int WM_GRAPHNOTIFY = 0x8000 + 1;

        public IVideoWindow videoWindow = null;
        IMediaControl mediaControl = null;
        IMediaEventEx mediaEventEx = null;
        IGraphBuilder graphBuilder = null;
        ICaptureGraphBuilder2 captureGraphBuilder = null;
        IBaseFilter sourceFilter = null;
        PlayState currentState = PlayState.Stopped;

        DsROTEntry rot = null;

        Panel panel;
        int width = 0;
        int height = 0;

        public CaptureDevice(Panel pPanel)
        {
            panel = pPanel;
            CaptureVideo();
        }

        public void CaptureVideo()
        {
            int hr = 0;

            try
            {
                // Get DirectShow interfaces
                GetInterfaces();

                // Attach the filter graph to the capture graph
                hr = this.captureGraphBuilder.SetFiltergraph(this.graphBuilder);
                DsError.ThrowExceptionForHR(hr);

                // Use the system device enumerator and class enumerator to find
                // a video capture/preview device, such as a desktop USB video camera.
                sourceFilter = FindCaptureDevice();

                // Add Capture filter to our graph.
                hr = this.graphBuilder.AddFilter(sourceFilter, "Video Capture");
                DsError.ThrowExceptionForHR(hr);

                // Render the preview pin on the video capture filter
                // Use this instead of this.graphBuilder.RenderFile
                hr = this.captureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, sourceFilter, null, null);
                DsError.ThrowExceptionForHR(hr);

                // Now that the filter has been added to the graph and we have
                // rendered its stream, we can release this reference to the filter.

                // Get Camera Size
                GetCameraSize();
                // Set video window style and position
                SetupVideoWindow();

                // Add our graph to the running object table, which will allow
                // the GraphEdit application to "spy" on our graph
                rot = new DsROTEntry(this.graphBuilder);

                // Start previewing video data
                hr = this.mediaControl.Run();
                DsError.ThrowExceptionForHR(hr);

                // Remember current state
                this.currentState = PlayState.Running;
            }
            catch
            {
                //MessageBox.Show("An unrecoverable error has occurred.");
            }
        }
            
        public void GetCameraSize()
        {
            int dr = 0;
            dr = captureGraphBuilder.FindInterface(PinCategory.Capture, MediaType.Video, sourceFilter, typeof(IAMStreamConfig).GUID, out object configObj);
            IAMStreamConfig config = (IAMStreamConfig)configObj;

            AMMediaType pmt;
            dr = config.GetFormat(out pmt);

            var header = (VideoInfoHeader)Marshal.PtrToStructure(pmt.formatPtr, typeof(VideoInfoHeader));
            width = header.BmiHeader.Width;
            height = header.BmiHeader.Height;
        }

        public void SetupVideoWindow()
        {
            if (this.videoWindow == null)
                return;

            int hr = 0;

            // Set the video window to be a child of the main window
            hr = this.videoWindow.put_Owner(panel.Handle);
            DsError.ThrowExceptionForHR(hr);

            hr = this.videoWindow.put_WindowStyle(DirectShowLib.WindowStyle.Child | DirectShowLib.WindowStyle.ClipChildren);
            DsError.ThrowExceptionForHR(hr);

            // Use helper function to position video window in client rect 
            // of main application window
            //this.videoWindow.SetWindowPosition(0, 0, panel.ClientSize.Width, panel.ClientSize.Height);
            double camRatio = (double)width / (double)height;
            double panelRatio = (double)Globals.cameraViewWidth / (double)Globals.cameraViewHeight;
            double screenScale = GetWindowsScaling();

            if (camRatio > panelRatio)
                this.videoWindow.SetWindowPosition(0, (int)(Globals.cameraViewWidth * (1 / panelRatio - 1 / camRatio) / 2 * screenScale), (int)(Globals.cameraViewWidth * screenScale), (int)(Globals.cameraViewWidth / camRatio * screenScale));
            else
                this.videoWindow.SetWindowPosition((int)(Globals.cameraViewHeight * (panelRatio - camRatio) / 2 * screenScale), 0, (int)(Globals.cameraViewHeight * camRatio * screenScale), (int)(Globals.cameraViewHeight * screenScale));

            // Make the video window visible, now that it is properly positioned
            hr = this.videoWindow.put_Visible(OABool.True);
        }

        public double GetWindowsScaling()
        {
            return (double)(Screen.PrimaryScreen.Bounds.Width / SystemParameters.PrimaryScreenWidth);
        }

        // This version of FindCaptureDevice is provide for education only.
        // A second version using the DsDevice helper class is define later.
        public IBaseFilter FindCaptureDevice()
        {
            DsDevice device = Globals.deviceManager.selectedDevice;

            if (device == null)
                return null;

            Guid iid = typeof(IBaseFilter).GUID;
            device.Mon.BindToObject(null, null, ref iid, out object camDevice);

            // An exception is thrown if cast fail
            return (IBaseFilter)camDevice;
        }

        public void GetInterfaces()
        {
            // An exception is thrown if cast fail
            this.graphBuilder = (IGraphBuilder)new FilterGraph();
            this.captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            this.mediaControl = (IMediaControl)this.graphBuilder;
            this.videoWindow = (IVideoWindow)this.graphBuilder;
        }

        public void CloseInterfaces()
        {
            // Stop previewing data
            if (this.mediaControl != null)
                this.mediaControl.StopWhenReady();

            this.currentState = PlayState.Stopped;

            // Relinquish ownership (IMPORTANT!) of the video window.
            // Failing to call put_Owner can lead to assert failures within
            // the video renderer, as it still assumes that it has a valid
            // parent window.
            if (this.videoWindow != null)
            {
                this.videoWindow.put_Visible(OABool.False);
                this.videoWindow.put_Owner(IntPtr.Zero);
            }

            // Release DirectShow interfaces
            Marshal.ReleaseComObject(this.mediaControl); this.mediaControl = null;
            Marshal.ReleaseComObject(this.videoWindow); this.videoWindow = null;
            Marshal.ReleaseComObject(this.graphBuilder); this.graphBuilder = null;
            Marshal.ReleaseComObject(this.captureGraphBuilder); this.captureGraphBuilder = null;

            if (this.sourceFilter != null)
            {
                Marshal.ReleaseComObject(this.sourceFilter);
                this.sourceFilter = null;
            }
        }

        public void ChangePreviewState(bool showVideo)
        {
            int hr = 0;

            // If the media control interface isn't ready, don't call it
            if (this.mediaControl == null)
                return;

            if (showVideo)
            {
                if (this.currentState != PlayState.Running)
                {
                    // Start previewing video data
                    hr = this.mediaControl.Run();
                    this.currentState = PlayState.Running;
                }
            }
            else
            {
                // Stop previewing video data
                hr = this.mediaControl.StopWhenReady();
                this.currentState = PlayState.Stopped;
            }
        }

        public void HandleGraphEvent()
        {
            int hr = 0;
            EventCode evCode;
            IntPtr evParam1, evParam2;

            if (this.mediaEventEx == null)
                return;

            while (this.mediaEventEx.GetEvent(out evCode, out evParam1, out evParam2, 0) == 0)
            {
                // Free event parameters to prevent memory leaks associated with
                // event parameter data.  While this application is not interested
                // in the received events, applications should always process them.
                hr = this.mediaEventEx.FreeEventParams(evCode, evParam1, evParam2);
                DsError.ThrowExceptionForHR(hr);

                // Insert event processing code here, if desired
            }
        }
    }
}

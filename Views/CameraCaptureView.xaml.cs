using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing; // For Bitmap handling
using System.IO; // For MemoryStream handling
using System.Drawing.Imaging;

namespace xcube_proj.Views
{
    public partial class CameraCaptureView : Window
    {
        private VideoCaptureDevice _videoCaptureDevice;
        private Bitmap _capturedFrame; // Store the latest frame captured from the camera

        public event Action<BitmapImage> OnImageCaptured;

        public CameraCaptureView(VideoCaptureDevice videoCaptureDevice)
        {
            InitializeComponent();
            _videoCaptureDevice = videoCaptureDevice;
            _videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame; // Subscribe to NewFrame event
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Store the current frame
            _capturedFrame = (Bitmap)eventArgs.Frame.Clone();
        }

        private void CaptureImage()
        {
            if (_capturedFrame != null)
            {
                // Convert the captured frame to BitmapImage
                BitmapImage capturedImage = ConvertBitmapToBitmapImage(_capturedFrame);
                // Raise the event
                OnImageCaptured?.Invoke(capturedImage);
            }
            else
            {
                MessageBox.Show("No frame available to capture.");
            }
        }

        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Bmp);
                memoryStream.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        // Ensure to stop the camera when the window is closed
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_videoCaptureDevice != null && _videoCaptureDevice.IsRunning)
            {
                _videoCaptureDevice.SignalToStop();
                _videoCaptureDevice.WaitForStop();
                _videoCaptureDevice.NewFrame -= VideoCaptureDevice_NewFrame; // Unsubscribe from the event
            }
        }
    }
}

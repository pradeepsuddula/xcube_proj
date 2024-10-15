using AForge.Video.DirectShow;
using AForge.Video;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows;
using xcube_proj.Command;
using System.Drawing;
using System.Drawing.Imaging;

namespace xcube_proj.ViewModel
{
    public class CameraCaptureViewModel : INotifyPropertyChanged
    {
        private VideoCaptureDevice _videoCaptureDevice;
        private BitmapImage _videoFeed;
        private BitmapImage _capturedImage;

        public BitmapImage VideoFeed
        {
            get => _videoFeed;
            set
            {
                _videoFeed = value;
                OnPropertyChanged(nameof(VideoFeed));
            }
        }

        public BitmapImage CapturedImage
        {
            get => _capturedImage;
            set
            {
                _capturedImage = value;
                OnPropertyChanged(nameof(CapturedImage));
            }
        }

        public ICommand CaptureCommand { get; }
        public ICommand ConfirmCommand { get; }
        public ICommand CloseCommand { get; }

        // New constructor that accepts VideoCaptureDevice
        public CameraCaptureViewModel(VideoCaptureDevice videoCaptureDevice)
        {
            _videoCaptureDevice = videoCaptureDevice ?? throw new ArgumentNullException(nameof(videoCaptureDevice));
            _videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame;

            CaptureCommand = new RelayCommand(CaptureImage);
            ConfirmCommand = new RelayCommand(ConfirmImage);
            CloseCommand = new RelayCommand(CloseWindow);

            _videoCaptureDevice.Start(); // Start the camera
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Convert the frame to BitmapImage and set it to VideoFeed
            using (Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone())
            {
                VideoFeed = ConvertBitmapToBitmapImage(bitmap);
            }
        }

        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                return bitmapImage;
            }
        }

        private void CaptureImage(object obj)
        {
            CapturedImage = VideoFeed; // Capture the current frame as the captured image
        }

        private void ConfirmImage(object obj)
        {
            // Here you can add logic to confirm the captured image and pass it back to the view model using an event or callback
            MessageBox.Show("Image confirmed!");
        }

        private void CloseWindow(object obj)
        {
            // Close the camera window
            Application.Current.Shutdown();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            if (_videoCaptureDevice != null && _videoCaptureDevice.IsRunning)
            {
                _videoCaptureDevice.SignalToStop();
                _videoCaptureDevice.WaitForStop();
            }
        }
    }
}

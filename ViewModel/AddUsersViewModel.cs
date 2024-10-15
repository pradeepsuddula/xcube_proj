using AForge.Video;
using AForge.Video.DirectShow;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using xcube_proj.Command;
using xcube_proj.Database;
using xcube_proj.Views;

namespace xcube_proj.ViewModel
{
    public class AddUserViewModel : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _age;
        public string Age
        {
            get => _age;
            set
            {
                _age = value;
                OnPropertyChanged(nameof(Age));
            }
        }

        private DateTime _dateOfBirth;
        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                _dateOfBirth = value;
                OnPropertyChanged(nameof(DateOfBirth));
            }
        }

        private string _contactNumber;
        public string ContactNumber
        {
            get => _contactNumber;
            set
            {
                _contactNumber = value;
                OnPropertyChanged(nameof(ContactNumber));
            }
        }

        private BitmapImage _profilePicture;
        public BitmapImage ProfilePicture
        {
            get => _profilePicture;
            set
            {
                _profilePicture = value;
                OnPropertyChanged(nameof(ProfilePicture));
            }
        }

        private VideoCaptureDevice _videoCaptureDevice;
        private FilterInfoCollection _videoDevices;

        public ICommand CaptureFromCameraCommand { get; }
        public ICommand ChooseFromFileCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand StopCameraCommand { get; } 

        public AddUserViewModel()
        {
            CaptureFromCameraCommand = new RelayCommand(CaptureImageFromCamera);
            ChooseFromFileCommand = new RelayCommand(ChooseImageFromFile);
            SaveCommand = new RelayCommand(SaveUser);
            StopCameraCommand = new RelayCommand(StopCamera); // Initialize the stop command

            InitializeCamera();
        }

        private void InitializeCamera()
        {
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (_videoDevices.Count > 0)
            {
                _videoCaptureDevice = new VideoCaptureDevice(_videoDevices[0].MonikerString);
                _videoCaptureDevice.NewFrame += VideoCaptureDevice_NewFrame; // Attach the event handler
            }
            else
            {
                MessageBox.Show("No video devices found.");
            }
        }

        private void CaptureImageFromCamera(object obj)
        {
            try
            {
                if (_videoCaptureDevice != null && !_videoCaptureDevice.IsRunning)
                {
                    _videoCaptureDevice.Start();
                    // Open camera capture in full-screen mode
                    var cameraWindow = new CameraCaptureView(_videoCaptureDevice);
                    cameraWindow.OnImageCaptured += (image) =>
                    {
                        ProfilePicture = image; // Set captured image to ProfilePicture
                    };
                    cameraWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Camera is already running.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while accessing the camera: {ex.Message}");
            }
        }

        private void VideoCaptureDevice_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            // Convert the frame to BitmapImage and set it to ProfilePicture
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            ProfilePicture = ConvertBitmapToBitmapImage(bitmap);
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

        private void ChooseImageFromFile(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    ProfilePicture = new BitmapImage(new Uri(openFileDialog.FileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while selecting the image: {ex.Message}");
                }
            }
        }

        private DatabaseHelper _databaseHelper = new DatabaseHelper();
        private void SaveUser(object obj)
        {
            try
            {
                string profilePicturePath = null;
                if (ProfilePicture != null)
                {
                    // Save the profile picture to a file
                    string filePath = Path.Combine(Environment.CurrentDirectory, $"{Name}_profile.png");
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(ProfilePicture));
                        encoder.Save(fileStream);
                    }
                    profilePicturePath = filePath;
                }

                _databaseHelper.AddUser(Name, int.Parse(Age), DateOfBirth, ContactNumber, profilePicturePath);
                MessageBox.Show("User saved successfully!");

                ResetFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving the user: {ex.Message}");
            }
        }

        private void ResetFields()
        {
            Name = string.Empty;
            Age = string.Empty;
            DateOfBirth = DateTime.Now;
            ContactNumber = string.Empty;
            ProfilePicture = null;
        }

        private void StopCamera(object obj)
        {
            if (_videoCaptureDevice != null && _videoCaptureDevice.IsRunning)
            {
                _videoCaptureDevice.SignalToStop();
                _videoCaptureDevice.WaitForStop(); 
                _videoCaptureDevice.NewFrame -= VideoCaptureDevice_NewFrame; 
                _videoCaptureDevice = null; 
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Dispose pattern for cleanup
        public void Dispose()
        {
            StopCamera(null); 
            Debug.WriteLine("Checking videocapturedevice instance::" + _videoCaptureDevice);
            if (_videoCaptureDevice != null)
            {
                _videoCaptureDevice.NewFrame -= VideoCaptureDevice_NewFrame; 
                _videoCaptureDevice = null;
            }
        }

        internal void StopCamera()
        {
            if (_videoCaptureDevice != null && _videoCaptureDevice.IsRunning)
            {
                _videoCaptureDevice.SignalToStop();
                _videoCaptureDevice.WaitForStop(); 
                _videoCaptureDevice.NewFrame -= VideoCaptureDevice_NewFrame; 
                _videoCaptureDevice = null; 
            }
        }
    }
}

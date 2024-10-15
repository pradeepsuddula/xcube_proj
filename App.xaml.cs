using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using xcube_proj.ViewModel;

namespace xcube_proj
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private AddUserViewModel addUserViewModel; // Reference to the ViewModel

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            addUserViewModel = new AddUserViewModel(); // Initialize your ViewModel here
            Application.Current.Exit += Current_Exit; // Subscribe to Exit event
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            // Stop the camera if it's running and dispose of the ViewModel
            addUserViewModel?.StopCamera();
            addUserViewModel?.Dispose();
        }
    }
}


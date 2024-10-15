using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using xcube_proj.Command;
using xcube_proj.Model;
namespace xcube_proj.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object currentView;
        private bool isMenuExpanded = true;
        private User selectedUser;
        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        public object CurrentView
        {
            get { return currentView; }
            set
            {
                currentView = value;
                OnPropertyChanged(nameof(CurrentView));

                // Update SelectedUser when AllUsersViewModel is active
                if (currentView is AllUsersViewModel allUsersViewModel)
                {
                    SelectedUser = allUsersViewModel.SelectedUser;
                    allUsersViewModel.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(allUsersViewModel.SelectedUser))
                        {
                            SelectedUser = allUsersViewModel.SelectedUser;
                        }
                    };
                }
            }
        }

        public bool IsMenuExpanded
        {
            get { return isMenuExpanded; }
            set
            {
                isMenuExpanded = value;
                OnPropertyChanged(nameof(IsMenuExpanded));
            }
        }

        // Commands for menu navigation
        public ICommand ShowAddUserCommand { get; }
        public ICommand ShowAllUsersCommand { get; }
        public ICommand ShowMapViewCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand ToggleMenuCommand { get; } // Command for toggling the menu

        private readonly AddUserViewModel addUserViewModel;
        private readonly AllUsersViewModel allUsersViewModel;
        private readonly MapViewModel mapViewModel;

        public MainViewModel()
        {
            addUserViewModel = new AddUserViewModel();
            allUsersViewModel = new AllUsersViewModel();
            mapViewModel = new MapViewModel();

            // Initialize commands with appropriate views
            ShowAddUserCommand = new RelayCommand(o => CurrentView = addUserViewModel);
            ShowAllUsersCommand = new RelayCommand(o => CurrentView = allUsersViewModel);
            ShowMapViewCommand = new RelayCommand(o => CurrentView = mapViewModel);
            CloseCommand = new RelayCommand(CloseApplication);
            ToggleMenuCommand = new RelayCommand(o => IsMenuExpanded = !IsMenuExpanded); // Toggle the menu visibility

            CurrentView = addUserViewModel; // Set the default view
        }

        private void CloseApplication(object obj)
        {
            Application.Current.Shutdown();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

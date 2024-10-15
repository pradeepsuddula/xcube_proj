using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xcube_proj.Database;
using xcube_proj.Model;

namespace xcube_proj.ViewModel
{
    public class AllUsersViewModel : INotifyPropertyChanged
    {
        private DatabaseHelper _databaseHelper = new DatabaseHelper();

        public ObservableCollection<User> Users { get; set; }

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

        public AllUsersViewModel()
        {
            Users = new ObservableCollection<User>(_databaseHelper.GetAllUsers());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

}

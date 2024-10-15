using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using xcube_proj.Command;

namespace xcube_proj.ViewModel
{
    public class MapViewModel
    {

        private double _scale = 1.0;
        private double _translateX = 0;
        private double _translateY = 0;

        public double Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                OnPropertyChanged(nameof(Scale));
            }
        }

        public double TranslateX
        {
            get => _translateX;
            set
            {
                _translateX = value;
                OnPropertyChanged(nameof(TranslateX));
            }
        }

        public double TranslateY
        {
            get => _translateY;
            set
            {
                _translateY = value;
                OnPropertyChanged(nameof(TranslateY));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand ZoomInCommand => new RelayCommand(_ => Scale *= 1.1);
        public ICommand ZoomOutCommand => new RelayCommand(_ => Scale /= 1.1);
    }
}


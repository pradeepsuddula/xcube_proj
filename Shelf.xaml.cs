using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace xcube_proj
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isDragging;
        private Point _dragStartPoint;
        private const int MinWindowWidth = 500; // Minimum width
        private const int MinWindowHeight = 300; // Minimum height

        public MainWindow()
        {
            InitializeComponent();
            // Attach event handlers for mouse actions
            MouseLeftButtonDown += Window_MouseLeftButtonDown;
            MouseMove += Window_MouseMove;
            MouseLeftButtonUp += Window_MouseLeftButtonUp;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                // Start dragging the window
                _isDragging = true;
                _dragStartPoint = e.GetPosition(this);
                Mouse.Capture(this); // Capture mouse to ensure it stays on this window
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                // Calculate the new position based on mouse movement
                Point currentPosition = e.GetPosition(this);
                double deltaX = currentPosition.X - _dragStartPoint.X;
                double deltaY = currentPosition.Y - _dragStartPoint.Y;

                // Only move the window if there is significant movement
                if (Math.Abs(deltaX) > 0 || Math.Abs(deltaY) > 0)
                {
                    Left += deltaX;
                    Top += deltaY;
                    // Update the starting point for the next move
                    _dragStartPoint = currentPosition;
                }
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                // Stop dragging the window
                _isDragging = false;
                Mouse.Capture(null); // Release mouse capture
            }
        }

        // Override OnRenderSizeChanged to handle resizing
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (Width < MinWindowWidth)
            {
                Width = MinWindowWidth;
            }
            if (Height < MinWindowHeight)
            {
                Height = MinWindowHeight;
            }
        }
    }
}

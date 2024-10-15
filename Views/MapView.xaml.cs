using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace xcube_proj.Views
{
    public partial class MapView : UserControl
    {
        private Point _previousMousePosition;
        private bool _isDragging = false;

        public MapView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Ensure the RenderTransform is a TransformGroup
            var transformGroup = (TransformGroup)MapImage.RenderTransform;

            // Get the ScaleTransform from the TransformGroup
            var scaleTransform = (ScaleTransform)transformGroup.Children[0];

            // Zoom in or out
            if (e.Delta > 0)
            {
                scaleTransform.ScaleX *= 1.1; // Zoom in
                scaleTransform.ScaleY *= 1.1; // Zoom in
            }
            else
            {
                scaleTransform.ScaleX /= 1.1; // Zoom out
                scaleTransform.ScaleY /= 1.1; // Zoom out
            }

            e.Handled = true; // Mark event as handled
        }

        private void ScrollViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                _isDragging = true;
                _previousMousePosition = e.GetPosition(ScrollViewer);
                ScrollViewer.CaptureMouse(); // Capture mouse events
            }
        }

        private void ScrollViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            ScrollViewer.ReleaseMouseCapture(); // Release mouse capture
        }

        private void ScrollViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var currentMousePosition = e.GetPosition(ScrollViewer);
                var offsetX = currentMousePosition.X - _previousMousePosition.X;
                var offsetY = currentMousePosition.Y - _previousMousePosition.Y;

                var transformGroup = (TransformGroup)MapImage.RenderTransform;
                var translateTransform = (TranslateTransform)transformGroup.Children[1];

                translateTransform.X += offsetX;
                translateTransform.Y += offsetY;

                _previousMousePosition = currentMousePosition; // Update previous position
            }
        }

        private void ScrollViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            _isDragging = false; // Stop dragging if mouse leaves
            ScrollViewer.ReleaseMouseCapture();
        }
    }
}

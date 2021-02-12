using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AdonisUI.Controls;

namespace Editor
{
    public partial class MainWindow : AdonisWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool _isDragging = false;
        private Point _previousPosition;

        // This very much breaks the mvvm principe but it's just faster right now..
        private void UIElement_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).MouseWheelEvent(e);
        }


        private Point _startPosition;
        private Point _endPosition;

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var relativePosition = e.GetPosition(MonoGameContentControl);
                if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.LeftShift))
                {
                    if (_previousPosition != default(Point))
                    {
                        var delta = Point.Subtract(relativePosition, _previousPosition);


                        ((MainWindowViewModel)this.DataContext).Move(delta.X, delta.Y);
                    }

                    _previousPosition = relativePosition;
                }
                else
                {

                    ((MainWindowViewModel) this.DataContext).TransformSelection(_startPosition.X, _startPosition.Y, relativePosition.X, relativePosition.Y);
                }
            }
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _previousPosition = e.GetPosition(MonoGameContentControl);

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.LeftShift))
            {
                _startPosition = _previousPosition;
            }
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                _endPosition = e.GetPosition(MonoGameContentControl);

                ((MainWindowViewModel)this.DataContext).Select(_startPosition.X, _startPosition.Y, _endPosition.X, _endPosition.Y);
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                _endPosition = e.GetPosition(MonoGameContentControl);

                ((MainWindowViewModel)this.DataContext).Select(_startPosition.X, _startPosition.Y, _endPosition.X, _endPosition.Y, true);
            }
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl) //if this event fired from TabControl then enter
            {
                if (tabControl.SelectedIndex == 0)
                {
                    ((MainWindowViewModel)this.DataContext).ActivateSpriteTab();
                }
                else if (tabControl.SelectedIndex == 1)
                {
                    ((MainWindowViewModel)this.DataContext).ActiveNoiseTab();
                }
                else if (tabControl.SelectedIndex == 2)
                {

                }
            }
        }
    }
}

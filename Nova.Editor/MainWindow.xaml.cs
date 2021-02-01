using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AdonisUI.Controls;
using Microsoft.Win32;

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
            ((MainWindowViewModel) this.DataContext).MouseWheelEvent(e);
        }

        private void UIElement_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                var relativePosition = e.GetPosition(MonoGameContentControl);
                if (_previousPosition != default(Point))
                {
                    var delta = Point.Subtract(relativePosition, _previousPosition);


                    ((MainWindowViewModel)this.DataContext).Move(delta.X, delta.Y);
                }

                _previousPosition = relativePosition;
            }
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _previousPosition = e.GetPosition(MonoGameContentControl);
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl tabControl) //if this event fired from TabControl then enter
            {
                if (tabControl.SelectedIndex == 0)
                {
                    ((MainWindowViewModel) this.DataContext).ActivateSpriteTab();
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

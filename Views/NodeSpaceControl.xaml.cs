using Nodify.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Nodify.Views
{
    public partial class NodeSpaceControl : UserControl
    {
        private const double MinZoom = 0.75, MaxZoom = 1.25, ZoomStep = 0.1;

        private bool _isCreatingContainer;
        private Point _containerStart;
        private readonly Rectangle _tempRect;

        private bool _isPanning;
        private Point _panStart;

        private object _dragItem;
        private Point _dragMouseStart;
        private Point _dragItemStart;

        private ConnectorViewModel _dragConnector;

        private NodeViewModel _currentMoveNode;
        private NodeControl _ghostControl;

        public MainViewModel ViewModel { get; }

        public NodeSpaceControl(MainViewModel viewModel)
        {
            InitializeComponent();

            DataContext = this;
            ViewModel = viewModel;

            _tempRect = new Rectangle
            {
                Stroke = Brushes.Orange,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(50, 255, 165, 0)),
                Visibility = Visibility.Collapsed,
                IsHitTestVisible = false
            };
            Loaded += (s, e) =>
            {
                SpaceCanvas.Children.Add(_tempRect);
            };
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pt = e.GetPosition(SpaceCanvas);

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                _isCreatingContainer = true;
                _containerStart = pt;
                Canvas.SetLeft(_tempRect, pt.X);
                Canvas.SetTop(_tempRect, pt.Y);
                _tempRect.Width = _tempRect.Height = 0;
                _tempRect.Visibility = Visibility.Visible;
                return;
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                _isPanning = true;
                _panStart = e.GetPosition(this);
                SpaceCanvas.CaptureMouse();
                return;
            }

            ViewModel.BeginDrag(null);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pt = e.GetPosition(SpaceCanvas);

            if (_currentMoveNode != null)
            {
                _ghostControl.Opacity = 0.5;
                Mouse.OverrideCursor = Cursors.SizeAll;
                Canvas.SetLeft(_ghostControl, pt.X);
                Canvas.SetTop(_ghostControl, pt.Y);
            }


            if (_isCreatingContainer)
            {
                var x = Math.Min(pt.X, _containerStart.X);
                var y = Math.Min(pt.Y, _containerStart.Y);
                _tempRect.Width = Math.Abs(pt.X - _containerStart.X);
                _tempRect.Height = Math.Abs(pt.Y - _containerStart.Y);
                Canvas.SetLeft(_tempRect, x);
                Canvas.SetTop(_tempRect, y);
                return;
            }

            if (_isPanning)
            {
                var delta = e.GetPosition(this) - _panStart;
                _panStart = e.GetPosition(this);
                var scale = SpaceScale.ScaleX;
                var nx = SpaceTranslate.X + delta.X / scale;
                var ny = SpaceTranslate.Y + delta.Y / scale;

                var maxX = ActualWidth - (SpaceCanvas.Width * scale);
                var maxY = ActualHeight - (SpaceCanvas.Height * scale);
                SpaceTranslate.X = Math.Min(0, Math.Max(maxX, nx));
                SpaceTranslate.Y = Math.Min(0, Math.Max(maxY, ny));

                return;
            }

            ViewModel.UpdateDrag(pt);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var pt = e.GetPosition(SpaceCanvas);

            if (_currentMoveNode != null)
            {
                ViewModel.AddNodeCmd.Execute((e.GetPosition(SpaceCanvas), _currentMoveNode));
                ViewModel.EndDrag(null);
                _currentMoveNode = null;
                Mouse.OverrideCursor = null;
                Mouse.OverrideCursor = null;
                if (_ghostControl != null)
                {
                    SpaceCanvas.Children.Remove(_ghostControl);
                    _ghostControl = null;
                }
            }

            if (_isCreatingContainer)
            {
                _isCreatingContainer = false;
                _tempRect.Visibility = Visibility.Collapsed;

                var rect = new Rect(
                    Canvas.GetLeft(_tempRect),
                    Canvas.GetTop(_tempRect),
                    _tempRect.Width,
                    _tempRect.Height
                );
                ViewModel.AddContainerCmd.Execute(rect);
            }

            if (_isPanning)
            {
                _isPanning = false;
                SpaceCanvas.ReleaseMouseCapture();
            }

            ViewModel.EndDrag(null);
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl)) return;

            var old = SpaceScale.ScaleX;
            var delta = e.Delta > 0 ? ZoomStep : -ZoomStep;
            var ns = Math.Clamp(old + delta, MinZoom, MaxZoom);
            var ratio = ns / old;
            var mp = e.GetPosition(SpaceCanvas);

            var nx = (SpaceTranslate.X - mp.X) * ratio + mp.X;
            var ny = (SpaceTranslate.Y - mp.Y) * ratio + mp.Y;

            var maxX = ActualWidth - SpaceCanvas.Width * ns;
            var maxY = ActualHeight - SpaceCanvas.Height * ns;
            SpaceTranslate.X = Math.Min(0, Math.Max(maxX, ns));
            SpaceTranslate.Y = Math.Max(0, Math.Max(maxY, ny));

            SpaceScale.ScaleX = SpaceScale.ScaleY = ns;
            e.Handled = true;
        }

        private void NodeControl_OnConnectorMouseDown(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is not Ellipse { Tag: ConnectorViewModel c }) return;
            _dragConnector = c;
            ViewModel.BeginDrag(c);
            e.Handled = true;
        }

        private void NodeControl_OnConnectorMouseUp(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is not Ellipse { Tag: ConnectorViewModel c }) return;
            ViewModel.EndDrag(c);
            _dragConnector = null;
            e.Handled = true;
        }

        private void NodeControl_OnNodeMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not NodeControl { DataContext: NodeViewModel vm } ctl) return;
            var pos = e.GetPosition(SpaceCanvas);

            _dragItem = vm;
            _dragMouseStart = pos;
            _dragItemStart = new Point(vm.X, vm.Y);
            ctl.CaptureMouse();
            e.Handled = true;
        }

        private void NodeControl_OnNodeMouseMove(object sender, MouseEventArgs e)
        {
            if (_currentMoveNode != null)
            {
                Mouse.OverrideCursor = Cursors.No;
            }
            var pt = e.GetPosition(SpaceCanvas);
            ViewModel.UpdateDrag(pt);
            if (_dragItem is not NodeViewModel vm || e.LeftButton != MouseButtonState.Pressed) return;
            var dx = pt.X - _dragMouseStart.X;
            var dy = pt.Y - _dragMouseStart.Y;
            ViewModel.TryMoveNode(vm, SpaceCanvas.ActualWidth, SpaceCanvas.ActualHeight, _dragItemStart.X + dx,
                _dragItemStart.Y + dy);
            e.Handled = true;
        }

        private void NodeControl_OnNodeMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentMoveNode != null)
            {
                Mouse.OverrideCursor = null;
                _currentMoveNode = null;
                if (_ghostControl != null)
                {
                    SpaceCanvas.Children.Remove(_ghostControl);
                    _ghostControl = null;
                }
            }

            if (_dragItem is not NodeViewModel) return;
            (sender as NodeControl)?.ReleaseMouseCapture();
            _dragItem = null;
            e.Handled = true;
        }

        private void NodeControl_OnNodeMouseRightClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not NodeControl node) return;
            if(node.DataContext is not NodeViewModel vm) return;

            ViewModel.RemoveNode(vm);
            e.Handled = true;
        }

        private void UIElement_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ConnectionControl connection) return;
            if(connection.DataContext is not ConnectionViewModel vm) return;
            ViewModel.RemoveConnection(vm);
            e.Handled = true;
        }

        private void ContainerControl_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not NodeContainerControl { DataContext: ContainerViewModel vm } ctl) return;
            _dragItem = vm;
            _dragMouseStart = e.GetPosition(SpaceCanvas);
            _dragItemStart = new Point(vm.X, vm.Y);
            ctl.CaptureMouse();
            e.Handled = true;
        }

        private void ContainerControl_OnMouseMove(object sender, MouseEventArgs e)
        {
            var pt = e.GetPosition(SpaceCanvas);
            ViewModel.UpdateDrag(pt);
            if (_dragItem is not ContainerViewModel vm || e.LeftButton != MouseButtonState.Pressed) return;
            var dx = pt.X - _dragMouseStart.X;
            var dy = pt.Y - _dragMouseStart.Y;
            vm.X = _dragItemStart.X + dx;
            vm.Y = _dragItemStart.Y + dy;
            e.Handled = true;
        }

        private void ContainerControl_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_dragItem is not ContainerViewModel) return;
            (sender as NodeContainerControl)?.ReleaseMouseCapture();
            _dragItem = null;
        }

        private void MenuLibraryControl_OnMenuMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not Grid grid) return;
            if (grid.DataContext is not NodeViewModel model) return;
            _currentMoveNode = model;
            _ghostControl = new NodeControl
            {
                DataContext = model,
                Opacity = 0,
                IsHitTestVisible = false
            };
            SpaceCanvas.Children.Add(_ghostControl);
            Panel.SetZIndex(_ghostControl, int.MaxValue);
            Canvas.SetLeft(_ghostControl, model.X);
            Canvas.SetTop(_ghostControl, model.Y);
            Mouse.OverrideCursor = Cursors.None;
        }

        private void MenuLibraryControl_OnMenuMouseMove(object sender, MouseEventArgs e)
        {
            if (_currentMoveNode != null)
            {
                if (_ghostControl != null) _ghostControl.Opacity = 0;
                Mouse.OverrideCursor = Cursors.No;
            }
        }

        private void MenuLibraryControl_OnMenuMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_ghostControl != null)
            {
                SpaceCanvas.Children.Remove(_ghostControl);
                _ghostControl = null;
            }

            _currentMoveNode = null;
            Mouse.OverrideCursor = null;
            e.Handled = true;
        }
    }
}
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Nodify.Helpers;
using Nodify.ViewModels;

namespace Nodify.Views
{
    public partial class NodeSpaceControl : UserControl
    {
        private const double MinZoom = 0.75, MaxZoom = 1.25, ZoomStep = 0.1;
        private const double MinContainerW = 250, MinContainerH = 125;
        private const int STEPS = 5;

        public NodeSpaceViewModel ViewModel { get; }

        private bool _isCreatingContainer;
        private Point _containerStart;
        private Rectangle _tempRect;

        private bool _isPanning;
        private Point _panStart;

        private NodeViewModel? _dragNode;
        private Point _initialMouse, _initialNodePos;

        private NodeContainerViewModel? _dragContainer;
        private Point _dragContMouseStart;
        private double _dragContStartX, _dragContStartY;

        public NodeSpaceControl()
        {
            InitializeComponent();
            ViewModel = new NodeSpaceViewModel();
            DataContext = this;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SpaceTranslate.X = (ActualWidth - SpaceCanvas.Width) / 2;
            SpaceTranslate.Y = (ActualHeight - SpaceCanvas.Height) / 2;

            _tempRect = new Rectangle
            {
                Stroke = Brushes.Orange,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(50, 255, 165, 0)),
                Visibility = Visibility.Collapsed,
                IsHitTestVisible = false
            };
            SpaceCanvas.Children.Add(_tempRect);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(SpaceCanvas);

            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                _containerStart = pos;
                Canvas.SetLeft(_tempRect, pos.X);
                Canvas.SetTop(_tempRect, pos.Y);
                _tempRect.Width = _tempRect.Height = 0;
                _tempRect.Visibility = Visibility.Visible;
                _isCreatingContainer = true;
                return;
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                _isPanning = true;
                _panStart = e.GetPosition(this);
                SpaceCanvas.CaptureMouse();
                return;
            }

            foreach (var cvm in ViewModel.Containers.Reverse<NodeContainerViewModel>())
            {
                if (!(pos.X >= cvm.X) || !(pos.X <= cvm.X + cvm.Width)
                                      || !(pos.Y >= cvm.Y) || !(pos.Y <= cvm.Y + cvm.Height)) continue;
                _dragContainer = cvm;
                _dragContMouseStart = pos;
                _dragContStartX = cvm.X;
                _dragContStartY = cvm.Y;
                SpaceCanvas.CaptureMouse();
                e.Handled = true;
                return;
            }

            var hit = VisualTreeHelper.HitTest(SpaceCanvas, pos)?.VisualHit;
            var nodeCtrl = FindAncestorHelper.FindAncestor<NodeControl>(hit);
            if (nodeCtrl?.DataContext is NodeViewModel nvm)
            {
                _dragNode = nvm;
                _initialMouse = pos;
                _initialNodePos = new Point(nvm.X, nvm.Y);
                nvm.DragStartX = nvm.X;
                nvm.DragStartY = nvm.Y;
                SpaceCanvas.CaptureMouse();
                e.Handled = true;
                return;
            }

            ViewModel.BeginDrag(null);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(SpaceCanvas);

            if (_isCreatingContainer)
            {
                var x = Math.Min(pos.X, _containerStart.X);
                var y = Math.Min(pos.Y, _containerStart.Y);
                _tempRect.Width = Math.Abs(pos.X - _containerStart.X);
                _tempRect.Height = Math.Abs(pos.Y - _containerStart.Y);
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

            if (_dragContainer != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var delta = pos - _dragContMouseStart;
                _dragContainer.X = _dragContStartX + delta.X;
                _dragContainer.Y = _dragContStartY + delta.Y;
                return;
            }

            if (_dragNode != null && e.LeftButton == MouseButtonState.Pressed)
            {
                var dx = pos.X - _initialMouse.X;
                var dy = pos.Y - _initialMouse.Y;
                ViewModel.TryMoveNode(
                    _dragNode,
                    SpaceCanvas.ActualWidth, SpaceCanvas.ActualHeight,
                    _initialNodePos.X + dx, _initialNodePos.Y + dy
                );
                e.Handled = true;
                return;
            }

            ViewModel.UpdateDrag(pos);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_isCreatingContainer)
            {
                _isCreatingContainer = false;
                _tempRect.Visibility = Visibility.Collapsed;

                var w = _tempRect.Width;
                var h = _tempRect.Height;
                if (w >= MinContainerW && h >= MinContainerH)
                {
                    var x = Canvas.GetLeft(_tempRect);
                    var y = Canvas.GetTop(_tempRect);
                    var r = new Rect(x, y, w, h);
                    if (!ViewModel.Containers.Any(c =>
                        new Rect(c.X, c.Y, c.Width, c.Height).IntersectsWith(r)))
                    {
                        ViewModel.Containers.Add(new NodeContainerViewModel
                        {
                            X = x,
                            Y = y,
                            Width = w,
                            Height = h
                        });
                    }
                }
            }

            if (_isPanning)
            {
                _isPanning = false;
                SpaceCanvas.ReleaseMouseCapture();
            }

            if (_dragContainer != null)
            {
                _dragContainer = null;
                SpaceCanvas.ReleaseMouseCapture();
                e.Handled = true;
                return;
            }

            if (_dragNode != null)
            {
                if (ViewModel.IsOverlapping(_dragNode, _dragNode.X, _dragNode.Y))
                {
                    var (sx, sy) = ViewModel.FindSafePosition(
                        _dragNode, STEPS,
                        _dragNode.DragStartX, _dragNode.DragStartY,
                        _dragNode.X, _dragNode.Y
                    );
                    _dragNode.X = sx;
                    _dragNode.Y = sy;
                }
                _dragNode = null;
                SpaceCanvas.ReleaseMouseCapture();
                e.Handled = true;
                return;
            }

            ViewModel.EndDrag(null);
        }

        private void Canvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.AddNodeCmd.Execute(e.GetPosition(SpaceCanvas));
        }

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
                return;

            var oldS = SpaceScale.ScaleX;
            var d = e.Delta > 0 ? ZoomStep : -ZoomStep;
            var ns = Math.Clamp(oldS + d, MinZoom, MaxZoom);
            var ratio = ns / oldS;
            var mp = e.GetPosition(SpaceCanvas);

            var nx = (SpaceTranslate.X - mp.X) * ratio + mp.X;
            var ny = (SpaceTranslate.Y - mp.Y) * ratio + mp.Y;

            var maxX = ActualWidth - SpaceCanvas.Width * ns;
            var maxY = ActualHeight - SpaceCanvas.Height * ns;
            SpaceTranslate.X = Math.Min(0, Math.Max(maxX, nx));
            SpaceTranslate.Y = Math.Min(0, Math.Max(maxY, ny));

            SpaceScale.ScaleX = SpaceScale.ScaleY = ns;
            e.Handled = true;
        }

        private void NodeControl_OnConnectorMouseDown(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is not Ellipse ell || ell.Tag is not ConnectorViewModel cvm) return;
            ViewModel.BeginDrag(cvm);
            e.Handled = true;
        }

        private void NodeControl_OnConnectorMouseUp(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is not Ellipse ell || ell.Tag is not ConnectorViewModel cvm) return;
            ViewModel.EndDrag(cvm);
            e.Handled = true;
        }
    }
}
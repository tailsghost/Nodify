using Nodify.Helpers;
using Nodify.ViewModels;
using System.ComponentModel;
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
        private Vector _dragContPointerOffset;

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

            ViewModel.UpdateDrag(pos);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_dragContainer != null)
            {
                _dragContainer = null;
                e.Handled = true;
                return;
            }

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

            ViewModel.EndDrag(null);
        }

        private void Canvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
            => ViewModel.AddNodeCmd.Execute(e.GetPosition(SpaceCanvas));

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
            if (e.OriginalSource is not Ellipse { Tag: ConnectorViewModel cvm }) return;
            ViewModel.BeginDrag(cvm);
            e.Handled = true;
        }

        private void NodeControl_OnConnectorMouseUp(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is not Ellipse { Tag: ConnectorViewModel cvm }) return;
            ViewModel.EndDrag(cvm);

            e.Handled = true;
        }

        private void NodeControl_OnNodeMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not NodeControl ctl) return;
            if (ctl.DataContext is not NodeViewModel cvm) return;
            var pos = e.GetPosition(SpaceCanvas);
            _dragNode = cvm;
            _initialMouse = pos;
            _initialNodePos = new Point(cvm.X, cvm.Y);
            cvm.DragStartX = cvm.X;
            cvm.DragStartY = cvm.Y;
            e.Handled = true;
        }

        private void NodeControl_OnNodeMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is not NodeControl ctl) return;
            if (ctl.DataContext is not NodeViewModel node) return;
            if (_dragNode == null) return;

            // 1) Сначала убираем ноду из всех контейнеров, в которых она могла быть
            bool wasInAny = false;
            foreach (var container in ViewModel.Containers)
            {
                if (container.Nodes.Contains(_dragNode))
                {
                    container.Nodes.Remove(_dragNode);
                    wasInAny = true;
                }
            }

            // 2) Если перекрытие было, сдвигаем обратно
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

            // 3) Пробуем добавить в новый контейнер
            bool added = false;
            foreach (var container in ViewModel.Containers)
            {
                var rect = new Rect(container.X, container.Y, container.Width, container.Height);
                var nodeRect = new Rect(_dragNode.X, _dragNode.Y, _dragNode.Width, _dragNode.Height);

                if (!rect.Contains(nodeRect))
                    continue;

                container.Nodes.Add(_dragNode);
                added = true;
                break;
            }

            // 4) Если раньше была в каком-то контейнере, а теперь не попала ни в один — разрываем все связи
            if (wasInAny && !added)
            {
                // Собираем все уникальные подключения
                var toDisconnect = new List<ConnectionViewModel>();
                foreach (var input in _dragNode.Inputs)
                {
                    if (input.Connection != null && !toDisconnect.Contains(input.Connection))
                        toDisconnect.Add(input.Connection);
                }
                foreach (var output in _dragNode.Outputs)
                {
                    if (output.Connection != null && !toDisconnect.Contains(output.Connection))
                        toDisconnect.Add(output.Connection);
                }

                // Разрываем каждую связь: чистим обе стороны и удаляем из глобального списка ViewModel.Connections
                foreach (var conn in toDisconnect)
                {
                    if (conn.Source.Connection == conn)
                        conn.Source.Connection = null;
                    if (conn.Target.Connection == conn)
                        conn.Target.Connection = null;

                    // Предполагаем, что все связи хранятся в коллекции ViewModel.Connections
                    if (ViewModel.Connections.Contains(conn))
                        ViewModel.Connections.Remove(conn);
                }
            }

            // 5) Завершаем перетаскивание
            _dragNode = null;
            SpaceCanvas.ReleaseMouseCapture();
            e.Handled = true;
        }

        private void NodeControl_OnNodeMove(object sender, MouseEventArgs e)
        {
            if (sender is not NodeControl ctl) return;
            if (ctl.DataContext is not NodeViewModel cvm) return;
            if (_dragNode == null || e.LeftButton != MouseButtonState.Pressed) return;
            var pos = e.GetPosition(SpaceCanvas);
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

        private void NodeControl_OnNodeContainerMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not NodeContainerControl ctl) return;

            if (ctl.DataContext is not NodeContainerViewModel cvm) return;

            var pos = e.GetPosition(SpaceCanvas);

            _dragContainer = cvm;
            _dragContMouseStart = pos;
            _dragContStartX = cvm.X;
            _dragContStartY = cvm.Y;
            _dragContainer.CacheNodePositions();
            ctl.CaptureMouse();
            e.Handled = true;
        }


        private void NodeControl_OnNodeContainerMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is not NodeContainerControl ctl) return;

            var cvm = ctl.DataContext as NodeContainerViewModel;
            if (cvm == null) return;

            _dragContainer = null;
            e.Handled = true;
        }

        private void NodeControl_OnNodeContainerMove(object sender, MouseEventArgs e)
        {
            if (sender is not NodeContainerControl ctl) return;

            if (ctl.DataContext is not NodeContainerViewModel cvm) return;
            if (_dragContainer == null || e.LeftButton != MouseButtonState.Pressed) return;
            var pos = e.GetPosition(SpaceCanvas);
            var delta = pos - _dragContMouseStart;

            _dragContainer.NodesRaiseChanged(_dragContStartX, delta.X, _dragContStartY, delta.Y);
            return;
        }
    }
}
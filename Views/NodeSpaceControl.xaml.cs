using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Nodify.ViewModels;

namespace Nodify.Views
{
    public partial class NodeSpaceControl : UserControl
    {
        public NodeSpaceViewModel ViewModel { get; init; }

        private NodeViewModel _dragNodeVm;
        private Point _initialMousePos;
        private Point _initialNodePos;
        private bool _isNodeDragging;

        public NodeSpaceControl()
        {
            ViewModel = new NodeSpaceViewModel();
            DataContext = this;
            InitializeComponent();
        }

        private void SpaceCanvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pt = e.GetPosition(SpaceCanvas);
            var hit = VisualTreeHelper.HitTest(SpaceCanvas, pt)?.VisualHit;
            var nodeCtrl = FindAncestor<NodeControl>(hit);
            if (nodeCtrl != null)
            {
                _dragNodeVm = nodeCtrl.DataContext as NodeViewModel;
                _initialMousePos = pt;
                _initialNodePos = new Point(_dragNodeVm.X, _dragNodeVm.Y);
                SpaceCanvas.CaptureMouse();
                e.Handled = true;
                return;
            }
            ViewModel.BeginDrag(null);
        }


        private void SpaceCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            var pt = e.GetPosition(SpaceCanvas);

            if (_dragNodeVm != null)
            {
                if (!_isNodeDragging)
                {
                    var dx = Math.Abs(pt.X - _initialMousePos.X);
                    var dy = Math.Abs(pt.Y - _initialMousePos.Y);
                    if (dx >= SystemParameters.MinimumHorizontalDragDistance ||
                        dy >= SystemParameters.MinimumVerticalDragDistance)
                    {
                        _isNodeDragging = true;
                    }
                }

                if (_isNodeDragging)
                {
                    var newX = _initialNodePos.X + (pt.X - _initialMousePos.X);
                    var newY = _initialNodePos.Y + (pt.Y - _initialMousePos.Y);
                    ViewModel.TryMoveNode(_dragNodeVm, newX, newY);
                    e.Handled = true;
                    return;
                }
            }

            ViewModel.UpdateDrag(pt);
        }

        private void SpaceCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_dragNodeVm != null)
            {
                _isNodeDragging = false;
                _dragNodeVm = null;
                SpaceCanvas.ReleaseMouseCapture();
                e.Handled = true;
                return;
            }
            ViewModel.EndDrag(null);
        }

        private void SpaceCanvas_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e) =>
            ViewModel.AddNodeCmd.Execute(e.GetPosition(SpaceCanvas));

        private void NodeControl_OnConnectorMouseDown(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is not Ellipse ellipse) return;
            if (ellipse.Tag is not ConnectorViewModel connectorVM) return;

            ViewModel.BeginDrag(connectorVM);
            e.Handled = true;
        }

        private void NodeControl_OnConnectorMouseUp(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is not Ellipse ellipse) return;
            if (ellipse.Tag is not ConnectorViewModel connectorVM) return;
            ViewModel.EndDrag(connectorVM);
            e.Handled = true;
        }

        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T typed) return typed;
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T t) yield return t;
                foreach (var descendant in FindVisualChildren<T>(child))
                    yield return descendant;
            }
        }
    }
}
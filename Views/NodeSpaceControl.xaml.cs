using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using Nodify.ViewModels;

namespace Nodify.Views
{
    /// <summary>
    /// Логика взаимодействия для NodeSpaceControl.xaml
    /// </summary>
    public partial class NodeSpaceControl : UserControl
    {
        public NodeSpaceViewModel ViewModel { get; init; }
        public NodeSpaceControl()
        {
            ViewModel = new NodeSpaceViewModel();
            DataContext = this;
            InitializeComponent();
        }

        private void SpaceCanvas_OnMouseMove(object sender, MouseEventArgs e) =>
            ViewModel.UpdateDrag(e.GetPosition(SpaceCanvas));

        private void SpaceCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => ViewModel.EndDrag(null);

        private void SpaceCanvas_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e) =>
            ViewModel.AddNodeCmd.Execute(e.GetPosition(SpaceCanvas));

        private void NodeControl_OnConnectorMouseDown(object sender, RoutedEventArgs e)
        {
            if(e.OriginalSource is not Ellipse ellipse) return;
            if(ellipse.Tag is not ConnectorViewModel connectorVM) return;
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
    }
}

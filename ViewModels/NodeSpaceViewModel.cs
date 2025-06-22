using Nodify.Helpers;
using Nodify.Models;
using Nodify.ViewModels;
using Nodify.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

public class NodeSpaceViewModel : BaseViewModel
{
    private Point _tempEnd;
    private Point _tempControl1;
    private Point _tempControl2;
    private bool _isConnecting;
    private Point _tempStart;

    private ConnectorModel _dragFrom;

    private GraphViewModel Graph { get; } = new(new GraphModel());

    public ObservableCollection<NodeViewModel> Nodes { get; }
    public ObservableCollection<ContainerViewModel> Containers { get; }
    public ObservableCollection<ConnectionViewModel> Connections { get; }

    public ICommand AddNodeCmd { get; }
    public ICommand AddContainerCmd { get; }
    public ICommand BeginConnectCmd { get; }
    public ICommand CompleteConnectCmd { get; }

    public Point TempEndPoint
    {
        get => _tempEnd;
        set
        {
            _tempEnd = value;
            OnPropertyChanged();
        }
    }
    public bool IsConnecting
    {
        get => _isConnecting;
        private set
        {
            _isConnecting = value;
            OnPropertyChanged();
        }
    }
    public Point TempStart
    {
        get => _tempStart;
        private set
        {
            _tempStart = value;
            OnPropertyChanged();
        }
    }

    public Point TempControl1 { get => _tempControl1; private set { _tempControl1 = value; OnPropertyChanged();}  }
    public Point TempControl2 { get => _tempControl2; private set { _tempControl2 = value; OnPropertyChanged(); } }

    public NodeSpaceViewModel()
    {
        Nodes = new ObservableCollection<NodeViewModel>();
        Containers = new ObservableCollection<ContainerViewModel>();
        Connections = new ObservableCollection<ConnectionViewModel>();

        AddNodeCmd = new RelayCommand(p =>
        {
            if (p is not Point pt) return;
            var m = Graph.Graph.AddNode("N" + (Nodes.Count + 1), pt.X, pt.Y);
            Nodes.Add(new NodeViewModel(m,159,200));
        });
        AddContainerCmd = new RelayCommand(p =>
        {
            if (p is not Rect r) return;
            var m = Graph.Graph.AddContainer(r.X, r.Y, r.Width, r.Height);
            Containers.Add(new ContainerViewModel(m));
        }, o =>
        {
            if (o is not Rect r) return false;
            return !Containers.Any(c => new Rect(c.X, c.Y, c.Width, c.Height).IntersectsWith(r));
        });
        BeginConnectCmd = new RelayCommand(p =>
        {
            if (p is not ConnectorModel cm) return;
            _dragFrom = cm;
            TempStart = new Point(_dragFrom.Parent.X, _dragFrom.Parent.Y);
            IsConnecting = true;
        });
        CompleteConnectCmd = new RelayCommand(p =>
        {
            if (IsConnecting && p is ConnectorModel to && _dragFrom != null && to != _dragFrom)
            {
                var source = _dragFrom.IsInput ? _dragFrom : to;
                var target = _dragFrom.IsInput ? to : _dragFrom;
                var e = Graph.Graph.AddEdge(_dragFrom, to);
                source.ConnectedTo = target;
                target.ConnectedTo = source;
                Connections.Add(new ConnectionViewModel(e));
            }
            IsConnecting = false;
        });
    }

    public void BeginDrag(ConnectorViewModel vmConnector)
    {
        _dragFrom = vmConnector?.Model;
        IsConnecting = true;
        if(_dragFrom == null) return;
        TempStart = new Point(_dragFrom.Parent.X + _dragFrom.Parent.Width / 2,
            _dragFrom.Parent.Y + _dragFrom.Parent.Height / 2);
        TempEndPoint = TempStart;
        UpdateBezier();
    }

    public void UpdateDrag(Point mousePos)
    {
        if (!IsConnecting || _dragFrom == null) return;
        TempEndPoint = mousePos;
        UpdateBezier();
    }

    public void EndDrag(ConnectorViewModel vmConnector)
    {
        if (IsConnecting && vmConnector != null && _dragFrom != null && vmConnector.Model != _dragFrom)
        {
            CompleteConnectCmd.Execute(vmConnector.Model);
        }

        IsConnecting = false;
        _dragFrom = null;
    }

    public bool TryMoveNode(NodeViewModel node, double maxX, double maxY, double newX, double newY)
    {
        var clX = Math.Max(0, Math.Min(newX, maxX - node.Width));
        var clY = Math.Max(0, Math.Min(newY, maxY - node.Height));
        node.X = clX; node.Y = clY;
        return true;
    }

    public (double X, double Y) FindSafePosition(NodeViewModel node, int steps,
        double startX, double startY, double endX, double endY)
    {
        double sx = startX, sy = startY;
        for (var i = 1; i <= steps; i++)
        {
            var t = i / (double)steps;
            var tx = startX + (endX - startX) * t;
            var ty = startY + (endY - startY) * t;
            if (IsOverlapping(node, tx, ty)) break;
            sx = tx; sy = ty;
        }
        return (sx, sy);
    }

    public bool IsOverlapping(NodeViewModel node, double x, double y)
    {
        for (var index = 0; index < Nodes.Count; index++)
        {
            var n = Nodes[index];
            if (n == node) continue;
            var ox = x < n.X + n.Width && x + node.Width > n.X;
            var oy = y < n.Y + n.Height && y + node.Height > n.Y;
            if (ox && oy) return true;
        }

        return false;
    }

    public void GetAllConnectedGroups()
    {
       var result = Graph.GetAllConnectedGroups();
       var result1 = Graph.GetConnectedNodes(Nodes.First().Node);
    }

    private void UpdateBezier()
    {
        var dx = TempEndPoint.X - TempStart.X;
        var offset = Math.Abs(dx) * 0.3;

        TempControl1 = new Point(TempStart.X + (dx >= 0 ? offset : -offset), TempStart.Y);
        TempControl2 = new Point(TempEndPoint.X - (dx >= 0 ? offset : -offset), TempEndPoint.Y);
    }

    private void ResetDrag()
    {
        _dragFrom = null;
        IsConnecting = false;
        TempStart = TempEndPoint = new Point();
        UpdateBezier();
    }
}

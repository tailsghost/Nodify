using Nodify.Helpers;
using Nodify.Models;
using Nodify.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Nodify.ViewModels;

public class MainViewModel : BaseViewModel
{
    private Point _tempEnd;
    private Point _tempControl1;
    private Point _tempControl2;
    private bool _isConnecting;
    private Point _tempStart;

    private ConnectorModel _dragFrom;

    protected GraphViewModel Graph { get; } = new(new GraphModel());

    public ObservableCollection<NodeViewModel> Nodes { get; }
    public ObservableCollection<ContainerViewModel> Containers { get; }
    public ObservableCollection<ConnectionViewModel> Connections { get; }
    public ObservableCollection<MenuLibrary> MenuLibrary { get; protected set; } = [];

    public ICommand AddNodeCmd { get; }
    public ICommand AddContainerCmd { get; }
    public ICommand BeginConnectCmd { get; }
    public ICommand CompleteConnectCmd { get; }

    public Point TempEndPoint
    {
        get => _tempEnd;
        set
        {
            if(_tempEnd == value)return;
            _tempEnd = value;
            OnPropertyChanged();
        }
    }
    public bool IsConnecting
    {
        get => _isConnecting;
        private set
        {
            if(_isConnecting == value) return;
            _isConnecting = value;
            OnPropertyChanged();
        }
    }
    public Point TempStart
    {
        get => _tempStart;
        private set
        {
            if(_tempStart == value) return;
            _tempStart = value;
            OnPropertyChanged();
        }
    }

    public Point TempControl1 { get => _tempControl1; private set { _tempControl1 = value; OnPropertyChanged();}  }
    public Point TempControl2 { get => _tempControl2; private set { _tempControl2 = value; OnPropertyChanged(); } }

    public MainViewModel()
    {
        Nodes = new ObservableCollection<NodeViewModel>();
        Containers = new ObservableCollection<ContainerViewModel>();
        Connections = new ObservableCollection<ConnectionViewModel>();

        AddNodeCmd = new RelayCommand(p =>
        {
            if(p is not (Point pt, NodeViewModel node)) return;

            var newNode = new NodeViewModel(new NodeModel(node.Name, node.Description,node.Node.InputsInfo, node.Node.OutputsInfo, node.IsFinalBlock));

            var m = Graph.Graph.AddNode(newNode.Node, pt.X, pt.Y);
            Nodes.Add(newNode);
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
            TempStart = new Point(_dragFrom.X, _dragFrom.Y);
            IsConnecting = true;
        });
        CompleteConnectCmd = new RelayCommand(p =>
        {
            if (IsConnecting && p is ConnectorViewModel to && _dragFrom != null && to.Model != _dragFrom)
            {
                var e = Graph.Graph.AddEdge(_dragFrom, to.Model);
                Connections.Add(new ConnectionViewModel(e));
            }
            IsConnecting = false;
        }, o =>
        {
            if (IsConnecting && o is ConnectorViewModel to && _dragFrom != null && to.Model != _dragFrom)
            {
                return to.AllowConnect(_dragFrom);
            }

            return false;
        });
    }

    public void BeginDrag(ConnectorViewModel vmConnector)
    {
        _dragFrom = vmConnector?.Model;
        IsConnecting = true;
        if(_dragFrom == null) return;
        TempStart = new Point(_dragFrom.X,
            _dragFrom.Y);
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
            CompleteConnectCmd.Execute(vmConnector);
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

    private void UpdateBezier()
    {
        var dx = TempEndPoint.X - TempStart.X;
        var offset = Math.Abs(dx) * 0.3;

        TempControl1 = new Point(TempStart.X + (dx >= 0 ? offset : -offset), TempStart.Y);
        TempControl2 = new Point(TempEndPoint.X - (dx >= 0 ? offset : -offset), TempEndPoint.Y);
    }
}

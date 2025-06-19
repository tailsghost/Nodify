using Nodify.Helpers;
using Nodify.ViewModels;
using Nodify.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

public class NodeSpaceViewModel : BaseViewModel
{
    private ConnectorViewModel _dragStart;
    private Point _mousePos;

    public ObservableCollection<NodeViewModel> Nodes { get; } = [];
    public ObservableCollection<ConnectionViewModel> Connections { get; } = [];

    public ICommand AddNodeCmd { get; }

    public PointCollection TempLine
    {
        get
        {
            if (_dragStart == null) return null;
            return new PointCollection
                {
                    TempStart,
                    new Point(_mousePos.X, TempStart.Y),
                    TempEnd
                };
        }
    }

    private Point _tempStart;
    public Point TempStart
    {
        get => _tempStart;
        set
        {
            _tempStart = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TempLine));
        }
    }

    private Point _tempEnd;
    public Point TempEnd
    {
        get => _tempEnd;
        set
        {
            _tempEnd = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TempLine));
        }
    }

    private bool _isDragging;
    public bool IsDragging
    {
        get => _isDragging;
        set
        {
            _isDragging = value;
            OnPropertyChanged();
        }
    }

    private Point _tempControl1;
    public Point TempControl1
    {
        get => _tempControl1;
        set
        {
            if (_tempControl1 == value) return;
            _tempControl1 = value;
            OnPropertyChanged();
        }
    }

    private Point _tempControl2;
    public Point TempControl2
    {
        get => _tempControl2;
        set
        {
            if (_tempControl2 != value)
            {
                _tempControl2 = value;
                OnPropertyChanged();
            }
        }
    }

    public NodeSpaceViewModel()
    {
        Nodes.Add(new NodeViewModel("A", 50, 100));
        Nodes.Add(new NodeViewModel("B", 250, 150));
        Nodes.Add(new NodeViewModel("C", 450, 250));
        Nodes.Add(new NodeViewModel("C", 600, 225));

        AddNodeCmd = new RelayCommand(p =>
        {
            if (p is not Point pt) return;
            var nm = $"N{Nodes.Count + 1}";
            var node = new NodeViewModel(nm, pt.X, pt.Y);
            if (!IsOverlapping(node, node.X, node.Y))
            {
                Nodes.Add(node);
            }
        });

    }

    public void BeginDrag(ConnectorViewModel c)
    {
        _dragStart = c;
        IsDragging = true;
        if (c != null)
        {
            TempStart = new Point(c.X, c.Y);
            TempEnd = TempStart;
            UpdateTempBezier();
        }
    }

    public void UpdateDrag(Point p)
    {
        if (!IsDragging || _dragStart == null) return;

        _mousePos = p;
        TempStart = new Point(_dragStart.X, _dragStart.Y);
        TempEnd = p;
        UpdateTempBezier();
    }

    public void EndDrag(ConnectorViewModel c)
    {
        if (_dragStart == null || c == null)
        {
            ResetDrag();
            return;
        }

        if (_dragStart.IsInput == c.IsInput)
        {
            ResetDrag();
            return;
        }

        var source = _dragStart.IsInput ? _dragStart : c;
        var target = _dragStart.IsInput ? c : _dragStart;

        if (source.Parent == target.Parent)
        {
            ResetDrag();
            return;
        }

        if (target.IsInput && Connections.Any(conn => conn.Target == target))
        {
            ResetDrag();
            return;
        }

        if (target.IsConnected)
        {
            ResetDrag();
            return;
        }

        var connection = new ConnectionViewModel(source, target);
        source.Connection = connection;
        target.Connection = connection;

        Connections.Add(connection);

        ResetDrag();
    }


    public bool TryMoveNode(NodeViewModel node, double newX, double newY)
    {
        var appWindow = Application.Current.MainWindow;

        if (appWindow == null)
            return false;

        var maxX = appWindow.ActualWidth;
        var maxY = appWindow.ActualHeight;

        if (newX < 0 || newY < 0 ||
            newX + node.Width > maxX ||
            newY + node.Height > maxY)
        {
            return false;
        }

        if (IsOverlapping(node, newX, newY))
            return false;

        node.X = newX;
        node.Y = newY;
        return true;
    }

    private bool IsOverlapping(NodeViewModel nodeToCheck, double newX, double newY)
    {
        foreach (var node in Nodes)
        {
            if (node == nodeToCheck)
                continue;

            var overlapX = newX < node.X + node.Width && newX + nodeToCheck.Width > node.X;
            var overlapY = newY < node.Y + node.Height && newY + nodeToCheck.Height > node.Y;

            if (overlapX && overlapY)
                return true;
        }
        return false;
    }

    private void UpdateTempBezier()
    {
        var dx = TempEnd.X - TempStart.X;
        var offset = Math.Abs(dx) * 0.1;

        TempControl1 = new Point(
            TempStart.X + (dx >= 0 ? offset : -offset),
            TempStart.Y);

        TempControl2 = new Point(
            TempEnd.X - (dx >= 0 ? offset : -offset),
            TempEnd.Y);
    }

    private void ResetDrag()
    {
        _dragStart = null;
        IsDragging = false;
        TempStart = TempEnd = new Point();
        UpdateTempBezier();
        OnPropertyChanged(nameof(TempLine));
    }
}
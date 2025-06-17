using Nodify.Helpers;
using Nodify.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Nodify.ViewModels;

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
            var p1 = new Point(_dragStart.X, _dragStart.Y);
            return [p1, new Point(_mousePos.X, p1.Y), _mousePos];
        }
    }

    public NodeSpaceViewModel()
    {
        Nodes.Add(new NodeViewModel("A", 50, 100));
        Nodes.Add(new NodeViewModel("B", 250, 150));
        Nodes.Add(new NodeViewModel("C", 450, 250));
        foreach (var n in Nodes) Subscribe(n);
        AddNodeCmd = new RelayCommand(p =>
        {
            if(p is not Point point) return;
            var nm = $"N{Nodes.Count + 1}";
            var node = new NodeViewModel(nm, point.X - 40, point.Y - 25);
            Nodes.Add(node);
            Subscribe(node);
        });
        Nodes.CollectionChanged += (_, _) =>
        {
            for (var i = 0; i < Nodes.Count; i++) Subscribe(Nodes[i]);
        };
    }

    public void BeginDrag(ConnectorViewModel c) => _dragStart = c;

    public void UpdateDrag(Point p)
    {
        _mousePos = p;
        OnPropertyChanged(nameof(TempLine));
    }

    public void EndDrag(ConnectorViewModel c)
    {
        if (_dragStart != null && c != null && _dragStart != null)
        {
            var connector = new ConnectionViewModel(_dragStart, c);
            Connections.Add(connector);
        }

        _dragStart = null;
        OnPropertyChanged(nameof(TempLine));
    }

    public void RebuildAll()
    {
        for (var i = 0; i < Connections.Count; i++)
        {
            var connection = Connections[i];
            connection.BuildRoute();
            connection.BuildArrow();
        }
    }

    private void Subscribe(NodeViewModel node)
    {
        node.Connectors.CollectionChanged += (_, _) => RebuildAll();
        node.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is "X" or "Y") RebuildAll();
        };
    }
}


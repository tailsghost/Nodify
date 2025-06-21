using System.Collections.ObjectModel;
using System.Windows;
using Nodify.ViewModels.Base;

namespace Nodify.ViewModels;

public class NodeContainerViewModel : BaseViewModel
{
    private Dictionary<NodeViewModel, Point> _initialNodePositions = new();
    public string Name { get; set; } = "Group_" + Guid.NewGuid().ToString("N").Substring(0, 5);

    private double _x;

    public double X
    {
        get => _x;
        set
        {
            _x=value;
            OnPropertyChanged();
        }
    }


    private double _y;

    public double Y
    {
        get => _y;
        set
        {
            _y = value;
            OnPropertyChanged();
        }
    }
    public double Width { get; set; }
    public double Height { get; set; }

    public ObservableCollection<NodeViewModel> Nodes { get; } = [];

    public void CacheNodePositions()
    {
        _initialNodePositions.Clear();
        foreach (var node in Nodes)
        {
            _initialNodePositions[node] = new Point(node.X, node.Y);
        }
    }

    public void NodesRaiseChanged(double dragContStartX, double deltaX, double dragContStartY, double deltaY)
    {
        X = dragContStartX + deltaX;
        Y = dragContStartY + deltaY;
        for (var i = 0; i < Nodes.Count; i++)
        {
            var node = Nodes[i];
            if (!_initialNodePositions.TryGetValue(node, out var start)) continue;
            node.X = start.X + deltaX;
            node.Y = start.Y + deltaY;
        }
    }
}


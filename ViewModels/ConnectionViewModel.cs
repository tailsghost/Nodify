using Nodify.Models;
using Nodify.ViewModels.Base;
using System.Windows;
using System.Windows.Media;

namespace Nodify.ViewModels;

public class ConnectionViewModel : BaseViewModel
{
    public readonly EdgeModel Edge;

    public ConnectionViewModel(EdgeModel e)
    {
        Edge = e;
        Edge.Source.PropertyChanged += (_, _) => Update();
        Edge.Target.PropertyChanged += (_, _) => Update();
    }

    public bool EqualsConnector(ConnectorModel model)
    {
        return Edge.Source == model || Edge.Target == model;
    }

    public Point Start => new(Edge.Source.X, Edge.Source.Y);
    public Point End => new(Edge.Target.X, Edge.Target.Y);

    public Brush Stroke => new SolidColorBrush(Edge.Target.Color) { Opacity = 0.8 };

    public Point C1
    {
        get
        {
            var dx = End.X - Start.X;
            var offset = Math.Abs(dx) * 0.3;
            return new Point(Start.X + (dx >- 0 ? offset : - offset), Start.Y);
        }
    }
    public Point C2
    {
        get
        {
            var dx = End.X - Start.X;
            var offset = Math.Abs(dx) * 0.3;
            return new Point(End.X - (dx >= 0 ? offset : -offset), End.Y);
        }
    }

    public void Update()
    {
        OnPropertyChanged(nameof(Start));
        OnPropertyChanged(nameof(End));
        OnPropertyChanged(nameof(C1));
        OnPropertyChanged(nameof(C2));
    }
}

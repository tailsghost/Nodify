using Nodify.Models;
using Nodify.ViewModels.Base;
using System.Net;
using System.Windows;
using System.Windows.Media;

namespace Nodify.ViewModels;

public class ConnectionViewModel : BaseViewModel
{
    private readonly EdgeModel _edge;

    public ConnectionViewModel(EdgeModel e)
    {
        _edge = e;
        _edge.Source.PropertyChanged += (_, _) => Update();
        _edge.Target.PropertyChanged += (_, _) => Update();
    }

    public Point Start => new(_edge.Source.Parent.X, _edge.Source.Parent.Y);
    public Point End => new(_edge.Target.Parent.X, _edge.Target.Parent.Y);

    public Brush Stroke => new SolidColorBrush(_edge.Target.Color) { Opacity = 0.8 };

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

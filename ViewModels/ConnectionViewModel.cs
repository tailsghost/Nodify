using Nodify.ViewModels.Base;
using System.Windows;
using System.Windows.Media;

namespace Nodify.ViewModels;

public class ConnectionViewModel : BaseViewModel
{
    public ConnectorViewModel Source { get; }
    public ConnectorViewModel Target { get; }

    public PointCollection Points { get; private set; }
    public Point StartPoint => new(Source.X, Source.Y);
    public Point EndPoint => new(Target.X, Target.Y);


    public SolidColorBrush StrokeBrush
    {
        get
        {
            var b = new SolidColorBrush(Target.Color);
            b.Opacity = 0.8;
            return b;
        }
    }

    public Point ControlPoint1
    {
        get
        {
            var dx = EndPoint.X - StartPoint.X;
            var offset = Math.Abs(dx) * 0.3;
            return new(StartPoint.X + (dx >= 0 ? offset : -offset), StartPoint.Y);
        }
    }

    public Point ControlPoint2
    {
        get
        {
            var dx = EndPoint.X - StartPoint.X;
            var offset = Math.Abs(dx) * 0.3;
            return new(EndPoint.X - (dx >= 0 ? offset : -offset), EndPoint.Y);
        }
    }

    public ConnectionViewModel(ConnectorViewModel source, ConnectorViewModel target)
    {
        Source = source;
        Target = target;
        Source.Color = Target.Color;
        Source.PropertyChanged += (_, _) => Update();
        Target.PropertyChanged += (_, _) => Update();
        Update();
    }
    public void Update()
    {
        OnPropertyChanged(nameof(StartPoint));
        OnPropertyChanged(nameof(EndPoint));
        var mid = new Point(EndPoint.X, StartPoint.Y);
        Points = new PointCollection { StartPoint, mid, EndPoint };
        OnPropertyChanged(nameof(Points));
        OnPropertyChanged(nameof(ControlPoint1));
        OnPropertyChanged(nameof(ControlPoint2));
    }
}


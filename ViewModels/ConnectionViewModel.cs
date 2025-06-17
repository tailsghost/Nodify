using Nodify.ViewModels.Base;
using System.Windows;
using System.Windows.Media;

namespace Nodify.ViewModels;

public class ConnectionViewModel : BaseViewModel
{
    public ConnectorViewModel Sourse { get; }
    public ConnectorViewModel Target { get; }

    private PointCollection _points;
    public PointCollection Points
    {
        get => _points;
        set
        {
            _points = value;
            OnPropertyChanged(nameof(Points));
        }
    }

    private PointCollection _arrow;

    public PointCollection ArrowPoints
    {
        get => _arrow;
        set
        {
            _arrow = value;
            OnPropertyChanged(nameof(ArrowPoints));
        }
    }

    public ConnectionViewModel(ConnectorViewModel a, ConnectorViewModel b)
    {
        Sourse = a;
        Target = b;
        BuildRoute();
        BuildArrow();
    }

    public void BuildRoute()
    {
        var p1 = new Point(Sourse.X, Sourse.Y);
        var p2 = new Point(Target.X, Target.Y);
        Points = [p1, new Point(p2.X, p1.Y), p2];
    }
    public void BuildArrow()
    {
        if (Points.Count < 2) return;
        var end = Points[^1];
        var prev = Points[^2];
        var dir = end - prev;
        if(dir.Length < 0.1)return;
        dir.Normalize();
        double L = 12, W = 6;
        var basePt = end - dir * L;
        var perp = new Vector(-dir.Y, dir.X) * (W / 2);
        ArrowPoints = [end, basePt + perp, basePt - perp];
    }
}


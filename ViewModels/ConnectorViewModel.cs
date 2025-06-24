using Nodify.Models;
using Nodify.ViewModels.Base;
using System.Windows.Media;

namespace Nodify.ViewModels;

public class ConnectorViewModel : BaseViewModel
{
    private Color _color;
    private const double Margin = 5;
    private ConnectionViewModel _connection;

    public ConnectorViewModel(ConnectorModel model)
    {
        Color = RandomColor();
        Model = model;
    }

    public ConnectorModel Model { get; }

    public bool IsInput => Model.IsInput;
    public int Index => Model.Index;

    public double ConnectorSize { get; init; } = 12;

    public Color Color
    {
        get => _color;
        set
        {
            if (_color == value) return;
            _color = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Brush));
            OnPropertyChanged(nameof(LineBrush));
        }
    }
    public SolidColorBrush Brush => new(Color);
    public SolidColorBrush LineBrush => new SolidColorBrush(Color) { Opacity = 0.8 };

    public bool IsConnected => Connection != null;

    public ConnectionViewModel Connection
    {
        get => _connection;
        set
        {
            if (_connection == value) return;
            _connection = value;
            OnPropertyChanged(); OnPropertyChanged(nameof(IsConnected));
        }
    }

    public string Name => Model.Name;

    private static Color RandomColor()
    {
        var rnd = new Random();
        return Color.FromRgb(
            (byte)rnd.Next(128, 256),
            (byte)rnd.Next(128, 256),
            (byte)rnd.Next(128, 256)
        );
    }
}

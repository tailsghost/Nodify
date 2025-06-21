using Nodify.ViewModels.Base;
using System.Windows.Media;

namespace Nodify.ViewModels;

public class ConnectorViewModel : BaseViewModel
{
    private static readonly Random _random = new();
    private const double Margin = 5;

    public NodeViewModel Parent { get; }

    public int Index { get; set; }
    public bool IsInput { get; }
    public bool IsConnected => Connection != null;

    private ConnectionViewModel _connection;
    public ConnectionViewModel Connection
    {
        get => _connection;
        set
        {
            if (_connection == value) return;
            _connection = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsConnected));
        }
    }

    private double _connectorSize;

    public double ConnectorSize
    {
        get => _connectorSize;
        set
        {
            _connectorSize = value;
            OnPropertyChanged();
        }
    }

    private Color _color = Colors.LightBlue; 
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

    public SolidColorBrush Brush => new SolidColorBrush(Color);
    public SolidColorBrush LineBrush
    {
        get
        {
            var b = new SolidColorBrush(Color);
            b.Opacity = 0.9;
            return b;
        }
    }

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            OnPropertyChanged();
        }
    }

    public ConnectorViewModel(NodeViewModel parent, int index, double size, bool isInput, string name)
    {
        Name = name;
        Color = Color = RandomColor();
        ConnectorSize = size;
        Parent = parent;
        Index = index;
        IsInput = isInput;
    }
    public double X =>
        IsInput
            ? Parent.X + 25 - ConnectorSize
            : Parent.X + Parent.Width - 15;

    public double Y
    {
        get
        {
            var list = IsInput ? Parent.Inputs : Parent.Outputs;
            var count = list.Count;

            if (count == 1)
                return Parent.Y + Parent.Height / 2;

            var available = Parent.Height - 2 * Margin - ConnectorSize;

            var step = available / (count - 1);

            return Parent.Y
                   + Margin
                   + ConnectorSize / 2
                   + step * Index;
        }
    }

    public void RaiseChanged()
    {
        OnPropertyChanged(nameof(X));
        OnPropertyChanged(nameof(Y));
    }


    private static Color RandomColor()
    {
        var r = (byte)_random.Next(128, 256);
        var g = (byte)_random.Next(128, 256);
        var b = (byte)_random.Next(128, 256);

        var maxIndex = _random.Next(3);
        switch (maxIndex)
        {
            case 0: r = 255; break;
            case 1: g = 255; break;
            case 2: b = 255; break;
        }

        return Color.FromRgb(r, g, b);
    }
}


using System.Windows.Media;
using Nodify.Interfaces;
using Nodify.ViewModels.Base;

namespace Nodify.Models;

public class ConnectorModel : BaseViewModel
{
    private double _x;
    private double _y;


    public IConnectable Parent { get; }
    public bool IsInput { get; }
    public string Name { get; }

    public Color Color { get; }
    public double ConnectorSize { get; }
    public int Index { get; }

    private ConnectorModel? _connectedTo;
    public ConnectorModel? ConnectedTo
    {
        get => _connectedTo;
        set
        {
            if (_connectedTo == value) return;
            _connectedTo = value;
            OnPropertyChanged();
        }
    }

    public double X
    {
        get => _x;
        set
        {
            if (_x == value) return;
            _x = value;
            OnPropertyChanged();
        }
    }
    public double Y
    {
        get => _y;
        set
        {
            if (_y == value) return;
            _y = value;
            OnPropertyChanged();
        }
    }

    public ConnectorModel(IConnectable parent, int index, double size, bool isInput, string name)
    {
        Parent = parent;
        IsInput = isInput;
        Index = index;
        ConnectorSize = size;
        Name = name;
        Color = Color.FromRgb((byte)Random.Shared.Next(128, 256), (byte)Random.Shared.Next(128, 256), (byte)Random.Shared.Next(128, 256));
    }
}

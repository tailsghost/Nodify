using System.Windows.Media;
using Nodify.Interfaces;
using Nodify.ViewModels.Base;

namespace Nodify.Models;

public class ConnectorModel : BaseViewModel
{
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
            if (_connectedTo != value)
            {
                _connectedTo = value;
                OnPropertyChanged();
            }
        }
    }

    public void UpdateProperty()
    {
        OnPropertyChanged();
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

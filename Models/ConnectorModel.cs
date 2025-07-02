using Nodify.Interfaces;
using Nodify.ViewModels.Base;
using System.Windows.Media;

namespace Nodify.Models;

public class ConnectorModel : BaseViewModel
{
    private double _x;
    private double _y;

    public Guid Id { get; init; }

    public bool IsFinalBlock;

    public IConnectorInfo ConnectorInfo { get; }

    public NodeModel Node { get; }
    public bool IsInput { get; }

    public string Name => ConnectorInfo.Name;

    public string AltName { get; set; }

    public string Type => ConnectorInfo.AllowedType.Type;

    public Color Color => ConnectorInfo.Color;
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

    public ConnectorModel(NodeModel parent, int index, double size, bool isInput, IConnectorInfo connectorInfo, bool isFinalBlock = false)
    {
        Node = parent;
        IsInput = isInput;
        Index = index;
        ConnectorSize = size;
        ConnectorInfo = connectorInfo;
        IsFinalBlock = isFinalBlock;
        Id = connectorInfo.Id == null || connectorInfo.Id == Guid.Empty ? Guid.NewGuid() : connectorInfo.Id;
    }



    //public virtual string SpecialMethod()
    //{
    //    return $""
    //};

}

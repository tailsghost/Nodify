using Nodify.Interfaces;
using Nodify.Models;
using Nodify.ViewModels.Base;
using System.Windows.Media;

namespace Nodify.ViewModels;

public class ConnectorViewModel : BaseViewModel
{
    private Color _color;

    public ConnectorViewModel(ConnectorModel model)
    {
        Color = model.Color;
        Model = model;
    }

    public ConnectorModel Model { get; }

    public string Name => Model.Name;
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

    public bool IsConnected => Model.ConnectedTo != null;
    public ConnectorModel ConnectedTo => Model.ConnectedTo;
    public  IConnectable Parent => Model.Node;

    public string Type => Model.ConnectorInfo.AllowedType.Type;

    public bool AllowConnect(ConnectorModel model)
    {

        if (Model.IsFinalBlock || model.IsFinalBlock)
        {
            return Model.ConnectorInfo.AllowedType.Type == model.ConnectorInfo.AllowedType.Type;
        }

        return !(this.IsConnected && model.ConnectedTo != null) && this.IsInput != model.IsInput &&
                     this.Model.ConnectorInfo.AllowedType.Type == model.ConnectorInfo.AllowedType.Type;
    }
}

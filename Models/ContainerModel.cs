using Nodify.Interfaces;
using Nodify.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Nodify.Models;

public class ContainerModel : BaseViewModel, IConnectable
{
    private double _x;
    private double _y;
    private double _width;
    private double _height;

    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }

    public double X
    {
        get => _x;
        set
        {
            if(_x == value) return;
            _x = value;
            OnPropertyChanged();
        }
    }
    public double Y
    {
        get => _y;
        set
        {
            if(_y == value) return;
            _y = value;
            OnPropertyChanged();
        }
    }
    public double Width
    {
        get => _width;
        set
        {
            if(_width == value) return;
            _width = value;
            OnPropertyChanged();
        }
    }
    public double Height
    {
        get => _height;
        set
        {
            if(_height == value) return;
            _height = value;
            OnPropertyChanged();
        }
    }

    private ConnectorModel Input { get; }
    private ConnectorModel Output { get; }

    public ConnectorModel InnerInput { get; }
    public ConnectorModel InnerOutput { get; }

    public ObservableCollection<ConnectorModel> AllInputs => [Input, InnerInput];
    public ObservableCollection<ConnectorModel> AllOutputs => [Output, InnerOutput];

    public ObservableCollection<ConnectorModel> Inputs => [Input];
    public ObservableCollection<ConnectorModel> Outputs => [Output];

    public ObservableCollection<NodeModel> Nodes { get; } = [];

    public ContainerModel(double x, double y, double w, double h)
    {
        Name = "Group_" + Id.ToString("N").Substring(0, 5);
        X = x;
        Y = y;
        Width = w;
        Height = h;
        Input = new ConnectorModel(this, 0, 12, true, new ConnectorInfo() {AllowedType = new AllowedType() {Type = "ANY"}, Color = Colors.White, Description = "", Name = ""});
        Output = new ConnectorModel(this, 0, 12, false, new ConnectorInfo() { AllowedType = new AllowedType() { Type = "ANY" }, Color = Colors.White, Description = "", Name = "" });
        InnerInput = new ConnectorModel(this, 1, 12, true, new ConnectorInfo() { AllowedType = new AllowedType() { Type = "ANY" }, Color = Colors.White, Description = "", Name = "" });
        InnerOutput = new ConnectorModel(this, 1, 12, false, new ConnectorInfo() { AllowedType = new AllowedType() { Type = "ANY" }, Color = Colors.White, Description = "", Name = "" });
    }
}


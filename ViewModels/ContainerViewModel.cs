using Nodify.Models;
using Nodify.ViewModels.Base;

namespace Nodify.ViewModels;

public class ContainerViewModel : BaseViewModel
{
    private readonly ContainerModel _m;
    public ContainerViewModel(ContainerModel m) => _m = m;

    public string Name => _m.Name;
    public double X { get => _m.X; set { _m.X = value; OnPropertyChanged(); } }
    public double Y { get => _m.Y; set { _m.Y = value; OnPropertyChanged(); } }
    public double Width => _m.Width;
    public double Height => _m.Height;

    public ConnectorViewModel Input => new(_m.Inputs.First());
    public ConnectorViewModel Output => new(_m.Outputs.First());

    public ConnectorModel InnerInput { get; }
    public ConnectorModel InnerOutput { get; }
}


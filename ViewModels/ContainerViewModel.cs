using Nodify.Models;
using Nodify.ViewModels.Base;

namespace Nodify.ViewModels;

public class ContainerViewModel : BaseViewModel
{
    public readonly ContainerModel Model;
    public ContainerViewModel(ContainerModel m) => Model = m;

    public string Name => Model.Name;
    public double X
    {
        get => Model.X; 
        set
        {
            if (Model.X == value) return;
            Model.X = value;
            OnPropertyChanged();
        }
    }
    public double Y
    {
        get => Model.Y; 
        set
        {
            if (Model.Y == value) return;
            Model.Y = value;
            OnPropertyChanged();
        }
    }
    public double Width => Model.Width;
    public double Height => Model.Height;

    public ConnectorViewModel Input => new(Model.Inputs.First());
    public ConnectorViewModel Output => new(Model.Outputs.First());

    public ConnectorModel InnerInput { get; }
    public ConnectorModel InnerOutput { get; }
}


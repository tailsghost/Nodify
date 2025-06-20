using System.Collections.ObjectModel;
using Nodify.ViewModels.Base;

namespace Nodify.ViewModels;

public class NodeContainerViewModel : BaseViewModel
{
    public string Name { get; set; } = "Group_" + Guid.NewGuid().ToString("N").Substring(0, 5);

    private double _x;

    public double X
    {
        get => _x;
        set
        {
            _x=value;
            OnPropertyChanged();
        }
    }


    private double _y;

    public double Y
    {
        get => _y;
        set
        {
            _y = value;
            OnPropertyChanged();
        }
    }
    public double Width { get; set; }
    public double Height { get; set; }

    public ObservableCollection<NodeViewModel> Nodes { get; } = [];
}


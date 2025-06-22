using System.Collections.ObjectModel;
using Nodify.Interfaces;
using Nodify.ViewModels.Base;

namespace Nodify.Models;

public class NodeModel : BaseViewModel, IConnectable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }

    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; }
    public double Height { get; }
    public ObservableCollection<ConnectorModel> Inputs { get; } = [];
    public ObservableCollection<ConnectorModel> Outputs { get; } = [];

    public NodeModel(string name, double x, double y)
    {
        Name = name;
        X = x;
        X = y;
        Inputs.Add(new ConnectorModel(this,0,12, true, "In1"));
        Inputs.Add(new ConnectorModel(this, 1, 12, true, "In2"));
        Outputs.Add(new ConnectorModel(this, 0, 12, false, "Out1"));
        Outputs.Add(new ConnectorModel(this, 1, 12, false, "Out2"));
    }
}


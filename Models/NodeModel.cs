using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Nodify.Interfaces;
using Nodify.ViewModels.Base;

namespace Nodify.Models;

public class NodeModel : BaseViewModel, IConnectable
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; }
    public string Description { get; init; }

    public List<IConnectorInfo> InputsInfo;
    public List<IConnectorInfo> OutputsInfo;

    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; }
    public double Height { get; }
    public ObservableCollection<ConnectorModel> Inputs { get; } = [];
    public ObservableCollection<ConnectorModel> Outputs { get; } = [];

    public bool IsFinalBlock { get; init; }

    public NodeModel(string name, string description, List<IConnectorInfo> inputs, List<IConnectorInfo> outputs, bool isFinalBlock = false)
    {
        Name = name;
        Description = description;
        InputsInfo = inputs;
        OutputsInfo = outputs;

        for (var i = 0; i < inputs.Count; i++)
        {
            var input = inputs[i];
            Inputs.Add(new ConnectorModel(this, i, 12, true, input, isFinalBlock));
        }

        for (var i = 0; i < outputs.Count; i++)
        {
            var output = outputs[i];
            Outputs.Add(new ConnectorModel(this, i, 12, false, output, isFinalBlock));
        }

        Height = Inputs.Count >= Outputs.Count
            ? CalculationHeight(Inputs.Count, 12, 12) + 30
            : CalculationHeight(Outputs.Count, 12, 12) + 30;

        IsFinalBlock = isFinalBlock;
    }

    private double CalculationHeight(int count, int size, int margin)
    {
        return (size * count) + (margin * count);
    }
}


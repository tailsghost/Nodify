using System.Collections.ObjectModel;
using Nodify.Interfaces;
using Nodify.ViewModels.Base;

namespace Nodify.Models;

public class NodeModel : BaseViewModel, IConnectable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Description { get; set; }

    public List<string> InputsName;
    public List<string> OutputsName;

    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; }
    public double Height { get; }
    public ObservableCollection<ConnectorModel> Inputs { get; } = [];
    public ObservableCollection<ConnectorModel> Outputs { get; } = [];

    public NodeModel(string name, string description, List<string> inputsName, List<string> outputsName)
    {
        Name = name;
        Description = description;
        InputsName = inputsName;
        OutputsName = outputsName;

        for (var i = 0; i < inputsName.Count; i++)
        {
            var input = inputsName[i];
            Inputs.Add(new ConnectorModel(this, i, 12, true, input));
        }

        for (var i = 0; i < outputsName.Count; i++)
        {
            var output = outputsName[i];
            Outputs.Add(new ConnectorModel(this,i,12,false,output));
        }

        Width = 200;
        Height = Inputs.Count >= Outputs.Count
            ? CalculationHeight(Inputs.Count, 12, 12) + 30
            : CalculationHeight(Outputs.Count, 12, 12) + 30;
    }


    private double CalculationHeight(int count, int size, int margin)
    {
        return (size * count) + (margin * count);
    }
}


using System.Collections.ObjectModel;
using System.Windows.Media;
using Nodify.Interfaces;
using Nodify.ViewModels;
using Nodify.ViewModels.Base;

namespace Nodify.Models;

public class NodeModel : BaseViewModel
{
    private bool _isMenuEnable = true;
    private bool _isUnique { get; }

    public bool IsMenuEnable
    {
        get => !_isUnique || _isMenuEnable;
        set => _isMenuEnable = value;
    }

    public Brush ColorText { get; set; } = Brushes.AliceBlue;

    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; init; }
    public string AltName { get; init; }
    public string Description { get; init; }
    public List<IConnectorInfo> InputsInfo;
    public List<IConnectorInfo> OutputsInfo;
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; }
    public double Height { get; private set; }

    public ObservableCollection<ConnectorModel> Inputs { get; set; } = [];
    public ObservableCollection<ConnectorModel> Outputs { get; set; } = [];

    public NodeViewModel Parent { get; set; }

    public NodeModel(string name, string altName, string description, List<IConnectorInfo> inputs, List<IConnectorInfo> outputs, bool isUnique = false)
    {
        _isUnique = isUnique;
        Name = name;
        AltName = altName;
        Description = description;
        InputsInfo = inputs;
        OutputsInfo = outputs;

        for (var i = 0; i < inputs.Count; i++)
        {
            var input = inputs[i];
            var connector = new ConnectorModel(this, i, 12, true, input);
            Inputs.Add(connector);
        }

        for (var i = 0; i < outputs.Count; i++)
        {
            var output = outputs[i];
            var connector = new ConnectorModel(this, i, 12, false, output);
            Outputs.Add(connector);
        }

        Height = Inputs.Count >= Outputs.Count
            ? CalculationHeight(Inputs.Count, 12, 12) + 30
            : CalculationHeight(Outputs.Count, 12, 12) + 30;
    }

    private double CalculationHeight(int count, int size, int margin)
    {
        return (size * count) + (margin * count);
    }
}


using System.Collections.ObjectModel;
using Nodify.Models;
using Nodify.ViewModels.Base;

namespace Nodify.ViewModels;

public class NodeViewModel : BaseViewModel
{
    public Guid Id => Node.Id;
    public string Name => Node.Name;

    public NodeModel Node { get; }

    public double X
    {
        get => Node.X;
        set
        {
            Node.X = value;
            OnPropertyChanged();
            RaiseConnectorPositionsChanged();
        }
    }
    public double Y
    {
        get => Node.Y;
        set
        {
            Node.Y = value;
            OnPropertyChanged();
            RaiseConnectorPositionsChanged();
        }
    }

    public double Width { get; init; }
    public double Height { get; init; }
    public ObservableCollection<ConnectorViewModel> Inputs
    {
        get
        {
            var list = new ObservableCollection<ConnectorViewModel>();
            for (var i = 0; i < Node.Inputs.Count; i++)
            {
                var cm = Node.Inputs[i];
                list.Add(new ConnectorViewModel(cm));
            }
            return list;
        }
    }

    public ObservableCollection<ConnectorViewModel> Outputs
    {
        get
        {
            var list = new ObservableCollection<ConnectorViewModel>();
            for (var i = 0; i < Node.Outputs.Count; i++)
            {
                var cm = Node.Outputs[i];
                list.Add(new ConnectorViewModel(cm));
            }
            return list;
        }
    }

    public NodeViewModel(NodeModel m, double width, double height)
    {
        Node = m;
        Width = width;
        Height = height;
    }

    private void RaiseConnectorPositionsChanged()
    {
        foreach (var input in Inputs)
            input.RaiseChanged();
        foreach (var output in Outputs)
            output.RaiseChanged();
    }
}

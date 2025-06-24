using System.Collections.ObjectModel;
using Nodify.Models;
using Nodify.ViewModels.Base;

namespace Nodify.ViewModels;

public class NodeViewModel : BaseViewModel
{
    public Guid Id => Node.Id;
    public string Name => Node.Name;
    public string Description => Node.Description;

    public NodeModel Node { get; }

    public double X
    {
        get => Node.X;
        set
        {
            if(Node.X == value) return;
            Node.X = value;
            OnPropertyChanged();
        }
    }
    public double Y
    {
        get => Node.Y;
        set
        {
            if(Node.Y == value) return;
            Node.Y = value;
            OnPropertyChanged();
        }
    }

    public double Width => Node.Width;
    public double Height => Node.Height;
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

    public NodeViewModel(NodeModel m)
    {
        Node = m;
    }
}

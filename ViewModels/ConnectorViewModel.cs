using Nodify.ViewModels.Base;

namespace Nodify.ViewModels;

public class ConnectorViewModel : BaseViewModel
{
    private const double ConnectorSize = 10;
    private const double ConnectorRowHeight = 20;
    public NodeViewModel Parent { get; }
    public int Index { get; set; }

    public ConnectorViewModel(NodeViewModel parent, int idx)
    {
        Parent = parent; 
        Index = idx;
    }

    public double X
        => Parent.X
           + (Parent.Width / (Index + 1) / (Parent.Connectors.Count))
           - ConnectorSize * 1.5;

    public double Y
        => Parent.Y
           + Parent.Height  
           + ConnectorRowHeight / 2
           - ConnectorSize / 2;

    public void RaiseChanged()
    {
        OnPropertyChanged(nameof(X)); 
        OnPropertyChanged(nameof(Y));
    }
}


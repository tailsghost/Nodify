using Nodify.Models;
using System.Collections.ObjectModel;

namespace Nodify.Interfaces;

public interface IConnectable
{
    public Guid Id { get; }
    public string Name { get; }
    double X { get; }
    double Y { get; }
    double Width { get; }
    double Height { get; }

    ObservableCollection<ConnectorModel> Inputs { get; }
    ObservableCollection<ConnectorModel> Outputs { get; }
}


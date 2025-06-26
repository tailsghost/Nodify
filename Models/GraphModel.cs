namespace Nodify.Models;

public class GraphModel
{
    public Dictionary<Guid, NodeModel> Nodes { get; } = [];
    public List<ContainerModel> Containers { get; } = [];
    public List<EdgeModel> Edges { get; } = [];

    public NodeModel AddNode(NodeModel node, double x, double y)
    {
        Nodes[node.Id] = node;
        node.X = x;
        node.Y = y;
        return node;
    }

    public ContainerModel AddContainer(double x, double y, double w, double h)
    {
        var c = new ContainerModel(x, y, w, h);
        Containers.Add(c);
        return c;
    }

    public EdgeModel AddEdge(ConnectorModel src, ConnectorModel tgt)
    {
        var e = new EdgeModel(src, tgt);
        Edges.Add(e);
        src.ConnectedTo = tgt;
        tgt.ConnectedTo = src;
        return e;
    }
}

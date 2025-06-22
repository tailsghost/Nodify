namespace Nodify.Models;

public class EdgeModel
{
    public ConnectorModel Source { get; }
    public ConnectorModel Target { get; }

    public EdgeModel(ConnectorModel src, ConnectorModel tgt)
    {
        Source = src; Target = tgt;
    }
}


namespace Nodify.Models;

public class EdgeModel
{
    public ConnectorModel Source { get; private set; }
    public ConnectorModel Target { get; private set; }

    public EdgeModel(ConnectorModel src, ConnectorModel tgt)
    {
        Source = src; Target = tgt;
    }
}


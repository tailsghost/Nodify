using System.Windows.Media;
using Nodify.Interfaces;

namespace Nodify.Models;

public class ConnectorInfo : IConnectorInfo
{
    public string Name { get; init; }
    public IAllowedType AllowedType { get; init; }
    public string Description { get; init; }
    public Color Color { get; init; }
}

public class AllowedType : IAllowedType
{
    public string Type { get; init; }
}

using System.Windows.Media;

namespace Nodify.Interfaces;

public interface IConnectorInfo
{
    string Name { get; init; }
    IAllowedType AllowedType { get; }
    string Description { get; init; }
    public Color Color { get; }
}


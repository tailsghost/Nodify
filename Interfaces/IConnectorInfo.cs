using System.Windows.Media;

namespace Nodify.Interfaces;

public interface IConnectorInfo
{
    Guid Id { get; init; }
    string Name { get; init; }
    IAllowedType AllowedType { get; }
    string Description { get; init; }
    public Color Color { get; }
}


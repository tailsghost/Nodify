using System.Windows.Media;

namespace Nodify.Interfaces;

public interface IConnectorInfo
{
    Guid Id { get; init; }
    string Name { get; init; }
    string AltName { get; }
    IAllowedType AllowedType { get; }
    string Description { get; init; }
    public Color Color { get; }
}


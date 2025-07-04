﻿using System.Windows.Media;
using Nodify.Interfaces;

namespace Nodify.Models;

public class ConnectorInfo : IConnectorInfo
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string AltName { get; init; }
    public IAllowedType AllowedType { get; init; }
    public string Description { get; init; }
    public Color Color { get; init; }
    public string[] TMP_VAR { get; init; }
    public string SetFunc { get; set; }
    public bool IsTemp { get; set; }
}

public class AllowedType : IAllowedType
{
    public string Type { get; init; }
}

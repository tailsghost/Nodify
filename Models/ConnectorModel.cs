﻿using System.Collections.ObjectModel;
using Nodify.Interfaces;
using Nodify.ViewModels.Base;
using System.Windows.Media;

namespace Nodify.Models;

public class ConnectorModel : BaseViewModel
{
    private double _x;
    private double _y;

    public Guid Id { get; init; }
    public string SetFunc => ConnectorInfo.SetFunc;
    public IConnectorInfo ConnectorInfo { get; }

    public NodeModel Node { get; }
    public bool IsInput { get; }

    public bool IsTemp => ConnectorInfo.IsTemp;

    public string Name => ConnectorInfo.Name;

    public string AltName => ConnectorInfo.AltName;

    public string Type => ConnectorInfo.AllowedType.Type;

    public Color Color => ConnectorInfo.Color;
    public double ConnectorSize { get; }
    public int Index { get; }
    public ObservableCollection<ConnectorModel> ConnectedTo { get; } = [];
    public string TMP_VAR { get; set; }

    public double X
    {
        get => _x;
        set
        {
            if (_x == value) return;
            _x = value;
            OnPropertyChanged();
        }
    }
    public double Y
    {
        get => _y;
        set
        {
            if (_y == value) return;
            _y = value;
            OnPropertyChanged();
        }
    }

    public ConnectorModel(NodeModel parent, int index, double size, bool isInput, IConnectorInfo connectorInfo)
    {
        Node = parent;
        IsInput = isInput;
        Index = index;
        ConnectorSize = size;
        ConnectorInfo = connectorInfo;
        Id = connectorInfo.Id == null || connectorInfo.Id == Guid.Empty ? Guid.NewGuid() : connectorInfo.Id;
    }

}

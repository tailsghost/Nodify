﻿using System.Collections.ObjectModel;
using Nodify.Models;
using Nodify.ViewModels.Base;
using System.Windows.Media;

namespace Nodify.ViewModels;

public class ConnectorViewModel : BaseViewModel
{
    private Color _color;

    public ConnectorViewModel(ConnectorModel model)
    {
        Color = model.Color;
        Model = model;
    }

    public ConnectorModel Model { get; }
    public string Name => Model.Name;
    public string AltName => Model.AltName;
    public bool IsInput => Model.IsInput;
    public int Index => Model.Index;
    public bool IsTemp => Model.IsTemp;

    public double ConnectorSize { get; init; } = 12;

    public Color Color
    {
        get => _color;
        set
        {
            if (_color == value) return;
            _color = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Brush));
            OnPropertyChanged(nameof(LineBrush));
        }
    }
    public SolidColorBrush Brush => new(Color);
    public SolidColorBrush LineBrush => new SolidColorBrush(Color) { Opacity = 0.8 };

    public bool IsConnected => Model.ConnectedTo != null;
    public ObservableCollection<ConnectorModel> ConnectedTo => Model.ConnectedTo;

    public string SetFunc => Model.SetFunc;

    public virtual bool AllowConnect(ConnectorModel model)
    {
        return true;
    }
}

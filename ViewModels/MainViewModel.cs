﻿using Nodify.Helpers;
using Nodify.Models;
using Nodify.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Nodify.ViewModels;

public class MainViewModel : BaseViewModel
{
    private Point _tempEnd;
    private Point _tempControl1;
    private Point _tempControl2;
    private bool _isConnecting;
    private Point _tempStart;

    private ConnectorModel _dragFrom;

    public ObservableCollection<NodeViewModel> Nodes { get; protected set; }
    public ObservableCollection<ContainerViewModel> Containers { get; }
    public ObservableCollection<ConnectionViewModel> Connections { get; }
    public ObservableCollection<MenuLibrary> MenuLibrary { get; protected set; } = [];

    public ICommand AddNodeCmd { get; protected set; }
    public ICommand AddContainerCmd { get; }
    public ICommand BeginConnectCmd { get; }
    public ICommand CompleteConnectCmd { get; }

    public Point TempEndPoint
    {
        get => _tempEnd;
        set
        {
            if(_tempEnd == value)return;
            _tempEnd = value;
            OnPropertyChanged();
        }
    }
    public bool IsConnecting
    {
        get => _isConnecting;
        private set
        {
            if(_isConnecting == value) return;
            _isConnecting = value;
            OnPropertyChanged();
        }
    }
    public Point TempStart
    {
        get => _tempStart;
        private set
        {
            if(_tempStart == value) return;
            _tempStart = value;
            OnPropertyChanged();
        }
    }

    public Point TempControl1 { get => _tempControl1; private set { _tempControl1 = value; OnPropertyChanged();}  }
    public Point TempControl2 { get => _tempControl2; private set { _tempControl2 = value; OnPropertyChanged(); } }

    public MainViewModel()
    {
        Nodes = new ObservableCollection<NodeViewModel>();
        Containers = new ObservableCollection<ContainerViewModel>();
        Connections = new ObservableCollection<ConnectionViewModel>();

        AddNodeCmd = new RelayCommand(p =>
        {
            if(p is not (Point pt, NodeViewModel node)) return;

            var newNode = new NodeViewModel(new NodeModel(node.Name, node.Description,node.Node.InputsInfo, node.Node.OutputsInfo, node.IsFinalBlock));
            newNode.X = pt.X;
            newNode.Y = pt.Y;
            newNode.GenerateDesignator();
            Nodes.Add(newNode);
        });
        AddContainerCmd = new RelayCommand(p =>
        {
            if (p is not Rect r) return;
            var c = new ContainerModel(r.X, r.Y, r.Width, r.Height);
            Containers.Add(new ContainerViewModel(c));
        }, o =>
        {
            if (o is not Rect r) return false;
            return !Containers.Any(c => new Rect(c.X, c.Y, c.Width, c.Height).IntersectsWith(r));
        });
        BeginConnectCmd = new RelayCommand(p =>
        {
            if (p is not ConnectorModel cm) return;
            _dragFrom = cm;
            TempStart = new Point(_dragFrom.X, _dragFrom.Y);
            IsConnecting = true;
        });
        CompleteConnectCmd = new RelayCommand(p =>
        {
            if (IsConnecting && p is ConnectorViewModel to && _dragFrom != null && to.Model != _dragFrom)
            {
                var outputPort = _dragFrom.IsInput ? to.Model : _dragFrom;
                var inputPort = _dragFrom.IsInput ? _dragFrom : to.Model;

                var e = new EdgeModel(outputPort, inputPort);
                Connections.Add(new ConnectionViewModel(e));
            }
            IsConnecting = false;
        }, o =>
        {
            if (IsConnecting && o is ConnectorViewModel to && _dragFrom != null && to.Model != _dragFrom)
            {
                return to.AllowConnect(_dragFrom);
            }

            return false;
        });
    }

    public void BeginDrag(ConnectorViewModel vmConnector)
    {
        _dragFrom = vmConnector?.Model;
        IsConnecting = true;
        if(_dragFrom == null) return;
        TempStart = new Point(_dragFrom.X,
            _dragFrom.Y);
        TempEndPoint = TempStart;
        UpdateBezier();
    }

    public void UpdateDrag(Point mousePos)
    {
        if (!IsConnecting || _dragFrom == null) return;
        TempEndPoint = mousePos;
        UpdateBezier();
    }

    public void EndDrag(ConnectorViewModel vmConnector)
    {
        if (IsConnecting && vmConnector != null && _dragFrom != null && vmConnector.Model != _dragFrom)
        {
            CompleteConnectCmd.Execute(vmConnector);
        }

        IsConnecting = false;
        _dragFrom = null;
    }

    public bool TryMoveNode(NodeViewModel node, double maxX, double maxY, double newX, double newY)
    {
        var clX = Math.Max(0, Math.Min(newX, maxX - node.Width));
        var clY = Math.Max(0, Math.Min(newY, maxY - node.Height));
        node.X = clX; node.Y = clY;
        return true;
    }

    public void RemoveConnection(ConnectionViewModel vm)
    {
        var edge = vm.Edge;
        edge.Source.ConnectedTo = null;
        edge.Target.ConnectedTo = null;

        var removeConnection = new List<ConnectionViewModel>();
        for (var i = 0; i < Connections.Count; i++)
        {
            if (Connections[i] == vm)
            {
                removeConnection.Add(Connections[i]);
            }
        }

        for (var i = 0; i < removeConnection.Count; i++)
        {
            Connections.Remove(removeConnection[i]);
        }
    }

    public void RemoveNode(NodeViewModel vm)
    {
        Nodes.Remove(vm);
        vm.ReleaseDesignator();

        for (var ci = 0; ci < Containers.Count; ci++)
        {
            var containerVm = Containers[ci];
            if (containerVm.Model.Nodes.Contains(vm.Node))
            {
                containerVm.Model.Nodes.Remove(vm.Node);
            }
        }

        var allConnectors = new List<ConnectorModel>();
        for (var i = 0; i < vm.Inputs.Count; i++)
        {
            allConnectors.Add(vm.Inputs[i].Model);
        }
        for (var i = 0; i < vm.Outputs.Count; i++)
        {
            allConnectors.Add(vm.Outputs[i].Model);
        }

        var connsToRemove = new List<ConnectionViewModel>();
        for (var ci3 = 0; ci3 < Connections.Count; ci3++)
        {
            var connVm = Connections[ci3];
            var involves = false;
            for (var ci4 = 0; ci4 < allConnectors.Count; ci4++)
            {
                var conn = allConnectors[ci4];
                if (connVm.Edge.Source != conn && connVm.Edge.Target != conn) continue;
                involves = true;
                break;
            }
            if (involves)
            {
                connsToRemove.Add(connVm);
            }
        }
        for (var ri2 = 0; ri2 < connsToRemove.Count; ri2++)
        {
            Connections.Remove(connsToRemove[ri2]);
        }
    }

    private void UpdateBezier()
    {
        var dx = TempEndPoint.X - TempStart.X;
        var offset = Math.Abs(dx) * 0.3;

        TempControl1 = new Point(TempStart.X + (dx >= 0 ? offset : -offset), TempStart.Y);
        TempControl2 = new Point(TempEndPoint.X - (dx >= 0 ? offset : -offset), TempEndPoint.Y);
    }
}

using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Nodify.Views;

public partial class NodeControl
{
    public static readonly RoutedEvent ConnectorMouseDownEvent =
        EventManager.RegisterRoutedEvent(nameof(ConnectorMouseDown), RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(NodeControl));

    public static readonly RoutedEvent ConnectorMouseUpEvent =
        EventManager.RegisterRoutedEvent(nameof(ConnectorMouseUp), RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(NodeControl));

    public event RoutedEventHandler ConnectorMouseDown
    {
        add => AddHandler(ConnectorMouseDownEvent, value);
        remove => RemoveHandler(ConnectorMouseDownEvent, value);
    }

    public event RoutedEventHandler ConnectorMouseUp
    {
        add => AddHandler(ConnectorMouseUpEvent, value);
        remove => RemoveHandler(ConnectorMouseUpEvent, value);
    }

    public NodeControl()
    {
        InitializeComponent();
    }

    private void OnConnectorMouseDown(object s, MouseButtonEventArgs e)
    {
        if (s is not Ellipse ellipse) return;
        RaiseEvent(new RoutedEventArgs(ConnectorMouseDownEvent, ellipse));
        e.Handled = true;
    }

    private void OnConnectorMouseUp(object s, MouseButtonEventArgs e)
    {
        if (s is not Ellipse ellipse) return;
        RaiseEvent(new RoutedEventArgs(ConnectorMouseUpEvent, ellipse));
        e.Handled = true;
    }
}


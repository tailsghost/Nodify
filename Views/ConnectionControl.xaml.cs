using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Nodify.Views;

public partial class ConnectionControl : UserControl
{
    public static readonly RoutedEvent ConnectionMouseRightClickEvent =
        EventManager.RegisterRoutedEvent(nameof(ConnectionMouseRightClick), RoutingStrategy.Bubble,
            typeof(MouseButtonEventHandler), typeof(ConnectionControl));

    public event RoutedEventHandler ConnectionMouseRightClick
    {
        add => AddHandler(ConnectionMouseRightClickEvent, value);
        remove => RemoveHandler(ConnectionMouseRightClickEvent, value);
    }

    public ConnectionControl()
    {
        InitializeComponent();
    }

    private void OnConnectionMouseRightClick(object s, MouseButtonEventArgs e)
    {
        RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
        {
            RoutedEvent = ConnectionMouseRightClickEvent,
            Source = this
        });
        e.Handled = true;
    }
}


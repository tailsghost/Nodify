using Nodify.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Nodify.Views;

public partial class MenuLibraryControl : UserControl
{

    public static readonly RoutedEvent MenuMouseDownEvent
        = EventManager.RegisterRoutedEvent(nameof(MenuMouseDown), RoutingStrategy.Bubble,
            typeof(MouseButtonEventHandler), typeof(MenuLibraryControl));

    public static readonly RoutedEvent MenuMouseUpEvent
        = EventManager.RegisterRoutedEvent(nameof(MenuMouseUp), RoutingStrategy.Bubble, typeof(MouseButtonEventHandler),
            typeof(MenuLibraryControl));

    public static readonly RoutedEvent MenuMouseMoveEvent
        = EventManager.RegisterRoutedEvent(nameof(MenuMouseMove), RoutingStrategy.Bubble,
            typeof(MouseEventHandler), typeof(MenuLibraryControl));


    public event MouseButtonEventHandler MenuMouseDown
    {
        add => AddHandler(MenuMouseDownEvent, value);
        remove => RemoveHandler(MenuMouseDownEvent, value);
    }

    public event MouseButtonEventHandler MenuMouseUp
    {
        add => AddHandler(MenuMouseUpEvent, value);
        remove => RemoveHandler(MenuMouseUpEvent, value);
    }

    public event MouseButtonEventHandler MenuMouseMove
    {
        add => AddHandler(MenuMouseMoveEvent, value);
        remove => RemoveHandler(MenuMouseMoveEvent, value);
    }

    public MenuLibraryControl()
    {
        InitializeComponent();
    }

    private void OnMenuMouseDown(object s, MouseButtonEventArgs e)
    {
        RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
        {
            RoutedEvent = MenuMouseDownEvent,
            Source = s
        });
    }

    private void OnMenuMouseMove(object s, MouseEventArgs e)
    {
        RaiseEvent(new MouseEventArgs(e.MouseDevice, e.Timestamp)
        {
            RoutedEvent = MenuMouseMoveEvent,
        });
        e.Handled = true;
    }

    private void OnMenuMouseUp(object s, MouseButtonEventArgs e)
    {
        RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
        {
            RoutedEvent = MenuMouseUpEvent,
        });
        e.Handled = true;
    }
}


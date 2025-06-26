using Nodify.Helpers;
using Nodify.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Nodify.Views;

public partial class NodeControl
{

    private Canvas _currentCanvas;

    public static readonly RoutedEvent ConnectorMouseDownEvent =
        EventManager.RegisterRoutedEvent(nameof(ConnectorMouseDown), RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(NodeControl));

    public static readonly RoutedEvent ConnectorMouseUpEvent =
        EventManager.RegisterRoutedEvent(nameof(ConnectorMouseUp), RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(NodeControl));

    public static readonly RoutedEvent NodeMouseDownEvent =
        EventManager.RegisterRoutedEvent(nameof(NodeMouseMoveDown), RoutingStrategy.Bubble,
            typeof(MouseButtonEventHandler), typeof(NodeControl));

    public static readonly RoutedEvent NodeMouseUpEvent =
        EventManager.RegisterRoutedEvent(nameof(NodeMouseMoveUp), RoutingStrategy.Bubble,
            typeof(MouseButtonEventHandler), typeof(NodeControl));

    public static readonly RoutedEvent NodeMouseMoveEvent =
        EventManager.RegisterRoutedEvent(nameof(NodeMouseMove),
            RoutingStrategy.Bubble,
            typeof(MouseEventHandler),
            typeof(NodeControl));

    public static readonly RoutedEvent NodeMouseRightClickEvent =
        EventManager.RegisterRoutedEvent(nameof(NodeMouseRightClick),
            RoutingStrategy.Bubble,
            typeof(MouseButtonEventHandler),
            typeof(NodeControl));

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

    public event MouseButtonEventHandler NodeMouseMoveDown
    {
        add => AddHandler(NodeMouseDownEvent, value);
        remove => RemoveHandler(NodeMouseDownEvent, value);
    }

    public event MouseButtonEventHandler NodeMouseMoveUp
    {
        add => AddHandler(NodeMouseUpEvent, value);
        remove => RemoveHandler(NodeMouseUpEvent, value);
    }

    public event MouseButtonEventHandler NodeMouseMove
    {
        add => AddHandler(NodeMouseMoveEvent, value);
        remove => RemoveHandler(NodeMouseMoveEvent, value);
    }

    public event MouseButtonEventHandler NodeMouseRightClick
    {
        add => AddHandler(NodeMouseRightClickEvent, value);
        remove => RemoveHandler(NodeMouseRightClickEvent, value);
    }

    public NodeControl()
    {
        InitializeComponent();

        Node.AddHandler(
            UIElement.PreviewMouseLeftButtonDownEvent,
            new MouseButtonEventHandler(OnNodeMouseDown),
            handledEventsToo: true
        );

        AddHandler(
            UIElement.PreviewMouseLeftButtonUpEvent,
            new MouseButtonEventHandler(OnNodeMouseUp),
            handledEventsToo: true
        );

        AddHandler(
            UIElement.MouseMoveEvent,
            new MouseEventHandler(OnNodeMove),
            handledEventsToo: true
        );

        LayoutUpdated += NodeControl_LayoutUpdated;

    }

    private void NodeControl_LayoutUpdated(object? sender, EventArgs e)
    {
        UpdateConnectorPositions();
    }

    private void UpdateConnectorPositions()
    {
        var ellipses = FindAncestorHelper.FindVisualChildren<Ellipse>(this);
        foreach (var ellipse in ellipses)
        {
            if (ellipse.Tag is not ConnectorViewModel connector) continue;

            var center = new Point(ellipse.ActualWidth / 2, ellipse.ActualHeight / 2);

            if (_currentCanvas == null)
            {
                var canvas = FindAncestorHelper.FindAncestor<Canvas>(ellipse);
                if (canvas == null) continue;

                _currentCanvas = canvas;
            }


            if (_currentCanvas == null) continue;
            var position = ellipse.TranslatePoint(center, _currentCanvas);

            connector.Model.X = position.X - 1.5;
            connector.Model.Y = position.Y - 1.5;
        }
    }


    private void OnNodeMouseDown(object s, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is Ellipse)
            return;

        CaptureMouse();
        RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
        {
            RoutedEvent = NodeMouseDownEvent,
            Source = this
        });
        e.Handled = true;
    }

    private void OnNodeMove(object s, MouseEventArgs e)
    {
        RaiseEvent(new MouseEventArgs(e.MouseDevice, e.Timestamp)
        {
            RoutedEvent = NodeMouseMoveEvent,
            Source = this
        });
        e.Handled = true;
    }

    private void OnNodeMouseUp(object s, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is Ellipse)
            return;
        ReleaseMouseCapture();
        RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
        {
            RoutedEvent = NodeMouseUpEvent,
            Source = this
        });
        e.Handled = true;
    }

    private void OnNodeMouseRightClick(object s, MouseButtonEventArgs e)
    {
        RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
        {
            RoutedEvent = NodeMouseRightClickEvent,
            Source = this
        });

        e.Handled = true;
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
        Mouse.Capture(null);
        RaiseEvent(new RoutedEventArgs(ConnectorMouseUpEvent, ellipse));
        e.Handled = true;
    }
}


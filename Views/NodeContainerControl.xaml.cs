using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Nodify.Views
{
    public partial class NodeContainerControl : UserControl
    {
        public static readonly RoutedEvent NodeContainerMouseDownEvent =
            EventManager.RegisterRoutedEvent(nameof(NodeContainerMouseDown),
                RoutingStrategy.Bubble,
                typeof(MouseButtonEventHandler),
                typeof(NodeContainerControl));

        public static readonly RoutedEvent NodeContainerMouseUpEvent =
            EventManager.RegisterRoutedEvent(nameof(NodeContainerMouseUp),
                RoutingStrategy.Bubble,
                typeof(MouseButtonEventHandler),
                typeof(NodeContainerControl));

        public static readonly RoutedEvent NodeContainerMoveEvent =
            EventManager.RegisterRoutedEvent(nameof(NodeContainerMove),
                RoutingStrategy.Bubble,
                typeof(MouseEventHandler),
                typeof(NodeContainerControl));

        public event MouseEventHandler NodeContainerMouseDown
        {
            add => AddHandler(NodeContainerMouseDownEvent, value);
            remove => RemoveHandler(NodeContainerMouseDownEvent, value);
        }

        public event MouseButtonEventHandler NodeContainerMouseUp
        {
            add => AddHandler(NodeContainerMouseUpEvent, value);
            remove => RemoveHandler(NodeContainerMouseUpEvent, value);
        }

        public event MouseButtonEventHandler NodeContainerMove
        {
            add => AddHandler(NodeContainerMoveEvent, value);
            remove => RemoveHandler(NodeContainerMoveEvent, value);
        }

        public NodeContainerControl()
        {
            InitializeComponent();

            AddHandler(
                UIElement.MouseLeftButtonUpEvent,
                new MouseButtonEventHandler(OnNodeContainerMouseUp),
                handledEventsToo: true
            );

            AddHandler(
                UIElement.MouseMoveEvent,
                new MouseEventHandler(OnNodeContainerMove),
                handledEventsToo: true
            );
        }

        private void OnNodeContainerMouseDown(object s, MouseButtonEventArgs e)
        {
            CaptureMouse();
            RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
            {
                RoutedEvent = NodeContainerMouseDownEvent,
                Source = this
            });
            e.Handled = true;
        }

        private void OnNodeContainerMove(object s, MouseEventArgs e)
        {
            RaiseEvent(new MouseEventArgs(e.MouseDevice, e.Timestamp)
            {
                RoutedEvent = NodeContainerMoveEvent,
                Source = this
            });
            e.Handled = true;
        }

        private void OnNodeContainerMouseUp(object s, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            RaiseEvent(new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, e.ChangedButton)
            {
                RoutedEvent = NodeContainerMouseUpEvent,
                Source = this
            });
            e.Handled = true;
        }
    }
}
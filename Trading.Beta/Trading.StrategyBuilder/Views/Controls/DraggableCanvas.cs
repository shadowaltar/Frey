using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Trading.StrategyBuilder.Views.Controls
{
    public class DraggableCanvas : Canvas
    {
        private UIElement elementBeingDragged;
        private bool isDragInProgress;
        private bool modifyLeftOffset, modifyTopOffset;
        private Point originalCursorLocation;
        private double originalHorizontalOffset, originVerticalOffset;

        public static readonly DependencyProperty SelectedElementProperty = DependencyProperty.Register("SelectedElement",
                typeof(object),
                typeof(DraggableCanvas),
                new FrameworkPropertyMetadata(null, OnSelectedElementChangedStatic));

        public static readonly DependencyProperty AllowDraggingProperty = DependencyProperty.Register("AllowDragging",
                typeof(bool),
                typeof(DraggableCanvas),
                new PropertyMetadata(true));

        public static readonly DependencyProperty AllowDragOutOfViewProperty = DependencyProperty.Register("AllowDragOutOfView",
                typeof(bool),
                typeof(DraggableCanvas),
                new UIPropertyMetadata(false));

        /// <summary>
        /// Enable grid-snapping when dragging elements.
        /// </summary>
        public static readonly DependencyProperty SnapToGridProperty = DependencyProperty.Register("SnapToGrid",
                typeof(bool),
                typeof(DraggableCanvas),
                new UIPropertyMetadata(false));

        protected int snapDistance;
        /// <summary>
        /// Define the pixel distance of snapping when dragging elements.
        /// </summary>
        public static readonly DependencyProperty SnapDistanceProperty = DependencyProperty.Register("SnapDistance",
                typeof(int),
                typeof(DraggableCanvas),
                new FrameworkPropertyMetadata(1, OnSnapDistanceChanged, (d, v) => (int)v > 1 ? v : 1), v => (int)v >= 1);

        #region SnapDistanceChanged event

        public static readonly RoutedEvent SnapDistanceChangedEvent =
            EventManager.RegisterRoutedEvent("SnapDistanceChanged", RoutingStrategy.Direct,
                typeof(RoutedPropertyChangedEventArgs<int>), typeof(DraggableCanvas));

        public event RoutedPropertyChangedEventHandler<int> SnapDistanceChanged;

        private static void OnSnapDistanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = ((DraggableCanvas)d);
            control.snapDistance = (int)e.NewValue;
            control.OnSnapDistanceChanged((int)e.OldValue, control.snapDistance);
        }

        protected virtual void OnSnapDistanceChanged(int oldValue, int newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue) { RoutedEvent = SnapDistanceChangedEvent };
            RaiseEvent(e);
        }

        #endregion

        #region SelectedElementChanged event

        public static readonly RoutedEvent SelectedElementChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedElementChanged", RoutingStrategy.Direct,
                typeof(RoutedPropertyChangedEventArgs<object>), typeof(DraggableCanvas));

        public event RoutedPropertyChangedEventHandler<int> SelectedElementChanged;

        private static void OnSelectedElementChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = ((DraggableCanvas)d);
            control.SelectedElement = e.NewValue;
            control.OnSelectedElementChanged(e.OldValue, control.SelectedElement);
        }

        protected virtual void OnSelectedElementChanged(object oldValue, object newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<object>(oldValue, newValue) { RoutedEvent = SelectedElementChangedEvent };
            RaiseEvent(e);
        }

        #endregion

        #region OnElementDragged event

        public event ElementDraggedDelegate ElementDragged;

        protected virtual void OnElementDragged(object element, double xOffset, double yOffset)
        {
            if (ElementDragged != null)
            {
                var args = new ElementDraggedEventArgs { Element = element, XOffset = xOffset, YOffset = yOffset };
                ElementDragged.Invoke(args);
            }
        }

        #endregion

        public bool AllowDragging
        {
            get { return (bool)GetValue(AllowDraggingProperty); }
            set { SetValue(AllowDraggingProperty, value); }
        }

        public bool AllowDragOutOfView
        {
            get { return (bool)GetValue(AllowDragOutOfViewProperty); }
            set { SetValue(AllowDragOutOfViewProperty, value); }
        }

        public bool SnapToGrid
        {
            get { return (bool)GetValue(SnapToGridProperty); }
            set { SetValue(SnapToGridProperty, value); }
        }

        public int SnapDistance
        {
            get { return (int)GetValue(SnapDistanceProperty); }
            set { SetValue(SnapDistanceProperty, value); }
        }

        public UIElement ElementBeingDragged
        {
            get { return !AllowDragging ? null : elementBeingDragged; }
            protected set { SetupElementBeingDragged(value); }
        }

        public object SelectedElement
        {
            get { return GetValue(SelectedElementProperty); }
            set { SetValue(SelectedElementProperty, value); }
        }

        public DraggableCanvas() { }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            foreach (UIElement child in Children)
            {
                var c = FindCanvasChild(child);
                var left = GetLeft(c);
                var top = GetTop(c);
                SetLeft(c, left - left % SnapDistance);
                SetTop(c, top - top % SnapDistance);
            }
        }

        public void BringToFront(UIElement element)
        {
            UpdateZOrder(element, true);
        }

        public void SendToBack(UIElement element)
        {
            UpdateZOrder(element, false);
        }

        public UIElement FindCanvasChild(DependencyObject depObj)
        {
            while (depObj != null)
            {
                // If the current object is a UIElement which is a child of the
                // Canvas, exit the loop and return it.
                var elem = depObj as UIElement;
                if (elem != null && Children.Contains(elem))
                    break;

                // VisualTreeHelper works with objects of type Visual or Visual3D.
                // If the current object is not derived from Visual or Visual3D,
                // then use the LogicalTreeHelper to find the parent element.
                if (depObj is Visual || depObj is Visual3D)
                    depObj = VisualTreeHelper.GetParent(depObj);
                else
                    depObj = LogicalTreeHelper.GetParent(depObj);
            }
            return depObj as UIElement;
        }

        private void SetupElementBeingDragged(UIElement value)
        {
            if (elementBeingDragged != null)
                elementBeingDragged.ReleaseMouseCapture();

            if (AllowDragging && DragInCanvas.GetCanBeDragged(value))
            {
                elementBeingDragged = value;
                elementBeingDragged.CaptureMouse();
            }
            else
            {
                elementBeingDragged = null;
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            isDragInProgress = false;

            // Cache the mouse cursor location.
            originalCursorLocation = e.GetPosition(this);

            // Walk up the visual tree from the element that was clicked, 
            // looking for an element that is a direct child of the Canvas.
            ElementBeingDragged = FindCanvasChild(e.Source as DependencyObject);
            if (ElementBeingDragged == null)
                return;

            SelectedElement = ElementBeingDragged;

            // Get the element's offsets from the four sides of the Canvas.
            double left = GetLeft(ElementBeingDragged);
            double right = GetRight(ElementBeingDragged);
            double top = GetTop(ElementBeingDragged);
            double bottom = GetBottom(ElementBeingDragged);

            // Calculate the offset deltas and determine for which sides
            // of the Canvas to adjust the offsets.
            originalHorizontalOffset = ResolveOffset(left, right, out modifyLeftOffset);
            originVerticalOffset = ResolveOffset(top, bottom, out modifyTopOffset);

            // Set the Handled flag so that a control being dragged 
            // does not react to the mouse input.
            e.Handled = true;

            isDragInProgress = true;
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            // If no element is being dragged, there is nothing to do.
            if (ElementBeingDragged == null || !isDragInProgress)
                return;

            // Get the position of the mouse cursor, relative to the Canvas.
            Point cursorLocation = e.GetPosition(this);

            // These values will store the new offsets of the drag element.

            #region Calculate Offsets

            var horizontalOffset = cursorLocation.X - originalCursorLocation.X;
            var verticalOffset = cursorLocation.Y - originalCursorLocation.Y;
            if (SnapToGrid && SnapDistance != 1)
            {
                horizontalOffset -= horizontalOffset % SnapDistance;
                verticalOffset -= verticalOffset % SnapDistance;
            }

            // Determine the horizontal offset.
            double newHorizontalOffset = modifyLeftOffset
                ? originalHorizontalOffset + horizontalOffset
                : originalHorizontalOffset - horizontalOffset;

            // Determine the vertical offset.
            double newVerticalOffset = modifyTopOffset
                ? originVerticalOffset + verticalOffset
                : originVerticalOffset - verticalOffset;

            #endregion // Calculate Offsets

            if (!AllowDragOutOfView)
            {
                #region Verify Drag Element Location

                // Get the bounding rect of the drag element.
                Rect elemRect = CalculateDragElementRect(newHorizontalOffset, newVerticalOffset);

                //
                // If the element is being dragged out of the viewable area, 
                // determine the ideal rect location, so that the element is 
                // within the edge(s) of the canvas.
                //
                bool leftAlign = elemRect.Left < 0;
                bool rightAlign = elemRect.Right > ActualWidth;

                if (leftAlign)
                    newHorizontalOffset = modifyLeftOffset ? 0 : ActualWidth - elemRect.Width;
                else if (rightAlign)
                    newHorizontalOffset = modifyLeftOffset ? ActualWidth - elemRect.Width : 0;

                bool topAlign = elemRect.Top < 0;
                bool bottomAlign = elemRect.Bottom > ActualHeight;

                if (topAlign)
                    newVerticalOffset = modifyTopOffset ? 0 : ActualHeight - elemRect.Height;
                else if (bottomAlign)
                    newVerticalOffset = modifyTopOffset ? ActualHeight - elemRect.Height : 0;

                #endregion // Verify Drag Element Location
            }

            #region Move Drag Element

            if (modifyLeftOffset)
                SetLeft(ElementBeingDragged, newHorizontalOffset);
            else
                SetRight(ElementBeingDragged, newHorizontalOffset);

            if (modifyTopOffset)
                SetTop(ElementBeingDragged, newVerticalOffset);
            else
                SetBottom(ElementBeingDragged, newVerticalOffset);

            if (originalHorizontalOffset != newHorizontalOffset || originVerticalOffset != newVerticalOffset)
            {
                Console.WriteLine(originalHorizontalOffset + "->"+ newHorizontalOffset+";");
                OnElementDragged(ElementBeingDragged, newHorizontalOffset, newVerticalOffset);
            }

            #endregion // Move Drag Element
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            SelectedElement = Equals(SelectedElement, ElementBeingDragged) ? null : ElementBeingDragged;

            // Reset the field whether the left or right mouse button was 
            // released, in case a context menu was opened on the drag element.
            ElementBeingDragged = null;
        }

        /// <summary>
        /// Returns a Rect which describes the bounds of the element being dragged.
        /// </summary>
        private Rect CalculateDragElementRect(double newHorizOffset, double newVertOffset)
        {
            if (ElementBeingDragged == null)
                throw new InvalidOperationException("ElementBeingDragged is null.");

            Size elemSize = ElementBeingDragged.RenderSize;

            double x, y;

            if (modifyLeftOffset)
                x = newHorizOffset;
            else
                x = ActualWidth - newHorizOffset - elemSize.Width;

            if (modifyTopOffset)
                y = newVertOffset;
            else
                y = ActualHeight - newVertOffset - elemSize.Height;

            var elemLoc = new Point(x, y);

            return new Rect(elemLoc, elemSize);
        }

        /// <summary>
        ///     Determines one component of a UIElement's location
        ///     within a Canvas (either the horizontal or vertical offset).
        /// </summary>
        /// <param name="side1">
        ///     The value of an offset relative to a default side of the
        ///     Canvas (i.e. top or left).
        /// </param>
        /// <param name="side2">
        ///     The value of the offset relative to the other side of the
        ///     Canvas (i.e. bottom or right).
        /// </param>
        /// <param name="useSide1">
        ///     Will be set to true if the returned value should be used
        ///     for the offset from the side represented by the 'side1'
        ///     parameter.  Otherwise, it will be set to false.
        /// </param>
        private static double ResolveOffset(double side1, double side2, out bool useSide1)
        {
            // If the Canvas.Left and Canvas.Right attached properties 
            // are specified for an element, the 'Left' value is honored.
            // The 'Top' value is honored if both Canvas.Top and 
            // Canvas.Bottom are set on the same element.  If one 
            // of those attached properties is not set on an element, 
            // the default value is Double.NaN.
            useSide1 = true;
            double result;
            if (double.IsNaN(side1))
            {
                if (double.IsNaN(side2))
                {
                    // Both sides have no value, so set the
                    // first side to a value of zero.
                    result = 0;
                }
                else
                {
                    result = side2;
                    useSide1 = false;
                }
            }
            else
            {
                result = side1;
            }
            return result;
        }

        /// <summary>
        ///     Helper method used by the BringToFront and SendToBack methods.
        /// </summary>
        /// <param name="element">
        ///     The element to bring to the front or send to the back.
        /// </param>
        /// <param name="bringToFront">
        ///     Pass true if calling from BringToFront, else false.
        /// </param>
        private void UpdateZOrder(UIElement element, bool bringToFront)
        {
            #region Safety Check

            if (element == null)
                throw new ArgumentNullException("element");

            if (!Children.Contains(element))
                throw new ArgumentException("Must be a child element of the Canvas.", "element");

            #endregion // Safety Check

            #region Calculate Z-Indici And Offset

            // Determine the Z-Index for the target UIElement.
            int elementNewZIndex = -1;
            if (bringToFront)
            {
                elementNewZIndex += Children.Cast<UIElement>()
                    .Count(elem => elem.Visibility != Visibility.Collapsed);
            }
            else
            {
                elementNewZIndex = 0;
            }

            // Determine if the other UIElements' Z-Index 
            // should be raised or lowered by one. 
            int offset = (elementNewZIndex == 0) ? +1 : -1;

            int elementCurrentZIndex = GetZIndex(element);

            #endregion // Calculate Z-Indices And Offset

            #region Update Z-Indici

            // Update the Z-Index of every UIElement in the Canvas.
            foreach (UIElement childElement in Children)
            {
                if (Equals(childElement, element))
                {
                    SetZIndex(element, elementNewZIndex);
                }
                else
                {
                    int zIndex = GetZIndex(childElement);

                    // Only modify the z-index of an element if it is  
                    // in between the target element's old and new z-index.
                    if (bringToFront && elementCurrentZIndex < zIndex ||
                        !bringToFront && zIndex < elementCurrentZIndex)
                    {
                        SetZIndex(childElement, zIndex + offset);
                    }
                }
            }

            #endregion // Update Z-Indices
        }
    }

    public delegate void ElementDraggedDelegate(ElementDraggedEventArgs args);

    public class ElementDraggedEventArgs : EventArgs
    {
        public object Element { get; set; }
        public double XOffset { get; set; }
        public double YOffset { get; set; }
    }

    public static class DragInCanvas
    {
        public static readonly DependencyProperty CanBeDraggedProperty
            = DependencyProperty.RegisterAttached("CanBeDragged",
            typeof(bool),
            typeof(DraggableCanvas),
            new UIPropertyMetadata(true));

        public static bool GetCanBeDragged(UIElement uiElement)
        {
            if (uiElement == null)
                return false;

            return (bool)uiElement.GetValue(CanBeDraggedProperty);
        }

        public static void SetCanBeDragged(UIElement uiElement, bool value)
        {
            if (uiElement != null)
                uiElement.SetValue(CanBeDraggedProperty, value);
        }
    }
}
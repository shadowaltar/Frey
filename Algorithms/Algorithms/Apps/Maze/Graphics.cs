using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Algorithms.Apps.Maze
{
    public class Graphics
    {
        private readonly Canvas canvas;
        private readonly Window root;
        private double canvasWidth;
        private double canvasHeight;
        private double widthOffset;
        private double heightOffset;

        public Graphics(Canvas canvas)
        {
            this.canvas = canvas;
            LineStroke = new SolidColorBrush(Colors.Black);
            FillColor = new SolidColorBrush(Colors.Black);
            LineThickness = 1;
            root = FindAncestor<Window>(canvas);
            if (root == null)
                throw new InvalidOperationException("No Window!");
        }

        public Brush LineStroke { get; set; }
        public Brush FillColor { get; set; }

        public double LineThickness { get; set; }

        public double CanvasWidth
        {
            get { return canvasWidth; }
            set
            {
                widthOffset = root.ActualWidth - canvas.ActualWidth;
                root.Width = value + widthOffset;
                canvasWidth = value;
            }
        }

        public double CanvasHeight
        {
            get { return canvasHeight; }
            set
            {
                heightOffset = root.ActualHeight - canvas.ActualHeight;
                root.Height = value + heightOffset;
                canvasHeight = value;
            }
        }

        public void SetCanvasSize(double width, double height)
        {
            CanvasWidth = width;
            CanvasHeight = height;
            root.Width = CanvasWidth + 20;
            root.Height = CanvasHeight + 42;
        }

        public void Clear()
        {
            canvas.Children.Clear();
        }

        public void DrawLine(double x1, double y1, double x2, double y2)
        {
            var line = new Line
            {
                Stroke = LineStroke,
                StrokeThickness = LineThickness,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };
            canvas.Children.Add(line);
        }

        public void DrawRectangle(double x, double y, double width, double height)
        {
            DrawRectangle(x, y, width, height, null);
        }

        protected void DrawRectangle(double x, double y, double width, double height, Brush fill)
        {
            var rectangle = new Rectangle
            {
                Stroke = LineStroke,
                StrokeThickness = LineThickness,
                Fill = fill,
                Width = width,
                Height = height,
                Margin = new Thickness(x, y, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            canvas.Children.Add(rectangle);
        }

        public T FindAncestor<T>(FrameworkElement element) where T : FrameworkElement
        {
            var parent = element.Parent as FrameworkElement;
            if (parent is T)
            {
                return (T)parent;
            }
            if (parent != null)
            {
                return FindAncestor<T>(parent);
            }
            return null;
        }
    }
}
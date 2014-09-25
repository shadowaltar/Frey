using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Charting
{
    public static class Charter
    {
        static Charter()
        {
            LineStroke = new SolidColorBrush(Colors.Black);
        }

        public static Brush LineStroke { get; set; }

        public static void DrawLine(this Canvas canvas, double x1, double y1, double x2, double y2)
        {
            var line = new Line
            {
                Stroke = LineStroke,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };
            canvas.Children.Add(line);
        }

        public static void DrawRectangle(this Canvas canvas, double x, double y, double width, double height)
        {
            var rectangle = new Rectangle
            {
                Stroke = LineStroke,
                Width = width,
                Height = height,
                Margin = new Thickness(x, y, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            canvas.Children.Add(rectangle);
        }
    }
}
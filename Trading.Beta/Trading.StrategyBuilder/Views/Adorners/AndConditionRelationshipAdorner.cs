using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using GraphSharp.Controls;

namespace Trading.StrategyBuilder.Views.Adorners
{
    public class AndConditionRelationshipAdorner : Adorner
    {
        public AndConditionRelationshipAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }
        private static SolidColorBrush renderBrush = new SolidColorBrush(Colors.Black) { Opacity = 0.2 };
        private static Pen renderPen = new Pen(new SolidColorBrush(Colors.Green), 1.5);

        protected override void OnRender(DrawingContext drawingContext)
        {
            //var center = new Point(rect.Left + rect.Width / 2,
            //    rect.Top + rect.Height / 2);
            //drawingContext.DrawEllipse(renderBrush, renderPen, center, 5, 5);
            double renderRadius = 5.0;
            var v1 = ((EdgeControl)AdornedElement).Source;
            var v2 = ((EdgeControl)AdornedElement).Target;

            // var rect = new Rect(AdornedElement.DesiredSize);
            var rect1 = new Rect(v1.DesiredSize);
            var c1 = new Point(rect1.Left + rect1.Width / 2, rect1.Top + rect1.Height / 2);
            var rect2 = new Rect(v2.DesiredSize);
            var c2 = new Point(rect2.Left + rect2.Width / 2, rect2.Top + rect2.Height / 2);

            // Draw a circle at each corner.
            drawingContext.DrawEllipse(renderBrush, renderPen, c1, renderRadius, renderRadius);
            drawingContext.DrawEllipse(renderBrush, renderPen, c2, renderRadius, renderRadius);
            //drawingContext.DrawEllipse(renderBrush, renderPen, rect.TopRight, renderRadius, renderRadius);
            //drawingContext.DrawEllipse(renderBrush, renderPen, rect.BottomLeft, renderRadius, renderRadius);
            //drawingContext.DrawEllipse(renderBrush, renderPen, rect.BottomRight, renderRadius, renderRadius);
        }
    }
}
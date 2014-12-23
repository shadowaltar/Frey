using System;
using System.Windows;
using System.Windows.Controls;
using Arrowheads;

namespace Trading.StrategyBuilder.Views.Controls
{
    public class AndLine : ArrowLine
    {
        private readonly DraggableCanvas canvas;
        private readonly UserControl control1;
        private readonly UserControl control2;

        public AndLine()
        {
            Symbol = new AndSymbol();
        }

        public AndLine(DraggableCanvas canvas, UserControl control1, UserControl control2)
            : this()
        {
            this.canvas = canvas;
            this.control1 = control1;
            this.control2 = control2;

            canvas.ElementDragged += OnElementDragged;
        }

        private int counter = 0;
        private void OnElementDragged(ElementDraggedEventArgs args)
        {

            if (Equals(args.Element, control1))
            {
                Console.WriteLine(counter);
                counter++;
            }
        }

        public Point[] RecalculatePoints()
        {
            Console.WriteLine(control1.GetValue(Canvas.TopProperty));
            Console.WriteLine(control1.GetValue(Canvas.LeftProperty));
            Console.WriteLine(control1.GetValue(ActualWidthProperty));
            Console.WriteLine(control1.GetValue(ActualHeightProperty));

            return null;
        }

        public AndSymbol Symbol { get; private set; }
    }
}
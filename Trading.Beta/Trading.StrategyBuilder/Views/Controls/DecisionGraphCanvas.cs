using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Arrowheads;
using Condition = Trading.StrategyBuilder.Core.Condition;

namespace Trading.StrategyBuilder.Views.Controls
{
    public class DecisionGraphCanvas : DraggableCanvas
    {
        protected double StartingTopOffset { get { return snapDistance * 1; } }
        protected double StartingLeftOffset { get { return snapDistance * 1; } }

        protected Dictionary<Condition, ConditionView> ConditionMappings = new Dictionary<Condition, ConditionView>();

        public void AddCondition(Condition condition)
        {
            var cv = new ConditionView { DataContext = condition };
            ConditionMappings[condition] = cv;

            // set ui
            Children.Add(cv);
            cv.SetValue(TopProperty, StartingTopOffset);
            cv.SetValue(LeftProperty, StartingLeftOffset);

            cv.Loaded += OnConditionViewLoaded;
        }

        public void LinkConditionsWithAnd(Condition one, Condition two)
        {
            var conditionView1 = GetConditionView(one);
            var conditionView2 = GetConditionView(two);
            var arrowLine = new AndLine(this, conditionView1, conditionView2);

            arrowLine.X1 = 60;
            arrowLine.X2 = 120;
            arrowLine.Y1 = 40;
            arrowLine.Y2 = 80;
            arrowLine.ArrowLength = 5;
            arrowLine.Fill = new SolidColorBrush(Colors.Brown);
            arrowLine.IsArrowClosed = true;
            arrowLine.ArrowEnds = ArrowEnds.Both;
            arrowLine.StrokeThickness = 2;
            arrowLine.Stroke = new SolidColorBrush(Colors.Brown);
            DragInCanvas.SetCanBeDragged(arrowLine, false);

            Children.Add(arrowLine);

            arrowLine.RecalculatePoints();
        }

        public void LinkConditionsWithOr(Condition condition)
        {
            var cv = new ConditionView();
            cv.DataContext = condition;
            Children.Add(cv);
            cv.SetValue(TopProperty, StartingTopOffset);
            cv.SetValue(LeftProperty, StartingLeftOffset);

            cv.Loaded += OnConditionViewLoaded;
        }

        private void OnConditionViewLoaded(object sender, RoutedEventArgs e)
        {
            RectifyConditionView((Control)sender);
        }

        public void RemoveCondition()
        {
            if (SelectedElement != null)
            {

            }
        }

        public void LinkElement()
        {

        }

        private void RectifyConditionView(Control view)
        {
            var unit = snapDistance;
            var actual = (int)view.ActualWidth;
            if (actual == (int)view.MinWidth)
                return;

            var newWidth = ((actual / unit) + 1) * unit;
            view.SetValue(WidthProperty, (double)newWidth);
        }

        private ConditionView GetConditionView(Condition condition)
        {
            ConditionView result;
            ConditionMappings.TryGetValue(condition, out result);
            return result;
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;
using Condition = Trading.StrategyBuilder.Core.Condition;

namespace Trading.StrategyBuilder.Views.Controls
{
    public class DecisionGraphCanvas : DraggableCanvas
    {
        protected double StartingTopOffset { get { return snapDistance * 1; } }
        protected double StartingLeftOffset { get { return snapDistance * 1; } }

        public void AddCondition(Condition condition)
        {
            var cv = new ConditionView();
            cv.DataContext = condition;
            Children.Add(cv);
            cv.SetValue(TopProperty, StartingTopOffset);
            cv.SetValue(LeftProperty, StartingLeftOffset);

            cv.Loaded += OnConditionViewLoaded;
        }

        public Condition GroupConditionsWithAnd(Condition[] condition)
        {
            var cv = new ConditionView();
            cv.DataContext = condition;
            Children.Add(cv);
            cv.SetValue(TopProperty, StartingTopOffset);
            cv.SetValue(LeftProperty, StartingLeftOffset);

            cv.Loaded += OnConditionViewLoaded;
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
    }
}
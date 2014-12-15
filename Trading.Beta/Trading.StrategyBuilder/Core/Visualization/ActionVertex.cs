using System;
using PropertyChanged;
using Trading.Common.ViewModels;

namespace Trading.StrategyBuilder.Core.Visualization
{
    [ImplementPropertyChanged]
    public class ActionVertex : ViewModelBaseSlim
    {
        public Condition Condition { get; set; }

        public string OverridingExpression { get; set; }

        public string Expression
        {
            get
            {
                if (Condition == null) return OverridingExpression;
                return Condition.ToString();
            }
        }

        public bool IsSelected { get; set; }
        public bool CanSelect { get; set; }

        public event SelectEvent SelectEvent;

        public void OnSelected()
        {
            if (CanSelect)
            {
                IsSelected = true;
                if (SelectEvent != null)
                    SelectEvent(this, null);
            }
        }

        public override string ToString()
        {
            return Expression;
        }
    }

    [ImplementPropertyChanged]
    public class FilterVertex : ActionVertex
    {

        public override string ToString()
        {
            return Expression;
        }
    }

    public delegate void SelectEvent(object sender, EventArgs args);
}
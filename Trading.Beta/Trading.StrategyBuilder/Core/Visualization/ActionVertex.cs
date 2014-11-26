using System;
using PropertyChanged;
using Trading.Common.ViewModels;

namespace Trading.StrategyBuilder.Core.Visualization
{
    [ImplementPropertyChanged]
    public class ActionVertex : ViewModelBaseSlim
    {
        public string Expression { get; set; }

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
        public Condition
        public override string ToString()
        {
            return Expression;
        }
    }

    public delegate void SelectEvent(object sender, EventArgs args);
}
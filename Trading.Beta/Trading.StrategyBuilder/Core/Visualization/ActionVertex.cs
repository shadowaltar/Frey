using System;
using PropertyChanged;
using Trading.Common.ViewModels;

namespace Trading.StrategyBuilder.Core.Visualization
{
    [ImplementPropertyChanged]
    public class ActionVertex : ViewModelBaseSlim
    {
        public string Formula { get; set; }
        public string Action { get; set; }

        public bool IsSelected { get; set; }

        public void OnSelected()
        {
            Console.WriteLine("SELECTED");
            IsSelected = true;
        }

        public override string ToString()
        {
            return "IF " + Formula + " THEN " + Action;
        }
    }
}
using Caliburn.Micro;
using PropertyChanged;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Core;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class StageViewModel : ViewModelBase, IStageViewModel
    {
        public string StageName { get; set; }

        /// <summary>
        /// A group of conditions connected by AND relationship.
        /// </summary>
        public BindableCollection<Condition> Conditions { get; private set; }

        public StageViewModel()
        {
            Conditions = new BindableCollection<Condition>();
        }
    }

    public interface IStageViewModel
    {
        BindableCollection<Condition> Conditions { get; }
    }
}
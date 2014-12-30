using Caliburn.Micro;
using PropertyChanged;
using Trading.Common.ViewModels;
using Trading.StrategyBuilder.Core;

namespace Trading.StrategyBuilder.ViewModels
{
    [ImplementPropertyChanged]
    public class CreateStageViewModel : ViewModelBase, ICreateStageViewModel
    {
        public string StageName { get; set; }

        public Stage Yield()
        {
            return new Stage { Name = StageName };
        }

        public void Ok()
        {
            TryClose(true);
        }
    }

    public interface ICreateStageViewModel
    {
        string StageName { get; set; }
        Stage Yield();
    }
}
using Trading.Common.Data;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Trading.Common.ViewModels;

namespace Trading.R.ViewModels
{
    public class MainViewModel : MainViewModelBase<TradingDataAccess>, IMainViewModel
    {
        public MainViewModel(IDataAccessFactory<TradingDataAccess> dataAccessFactory, ISettings settings)
            : base(dataAccessFactory, settings)
        {
        }

        public override string ProgramName
        {
            get { return "R Sandbox"; }
        }

        public void Test()
        {
            
        }
    }

    public interface IMainViewModel : IHasViewService
    {
    }
}
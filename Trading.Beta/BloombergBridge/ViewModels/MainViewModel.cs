using BloombergBridge.Data;
using PropertyChanged;
using Trading.Common.Data;
using Trading.Common.Utils;
using Trading.Common.ViewModels;

namespace BloombergBridge.ViewModels
{
    [ImplementPropertyChanged]
    public class MainViewModel : MainViewModelBase<Access>, IMainViewModel
    {
        public override string ProgramName { get { return "Bloomberg Bridge"; } }

        public MainViewModel(IDataAccessFactory<Access> dataAccessFactory)
            : base(dataAccessFactory)
        {
        }
    }

    public interface IMainViewModel : IHasViewService
    {
    }
}
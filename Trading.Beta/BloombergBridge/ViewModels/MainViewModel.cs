using BloombergBridge.Data;
using Caliburn.Micro;
using PropertyChanged;
using Trading.Common.Data;
using Trading.Common.Entities;
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
            InputSecurities = new BindableCollection<string>();
            Securities = new BindableCollection<Security>();
            InputFields = new BindableCollection<string>();
            Fields = new BindableCollection<string>();
            HistoricalDataFrequencies = new BindableCollection<string>{
                "Daily","Weekly","Monthly","Quarterly","Quarterly","SemiAnnual","Yearly"};

            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            SelectedSecurity = null;
            Sele
            SelectedHistoricalDataFrequency = HistoricalDataFrequencies[0];
        }

        public BindableCollection<string> InputSecurities { get; private set; }
        public BindableCollection<Security> Securities { get; private set; }
        public BindableCollection<string> InputFields { get; private set; }
        public BindableCollection<string> Fields { get; private set; }
        public BindableCollection<string> HistoricalDataFrequencies { get; private set; }

        public string SelectedInputSecurity { get; set; }
        public Security SelectedSecurity { get; set; }
        public string SelectedInputField { get; set; }
        public string SelectedField { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public bool IsHistoricalRequest { get; set; }
        public string SelectedHistoricalDataFrequency { get; set; }

        public void Request()
        {
        }
    }

    public interface IMainViewModel : IHasViewService
    {
    }
}
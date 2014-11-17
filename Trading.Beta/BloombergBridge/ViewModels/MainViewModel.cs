using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using BloombergBridge.Data;
using BloombergBridge.Models;
using Caliburn.Micro;
using PropertyChanged;
using Trading.Common;
using Trading.Common.Data;
using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.Common.ViewModels;

namespace BloombergBridge.ViewModels
{
    [ImplementPropertyChanged]
    public class MainViewModel : MainViewModelBase<Access>, IMainViewModel
    {
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

        public override string ProgramName { get { return "Bloomberg Bridge"; } }

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

        private int fakeSecurityId;
        private HashSet<BloombergField> fields;

        public async void Request()
        {
            if (Securities.IsNullOrEmpty() || Fields.IsNullOrEmpty())
                return;
            if (IsHistoricalRequest && StartTime > EndTime)
                return;

            var workingFolder = ParseWorkingFolder();
            if (string.IsNullOrEmpty(workingFolder))
            {
                await ViewService.ShowWarning("Failed to initialize working folder.");
                return;
            }

            var contents = ConstructRequestFile();

            var timeStamp = DateTime.Now.ToTimeDouble().ToString();
            var path = Path.Combine(workingFolder, timeStamp
                + (IsHistoricalRequest ? ".historical.request" : ".request"));

            File.WriteAllText(path, contents);

            await MonitorResponse(workingFolder, timeStamp);
        }

        public void PlaceInputSecurity(ActionExecutionContext context)
        {
            var args = context.EventArgs as KeyEventArgs;
            if (args == null) return;
            if (SelectedInputSecurity.IsNullOrWhitespace()) return;
            if (!SelectedInputSecurity.Contains(" ")) return;

            if (args.Key == Key.Enter)
            {
                var inputSec = SelectedInputSecurity.ToUpperInvariant();
                if (!InputSecurities.Contains(inputSec))
                {
                    InputSecurities.Add(inputSec);
                }

                var id = Interlocked.Increment(ref fakeSecurityId);
                SelectedSecurity = new Security
                {
                    Id = id,
                    Code = inputSec.Split(' ')[0],
                    Identifiers = new SecurityIdentifiers { Bloomberg = inputSec }
                };
                if (!Securities.Contains(SelectedSecurity))
                {
                    Securities.Add(SelectedSecurity);
                }
            }
        }

        private void InitializeDefaults()
        {
            SelectedInputSecurity = null;
            SelectedSecurity = null;
            SelectedInputField = null;
            SelectedField = null;
            SelectedHistoricalDataFrequency = HistoricalDataFrequencies[0];
            StartTime = 20040101;
            EndTime = 20131231;
            SetupPredefinedBloombergFields();
        }

        private string ParseWorkingFolder()
        {
            string workingFolder = "";
            try
            {
                workingFolder = File.ReadAllText("WorkingFolder").Trim();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            if (!Directory.Exists(workingFolder))
                workingFolder = "";
            return workingFolder;
        }

        private string ConstructRequestFile()
        {
            var sb = new StringBuilder();
            sb.Append("SECURITIES:");
            for (int i = 0; i < Securities.Count; i++)
            {
                var security = Securities[i];
                sb.Append(security.Identifiers.Bloomberg);
                if (i != Securities.Count - 1)
                {
                    sb.Append('|');
                }
                else
                {
                    sb.AppendLine();
                }
            }
            sb.Append("FIELDS:");
            for (int i = 0; i < Fields.Count; i++)
            {
                var field = Fields[i];
                sb.Append(field);
                if (i != Fields.Count - 1)
                {
                    sb.Append('|');
                }
                else
                {
                    sb.AppendLine();
                }
            }
            if (IsHistoricalRequest)
            {
                sb.Append("STARTDATE:").AppendLine(StartTime.ToString())
                    .Append("ENDDATE:").AppendLine(EndTime.ToString())
                    .Append("PERIODICITYSELECTION").AppendLine(SelectedHistoricalDataFrequency);
            }
            return sb.ToString();
        }

        private Task MonitorResponse(string workingFolder, string expectedPartialFileName)
        {
            var fsw = new FileSystemWatcher
            {
                Path = workingFolder,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*" + expectedPartialFileName + "*.csv"
            };

            var tcs = new TaskCompletionSource<string[]>();
            var ct = new CancellationTokenSource(20000);
            ct.Token.Register(() => tcs.TrySetCanceled(), false);

            FileSystemEventHandler handler = null;
            handler = (s, args) =>
            {
                fsw.Changed -= handler;
                try
                {
                    var contents = File.ReadAllLines(args.FullPath);
                    tcs.TrySetResult(contents);
                }
                catch (Exception e)
                {
                    Log.Error("Cannot get responses.", e);
                }

                fsw.Dispose();
            };

            fsw.Changed += handler;
            fsw.EnableRaisingEvents = true;

            return tcs.Task;
        }

        private void SetupPredefinedBloombergFields()
        {
            var fn = Path.Combine(Constants.BloombergDataDirectory, "BloombergFields.csv");

            var results = FileHelper.Read<BloombergField>(fn);
            fields = new HashSet<BloombergField>();
            fields.AddRange(results);
            InputFields.AddRange(fields.Select(f => f.ToString()));
        }

    }

    public interface IMainViewModel : IHasViewService
    {
    }
}
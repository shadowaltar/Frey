using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using System;
using System.Linq;
using System.Threading.Tasks;
using Maintenance.Common.ViewModels;
using Maintenance.Portfolios.Entities;
using Portfolio = Maintenance.Portfolios.Entities.Portfolio;

namespace Maintenance.Portfolios.ViewModels
{
    public class BenchmarkAssociationFlyoutViewModel : ViewModelBase, IBenchmarkAssociationFlyoutViewModel
    {
        public BenchmarkAssociationFlyoutViewModel(IBenchmarkHistoryEditorViewModel editor)
        {
            benchmarkHistoryEditor = editor;
        }

        private readonly IBenchmarkHistoryEditorViewModel benchmarkHistoryEditor;

        public IViewService ViewService { get; set; }

        private IDataAccessFactory<PortfolioDataAccess> dataAccessFactory;
        public IDataAccessFactory<PortfolioDataAccess> DataAccessFactory
        {
            get { return dataAccessFactory; }
            set { dataAccessFactory = value; benchmarkHistoryEditor.DataAccessFactory = value; }
        }

        private Portfolio portfolio;

        public bool IsReady { get; set; }

        private Index selectedIndex;
        public Index SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (SetNotify(ref selectedIndex, value))
                {
                    CanSave = CheckCanSave();
                }
            }
        }

        private DateTime selectedExpiryDate;
        public DateTime SelectedExpiryDate
        {
            get { return selectedExpiryDate; }
            set
            {
                if (SetNotify(ref selectedExpiryDate, value))
                {
                    CanSave = CheckCanSave();
                }
            }
        }

        private DateTime selectedEffectiveDate;
        public DateTime SelectedEffectiveDate
        {
            get { return selectedEffectiveDate; }
            set
            {
                if (SetNotify(ref selectedEffectiveDate, value))
                {
                    CanSave = CheckCanSave();
                }
            }
        }

        private readonly BindableCollection<Index> indexes = new BindableCollection<Index>();
        public BindableCollection<Index> Indexes { get { return indexes; } }

        private bool canSave;
        public bool CanSave
        {
            get { return canSave; }
            set { SetNotify(ref canSave, value); }
        }

        private bool canShowHistoryEditor;
        public bool CanShowHistoryEditor
        {
            get { return canShowHistoryEditor; }
            set { SetNotify(ref canShowHistoryEditor, value); }
        }

        /// <summary>
        /// Let main vm sets the selected item to the edit vm.
        /// </summary>
        /// <param name="item"></param>
        public void SetItem(Portfolio item)
        {
            portfolio = item;

            if (item.Benchmark != null)
            {
                SelectedEffectiveDate = item.Benchmark.EffectiveDate;
                SelectedExpiryDate = item.Benchmark.ExpiryDate;
            }
            else
            {
                SelectedIndex = null;
                SelectedEffectiveDate = DateTime.Today;
                SelectedExpiryDate = DateTime.MaxValue.Date;
            }

            if (portfolio.Index != null)
            {
                SelectedIndex = Indexes.FirstOrDefault(b => b.Code == portfolio.Index.Code);
            }

            CanShowHistoryEditor = portfolio.Index != null;
            CanSave = false;
        }

        /// <summary>
        /// Save the edited item to db. The method contains UI logics also.
        /// </summary>
        public async void Save()
        {
            var decision = await ViewService.ShowMessage("Confirm to save?",
                "Remember you can't undo, and applications would immediately make use of your change.",
                false, true, "Yes", "No");
            if (decision != MessageDialogResult.Affirmative)
                return;

            var saveResult = SaveResult.Unknown;
            try
            {
                saveResult = await InternalSave();
            }
            catch (Exception e)
            {
                Log.Error("Error occurs when adding data to database.", e);
            }

            if (saveResult == SaveResult.Success)
            {
                CanSave = false;

                await ViewService.ShowMessage("Changes saved",
                    "Portfolio \"" + portfolio.Code + "\" is now associated with benchmark \"" + SelectedIndex.Code + "\".",
                    true, false, "ok");

                Publish<IMainViewModel>(ActivityType.ChangePortfolioToBenchmark, portfolio);
            }
            else
            {
                var message = "Cannot save the benchmark.";
                switch (saveResult)
                {
                    case SaveResult.CannotAddEntry:
                        message = string.Format(@"Cannot assign benchmark {0} (effective on {1}) to the portfolio."
                            , SelectedIndex.Code, SelectedEffectiveDate.IsoFormat());
                        break;
                    case SaveResult.ExpirySmallerThanEffective:
                        message = string.Format(@"To preserve benchmark relationship history, you cannot assign a benchmark which its effective date is equal or smaller than the previously assigned benchmark.
You specified {0} but it should be later than the date {1}."
                            , SelectedEffectiveDate.IsoFormat()
                            , portfolio.Benchmark.EffectiveDate.IsoFormat());
                        break;
                    case SaveResult.CannotModifyLastEntry:
                        message = @"Cannot adjust the expiry date of previously assigned benchmark.";
                        break;
                    // default: // this will never happen.
                }
                await ViewService.ShowError(message);
            }
        }

        private Task<SaveResult> InternalSave()
        {
            return Task.Factory.StartNew(() =>
            {
                Log.InfoFormat("Add bmk relationship item in database: {0}, Bmk:{1}->{2};EffectiveDate:{3}->{4}.",
                    portfolio.DisplayName,
                    portfolio.Index != null ? portfolio.Index.Code : "NULL",
                    SelectedIndex.Code,
                    portfolio.Benchmark != null ? portfolio.Benchmark.EffectiveDate.IsoFormat() : "N/A",
                    SelectedEffectiveDate);

                using (var da = DataAccessFactory.NewTransaction())
                {
                    // try to adjust last-time assigned benchmark's expiry date.
                    bool result;
                    if (portfolio.Benchmark != null && portfolio.Index != null)
                    {
                        var lastBenchmarkNewExpiryDate = SelectedEffectiveDate.AddDays(-1);
                        var lastBenchmarkOldEffectiveDate = portfolio.Benchmark.EffectiveDate;

                        if (lastBenchmarkOldEffectiveDate > lastBenchmarkNewExpiryDate)
                        {
                            // stop immediately
                            Log.InfoFormat(@"Attempt to update last-time benchmark 
which ends before it starts: new expiry date is {0}, the effective date is {1}."
                                , lastBenchmarkNewExpiryDate, lastBenchmarkOldEffectiveDate);
                            da.Rollback();
                            return SaveResult.ExpirySmallerThanEffective;
                        }

                        var lastBenchmark = portfolio.Benchmark.Clone();
                        lastBenchmark.ExpiryDate = lastBenchmarkNewExpiryDate;
                        result = da.UpdateBenchmark(portfolio.Id, lastBenchmark, portfolio.Benchmark);
                        if (!result)
                        {
                            Log.InfoFormat(@"Cannot modify last benchmark's expiry date to {0}!",
                                lastBenchmarkNewExpiryDate);
                            da.Rollback();
                            return SaveResult.CannotModifyLastEntry;
                        }
                    }

                    var insert = new Benchmark
                    {
                        EffectiveDate = SelectedEffectiveDate,
                        ExpiryDate = Constants.MaxExpiryDate,
                        Index = SelectedIndex,
                        Type = Constants.Imap,
                    };
                    result = da.InsertBenchmark(portfolio.Id, insert);
                    if (!result)
                    {
                        Log.Info(@"Cannot add the new benchmark!");
                        da.Rollback();
                        return SaveResult.CannotAddEntry;
                    }
                }
                return SaveResult.Success;
            });
        }

        public void UndoAll()
        {
            SelectedIndex = portfolio.Index != null
                ? Indexes.FirstOrDefault(b => b.Code == portfolio.Index.Code) : null;
            CanSave = false;
        }

        public void ShowHistoryEditor()
        {
            if (portfolio.Index == null)
                return;

            // initialize the vm by Initialize() instead of putting into the collections directly
            benchmarkHistoryEditor.Initialize(portfolio, Indexes, portfolio.Benchmarks);
            ViewService.ShowDialog(benchmarkHistoryEditor as ViewModelBase);
        }

        private bool CheckCanSave()
        {
            return (SelectedIndex != null) &&
                ((portfolio.Index != SelectedIndex) ||
                (portfolio.Benchmark.EffectiveDate != SelectedEffectiveDate) ||
                (portfolio.Benchmark.ExpiryDate != SelectedExpiryDate));
        }

        private enum SaveResult
        {
            Unknown,
            ExpirySmallerThanEffective,
            CannotModifyLastEntry,
            CannotAddEntry,
            Success,
        }
    }
}
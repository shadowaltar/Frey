using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Maintenance.Common.Data;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;
using Maintenance.Portfolios.Entities;
using System.Collections.Generic;
using System.Linq;
using Portfolio = Maintenance.Portfolios.Entities.Portfolio;

namespace Maintenance.Portfolios.ViewModels
{
    public class BenchmarkHistoryEditorViewModel : ViewModelBase, IBenchmarkHistoryEditorViewModel
    {
        public BenchmarkHistoryEditorViewModel(IViewService viewService)
        {
            DisplayName = "Benchmark History Editor";
            ViewService = viewService; // don't use parent's viewService; since we want to show dialogs on top of this window directly
        }

        private readonly BindableCollection<Index> indexes = new BindableCollection<Index>();
        public BindableCollection<Index> Indexes { get { return indexes; } }

        private readonly BindingList<BenchmarkViewModel> benchmarks
            = new BindingList<BenchmarkViewModel>();
        public BindingList<BenchmarkViewModel> Benchmarks { get { return benchmarks; } }

        public IDataAccessFactory<PortfolioDataAccess> DataAccessFactory { get; set; }
        public IViewService ViewService { get; set; }

        private List<Benchmark> history;
        private Portfolio portfolio;

        private BenchmarkViewModel selectedBenchmark;
        public BenchmarkViewModel SelectedBenchmark
        {
            get { return selectedBenchmark; }
            set
            {
                if (SetNotify(ref selectedBenchmark, value))
                {
                    CanAddAbove = CanAddBelow = CanDelete = value != null;
                }
            }
        }

        private bool canSave;
        public bool CanSave
        {
            get { return canSave; }
            set { SetNotify(ref canSave, value); }
        }

        private bool canUndoAll;
        public bool CanUndoAll
        {
            get { return canUndoAll; }
            set { SetNotify(ref canUndoAll, value); }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { SetNotify(ref errorMessage, value); }
        }

        private bool canAddAbove;
        public bool CanAddAbove
        {
            get { return canAddAbove; }
            set { SetNotify(ref canAddAbove, value); }
        }

        private bool canAddBelow;
        public bool CanAddBelow
        {
            get { return canAddBelow; }
            set { SetNotify(ref canAddBelow, value); }
        }

        private bool canDelete;
        public bool CanDelete
        {
            get { return canDelete; }
            set { SetNotify(ref canDelete, value); }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            ViewService.Window = view as MetroWindow;
        }

        public override void CanClose(Action<bool> callback)
        {
            base.CanClose(callback);
            Benchmarks.ListChanged -= Benchmarks_ListChanged;
        }

        public void Initialize(Portfolio ptf, IEnumerable<Index> idxs, IEnumerable<Benchmark> bmks)
        {
            Benchmarks.ListChanged -= Benchmarks_ListChanged;
            CanSave = false;
            CanUndoAll = false;
            portfolio = ptf;
            Indexes.ClearAndAddRange(idxs);

            history = new List<Benchmark>();
            Benchmarks.Clear();
            foreach (var bmk in bmks)
            {
                history.Add(bmk.Clone());
                Benchmarks.Add(new BenchmarkViewModel(bmk.Clone())); // guarantee new history also has the same (fake) id sequence here.
            }
            Benchmarks.ListChanged += Benchmarks_ListChanged;
        }

        private void Benchmarks_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                case ListChangedType.ItemAdded:
                case ListChangedType.ItemDeleted:
                    CanSave = CanUndoAll = CheckChanges();
                    break;
            }
        }

        public async void Save()
        {
            if (!CheckChanges())
            {
                await ViewService.ShowMessage("There are no changes",
                    "Nothing to be saved as no changes are detected.");
                return;
            }

            Index invalidBenchmark;
            if (!CheckValidity(out invalidBenchmark))
            {
                await ViewService.ShowError("Each benchmark effective date must be later than that of " +
                                            "the earlier benchmark." + Environment.NewLine +
                                            "Invalid benchmark is " + invalidBenchmark.Code + ".");
            }
            else
            {
                var result = await ViewService.ShowMessage("Warning",
                    "Remember you are editing the historical benchmark bindings, which the changes " +
                    "can't be undone. The applications will immediately make use of the changes.", false);

                if (result != MessageDialogResult.Affirmative)
                    return;

                var newHistory = new List<Benchmark>();

                // ignore 'to-be-deleted'; adjust all the ExpiryDate to be the last EffectiveDate-1
                Benchmark last = null;
                foreach (var relationship in Benchmarks.Select(vm => vm.Benchmark))
                {
                    if (last != null)
                    {
                        relationship.ExpiryDate = last.EffectiveDate.AddDays(-1);
                    }
                    newHistory.Add(relationship);
                    last = relationship;
                }

                // make its [0] = oldest
                if (newHistory.Count > 0)
                {
                    newHistory[0].ExpiryDate = DateTime.MaxValue.Date;
                    newHistory.Reverse();
                }
                var saveResult = await InternalSave(newHistory);
                if (saveResult)
                {
                    CanSave = false;
                    await ViewService.ShowMessage("Changes saved",
                        "The historical benchmark bindings to the portfolio \"" + portfolio.Code +
                        "\" are saved successfully.");

                    Publish<IMainViewModel>(ActivityType.ChangePortfolioToBenchmark);
                    TryClose(true);
                }
                else
                {
                    await ViewService.ShowError("Cannot save the changes of the historical benchmark relationships " +
                        "related to the portfolio \"" + portfolio.Code + "\".");
                }
            }
        }

        public void UndoAll()
        {
            Benchmarks.ListChanged -= Benchmarks_ListChanged;
            Benchmarks.Clear();
            foreach (var relationship in history)
            {
                Benchmarks.Add(new BenchmarkViewModel(relationship.Clone()));
            }
            Benchmarks.ListChanged += Benchmarks_ListChanged;
            CanSave = false;
        }

        public void AddAbove()
        {
            if (SelectedBenchmark != null)
            {
                var selectedIndex = Benchmarks.IndexOf(SelectedBenchmark);
                // effective time range [selected_eff_date+1,selected_eff_date+1]
                var r = new Benchmark
                {
                    Index = null,
                    EffectiveDate = SelectedBenchmark.EffectiveDate.AddDays(1),
                };
                r.ExpiryDate = r.EffectiveDate; // it'll be normalized in Save()
                Benchmarks.Insert(selectedIndex, new BenchmarkViewModel(r));
                SelectedBenchmark = Benchmarks[selectedIndex];

                CanSave = false; // whenever empty bmk exists, can't save.
            }
        }

        public void AddBelow()
        {
            if (SelectedBenchmark != null)
            {
                var selectedIndex = Benchmarks.IndexOf(SelectedBenchmark);
                // effective time range [selected_eff_date-1,selected_eff_date-1]
                var r = new Benchmark
                {
                    Index = null,
                    EffectiveDate = SelectedBenchmark.EffectiveDate.AddDays(-1),
                };
                r.ExpiryDate = r.EffectiveDate; // it'll be normalized in Save()
                Benchmarks.Insert(selectedIndex + 1, new BenchmarkViewModel(r));
                SelectedBenchmark = Benchmarks[selectedIndex];

                CanSave = false; // whenever empty bmk exists, can't save.
            }
        }

        public void Delete()
        {
            if (SelectedBenchmark != null)
            {
                Benchmarks.Remove(SelectedBenchmark);
            }
        }

        /// <summary>
        /// Check if there are any changes.
        /// Return true if any benchmark code or effective date doesn't
        /// match the original list of relationship, with their positions in the lists are the same.
        /// </summary>
        /// <returns></returns>
        private bool CheckChanges()
        {
            ErrorMessage = string.Empty;

            var newRelationships = Benchmarks;

            // null check for benchmarks
            if (newRelationships.Any(b => b.Index == null))
            {
                ErrorMessage = "Please assign all the missing indexes.";
                return false;
            }

            // sequence check for effective dates
            if (newRelationships.Count > 1)
            {
                for (int i = 1; i < newRelationships.Count; i++)
                {
                    var newer = newRelationships[i - 1];
                    var earlier = newRelationships[i];
                    if (newer.EffectiveDate <= earlier.EffectiveDate)
                    {
                        ErrorMessage = string.Format("The effective date of benchmark \"{0}\" must be a date " +
                                                     "later than \"{1}\".", newer.Index.Code,
                                                     earlier.EffectiveDate.IsoFormat());
                        return false;
                    }
                }
            }

            // if now with more entries, can save
            if (newRelationships.Count != history.Count)
                return true;

            // if same count of entries, compare one by one sequentially
            for (int i = 0; i < history.Count; i++)
            {
                var original = history[i];
                var now = newRelationships[i];
                // ensure no null bmk
                if (now.Index == null)
                    return false;
                // check if any difference
                if (original.Index.Code != now.Index.Code || original.EffectiveDate != now.EffectiveDate)
                    return true;
            }
            return false;
        }

        private Task<bool> InternalSave(IEnumerable<Benchmark> newHistory)
        {
            return TaskEx.Run(() =>
            {
                using (var access = DataAccessFactory.NewTransaction())
                {
                    try
                    {
                        List<Benchmark> added;
                        List<Benchmark> removed;
                        List<Benchmark> modified;
                        List<Benchmark> modifiedOriginal;
                        FindModifiedBenchmarks(history, newHistory.ToList(),
                            out added, out removed, out modified, out modifiedOriginal);

                        foreach (var bmk in removed)
                        {
                            if (!access.DeleteBenchmark(portfolio.Id, bmk))
                            {
                                access.Rollback();
                                return false;
                            }
                        }
                        foreach (var bmk in added)
                        {
                            if (!access.InsertBenchmark(portfolio.Id, bmk))
                            {
                                access.Rollback();
                                return false;
                            }
                        }
                        // assume modified & modifiedOriginal have the same id sequence.
                        for (int i = 0; i < modified.Count; i++)
                        {
                            var original = modifiedOriginal[i];
                            var current = modified[i];
                            if (!access.UpdateBenchmark(portfolio.Id, current, original))
                            {
                                access.Rollback();
                                return false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Error occurred when saving the benchmark history.", e);
                        access.Rollback();
                        return false;
                    }
                    return true;
                }
            });
        }

        private void FindModifiedBenchmarks(List<Benchmark> original, List<Benchmark> current,
            out List<Benchmark> added,
            out List<Benchmark> removed,
            out List<Benchmark> modified, out List<Benchmark> modifiedOriginal)
        {
            // use the fake id to search for new and deleted bmks.
            added = current.Where(b => b.Id == 0).ToList();
            removed = original.Where(ob => current.FirstOrDefault(cb => ob.Id == cb.Id) == null).ToList();
            modifiedOriginal = original.Where(ob =>
            {
                var cb = current.FirstOrDefault(c => ob.Id == c.Id);
                if (cb == null)
                    return false;
                if (cb.Index != ob.Index || cb.EffectiveDate != ob.EffectiveDate)
                    return true;
                return false;
            }).OrderBy(b => b.Id).ToList();
            var modifiedIds = modifiedOriginal.Select(mb => mb.Id).ToList(); // can't use modifiedOriginal in anonymous method
            modified = current.Where(cb => modifiedIds.Contains(cb.Id)).OrderBy(b => b.Id).ToList();
        }

        private bool CheckValidity(out Index invalidBenchmark)
        {
            invalidBenchmark = null;
            Benchmark moreRecent = null;
            foreach (var relationship in Benchmarks.Select(vm => vm.Benchmark))
            {
                if (moreRecent != null && moreRecent.EffectiveDate <= relationship.EffectiveDate)
                {
                    invalidBenchmark = moreRecent.Index;
                    return false;
                }
                moreRecent = relationship;
            }
            return true;
        }
    }
}
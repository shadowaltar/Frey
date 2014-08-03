using System.Collections;
using System.Collections.Generic;
using Caliburn.Micro;
using Maintenance.Common;
using Maintenance.Common.Data;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;

namespace Maintenance.Benchmarks.ViewModels
{
    public partial class MainViewModel
    {
        public IViewService ViewService { get; set; }
        public IDataAccessFactory DataAccessFactory { get; set; }

        private string environment = Settings.DefaultEnvironment;

        private readonly Dictionary<long, Benchmark> allBenchmarks = new Dictionary<long, Benchmark>();
        private readonly Dictionary<long, long> benchmarkToIndexMappings = new Dictionary<long, long>();
        private readonly Dictionary<long, Index> allIndexes = new Dictionary<long, Index>();

        private readonly BindableCollection<Benchmark> benchmarks = new BindableCollection<Benchmark>();
        public BindableCollection<Benchmark> Benchmarks { get { return benchmarks; } }

        public IAddFlyoutViewModel AddFlyout { get; private set; }
        public IEditFlyoutViewModel EditFlyout { get; private set; }
        public IFilterFlyoutViewModel FilterFlyout { get; private set; }
        public IOptionsFlyoutViewModel OptionsFlyout { get; private set; }

        private readonly IBenchmarkDependencyReportViewModel dependencyReport;

        private List<string> availableBenchmarkTypes = new List<string> { "INDEX" };

        private IList selectedItems;
        private bool isSingleSelection;

        private Benchmark selectedBenchmark;
        public Benchmark SelectedBenchmark
        {
            get { return selectedBenchmark; }
            set
            {
                if (SetNotify(ref selectedBenchmark, value) && value != null)
                {
                    if (CanToggleEdit && IsEditFlyoutOpen)
                        ToggleEdit();
                    CheckEnabled();
                }
            }
        }

        private bool canToggleAdd;
        public bool CanToggleAdd
        {
            get { return canToggleAdd; }
            set { SetNotify(ref canToggleAdd, value); }
        }

        private bool canToggleEdit;
        public bool CanToggleEdit
        {
            get { return canToggleEdit; }
            set { SetNotify(ref canToggleEdit, value); }
        }

        private bool canToggleFilter;
        public bool CanToggleFilter
        {
            get { return canToggleFilter; }
            set { SetNotify(ref canToggleFilter, value); }
        }

        private bool canDelete;
        public bool CanDelete
        {
            get { return canDelete; }
            set { SetNotify(ref canDelete, value); }
        }

        private bool canSelectBenchmarks;
        public bool CanSelectBenchmarks
        {
            get { return canSelectBenchmarks; }
            set { SetNotify(ref canSelectBenchmarks, value); }
        }

        private bool isAddFlyoutOpen;
        public bool IsAddFlyoutOpen
        {
            get { return isAddFlyoutOpen; }
            set { SetNotify(ref isAddFlyoutOpen, value); }
        }

        private bool isEditFlyoutOpen;
        public bool IsEditFlyoutOpen
        {
            get { return isEditFlyoutOpen; }
            set { SetNotify(ref isEditFlyoutOpen, value); }
        }

        private bool isFilterFlyoutOpen;
        public bool IsFilterFlyoutOpen
        {
            get { return isFilterFlyoutOpen; }
            set { SetNotify(ref isFilterFlyoutOpen, value); }
        }

        private bool isOptionsFlyoutOpen;
        public bool IsOptionsFlyoutOpen
        {
            get { return isOptionsFlyoutOpen; }
            set { SetNotify(ref isOptionsFlyoutOpen, value); }
        }
    }
}
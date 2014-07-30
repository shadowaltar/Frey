using Caliburn.Micro;
using Maintenance.Common;
using Maintenance.Grits.Entities;
using System.Collections.Generic;

namespace Maintenance.Grits.ViewModels
{
    public partial class MainViewModel
    {
        public IAddFlyoutViewModel AddFlyout { get; private set; }
        public IEditFundFlyoutViewModel EditFundFlyout { get; private set; }
        public IEditBenchmarkFlyoutViewModel EditBenchmarkFlyout { get; private set; }
        public IOptionsFlyoutViewModel OptionsFlyout { get; private set; }

        private List<Benchmark> allBenchmarks = new List<Benchmark>();
        private List<GritsBenchmark> allGritsBenchmarks = new List<GritsBenchmark>();

        private List<Fund> allFunds = new List<Fund>();
        private List<GritsFund> allGritsFunds = new List<GritsFund>();

        private readonly Dictionary<int, int> fundToBenchmark = new Dictionary<int, int>();

        private readonly BindableCollection<GritsFund> funds = new BindableCollection<GritsFund>();
        public BindableCollection<GritsFund> Funds { get { return funds; } }

        private readonly BindableCollection<GritsBenchmark> benchmarks = new BindableCollection<GritsBenchmark>();
        public BindableCollection<GritsBenchmark> Benchmarks { get { return benchmarks; } }

        private bool isBelongsToVisible;
        public bool IsBelongsToVisible
        {
            get { return isBelongsToVisible; }
            set { SetNotify(ref isBelongsToVisible, value); }
        }

        private bool isAddFlyoutOpen;
        public bool IsAddFlyoutOpen
        {
            get { return isAddFlyoutOpen; }
            set { SetNotify(ref isAddFlyoutOpen, value); }
        }

        private bool isEditFundFlyoutOpen;
        public bool IsEditFundFlyoutOpen
        {
            get { return isEditFundFlyoutOpen; }
            set { SetNotify(ref isEditFundFlyoutOpen, value); }
        }

        private bool isEditBenchmarkFlyoutOpen;
        public bool IsEditBenchmarkFlyoutOpen
        {
            get { return isEditBenchmarkFlyoutOpen; }
            set { SetNotify(ref isEditBenchmarkFlyoutOpen, value); }
        }

        private bool isOptionsFlyoutOpen;
        public bool IsOptionsFlyoutOpen
        {
            get { return isOptionsFlyoutOpen; }
            set { SetNotify(ref isOptionsFlyoutOpen, value); }
        }

        private bool canSelectItems;
        public bool CanSelectItems
        {
            get { return canSelectItems; }
            set { SetNotify(ref canSelectItems, value); }
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

        private GritsFund selectedFund;

        public GritsFund SelectedFund
        {
            get { return selectedFund; }
            set
            {
                if (SetNotify(ref selectedFund, value) && value != null)
                {
                    CanToggleEdit = true;
                    SelectedBenchmark = null;
                }
            }
        }

        private GritsBenchmark selectedBenchmark;
        public GritsBenchmark SelectedBenchmark
        {
            get { return selectedBenchmark; }
            set
            {
                if (SetNotify(ref selectedBenchmark, value) && value != null)
                {
                    CanToggleEdit = true;
                    SelectedFund = null;
                }
            }
        }
    }
}
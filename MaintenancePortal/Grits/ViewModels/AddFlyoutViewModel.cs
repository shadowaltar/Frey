using Caliburn.Micro;
using MahApps.Metro.Controls.Dialogs;
using Maintenance.Common.Data;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;
using Maintenance.Grits.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maintenance.Grits.ViewModels
{
    public class AddFlyoutViewModel : ViewModelBase, IAddFlyoutViewModel
    {
        public AddFlyoutViewModel()
        {
            GritsModes.Add(GritsMode.Both);
            GritsModes.Add(GritsMode.BBU);
            GritsModes.Add(GritsMode.Enterprise);
        }

        public IDataAccessFactory<GritsDataAccess> DataAccessFactory { get; set; }

        public IViewService ViewService { get; set; }

        public bool IsReady { get; set; }

        private bool canSave;
        public bool CanSave
        {
            get { return canSave; }
            set { SetNotify(ref canSave, value); }
        }

        private GritsMode? selectedGritsMode;
        public GritsMode? SelectedGritsMode
        {
            get { return selectedGritsMode; }
            set
            {
                if (SetNotify(ref selectedGritsMode, value))
                {
                    CheckCanSave();
                }
            }
        }

        private Fund selectedFund;
        public Fund SelectedFund
        {
            get { return selectedFund; }
            set
            {
                if (SetNotify(ref selectedFund, value))
                {
                    CheckCanSave();
                    if (value != null)
                    {
                        SelectedBenchmark = value.Benchmark;
                    }
                }
            }
        }

        private Benchmark selectedBenchmark;
        public Benchmark SelectedBenchmark
        {
            get { return selectedBenchmark; }
            set
            {
                if (SetNotify(ref selectedBenchmark, value))
                {
                    CheckCanSave();
                }
            }
        }

        private bool addBenchmark;
        public bool AddBenchmark
        {
            get { return addBenchmark; }
            set
            {
                if (SetNotify(ref addBenchmark, value))
                {
                    CheckCanSave();
                }
            }
        }

        private bool isBenchmarkLoadedAtNight;
        public bool IsBenchmarkLoadedAtNight
        {
            get { return isBenchmarkLoadedAtNight; }
            set { SetNotify(ref isBenchmarkLoadedAtNight, value); }
        }

        private readonly BindableCollection<Fund> funds = new BindableCollection<Fund>();
        public BindableCollection<Fund> Funds { get { return funds; } }

        private readonly BindableCollection<Benchmark> benchmarks = new BindableCollection<Benchmark>();
        public BindableCollection<Benchmark> Benchmarks { get { return benchmarks; } }

        private readonly BindableCollection<GritsMode> gritsModes = new BindableCollection<GritsMode>();
        public BindableCollection<GritsMode> GritsModes { get { return gritsModes; } }

        public List<GritsFund> ExistingGritsFunds { get; set; }
        public List<GritsBenchmark> ExistingGritsBenchmarks { get; set; }

        public async void Save()
        {
            if (SelectedGritsMode == null)
                return;

            var decision = await ViewService.AskSave();

            if (decision != MessageDialogResult.Affirmative)
                return;

            var saveResult = true;
            try
            {
                await SaveFund();
            }
            catch (Exception e)
            {
                Log.Error("Error occurs when editing data to database.", e);
                saveResult = false;
            }
            if (saveResult)
            {
                // disable save button if success
                CanSave = false;

                await ViewService.ShowMessage("New Fund Added",
                    "The new GRITS fund \"" + SelectedFund + "\" is added successfully.");

                if (!AddBenchmark)
                    CleanAll();
            }
            else
            {
                await ViewService.ShowError("The fund \"" + SelectedFund
                    + "\" cannot be added to GRITS loader.");
            }

            if (AddBenchmark)
            {
                try
                {
                    saveResult = await SaveBenchmark();
                }
                catch (Exception)
                {
                    saveResult = false;
                }
                if (saveResult)
                {
                    // disable save button if success
                    CanSave = false;

                    await ViewService.ShowMessage("New benchmark added",
                        "The new GRITS benchmark \"" + SelectedBenchmark + "\" is added successfully.");

                    CleanAll();
                }
                else
                {
                    await ViewService.ShowError("The benchmark \"" + SelectedBenchmark
                        + "\" cannot be added to GRITS loader.");
                }
            }

            Publish<IMainViewModel>(ActivityType.Add);
        }

        private Task<bool> SaveFund()
        {
            return Task.Factory.StartNew(() =>
            {
                if (SelectedGritsMode == null)
                    throw new InvalidOperationException("You must select a mode.");
                using (var da = DataAccessFactory.New())
                {
                    return da.AddFund(SelectedFund.Code, (GritsMode)SelectedGritsMode);
                }
            });
        }

        private Task<bool> SaveBenchmark()
        {
            return Task.Factory.StartNew(() =>
            {
                if (SelectedGritsMode == null)
                    throw new InvalidOperationException("You must select a mode.");
                using (var da = DataAccessFactory.New())
                {
                    return da.AddBenchmark(SelectedBenchmark.Code, (GritsMode)SelectedGritsMode, IsBenchmarkLoadedAtNight);
                }
            });
        }

        public void CleanAll()
        {
            SelectedGritsMode = null;
            SelectedFund = null;
            SelectedBenchmark = null;
            AddBenchmark = false;
            IsBenchmarkLoadedAtNight = false;
        }

        private void CheckCanSave()
        {
            var result = false;
            if (SelectedGritsMode != null && SelectedFund != null)
            {
                if (!AddBenchmark || (AddBenchmark && SelectedBenchmark != null && !CheckGritsBenchmarkExists()))
                {
                    result = true;
                }
            }
            CanSave = result && !CheckGritsFundExists();
        }

        private bool CheckGritsFundExists()
        {
            return ExistingGritsFunds.Any(f => f.Code == SelectedFund.Code);
        }

        private bool CheckGritsBenchmarkExists()
        {
            return ExistingGritsBenchmarks.Any(b => b.Code == SelectedBenchmark.Code);
        }
    }
}
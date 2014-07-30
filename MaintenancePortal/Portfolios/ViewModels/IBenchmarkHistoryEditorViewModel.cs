using System;
using System.Collections.Generic;
using System.ComponentModel;
using Caliburn.Micro;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Maintenance.Portfolios.Entities;
using Portfolio = Maintenance.Portfolios.Entities.Portfolio;

namespace Maintenance.Portfolios.ViewModels
{
    public interface IBenchmarkHistoryEditorViewModel : IHasDataAccessFactory<PortfolioDataAccess>,
        IHasViewService
    {
        BindableCollection<Index> Indexes { get; }
        BindingList<BenchmarkViewModel> Benchmarks { get; }
        BenchmarkViewModel SelectedBenchmark { get; set; }
        bool CanSave { get; set; }
        bool CanAddAbove { get; set; }
        bool CanAddBelow { get; set; }
        bool CanDelete { get; set; }

        void CanClose(Action<bool> callback);

        void Initialize(Portfolio ptf, IEnumerable<Index> idxs,
            IEnumerable<Benchmark> bmks);

        void Save();
        void UndoAll();
        void AddAbove();
        void AddBelow();
        void Delete();
    }
}
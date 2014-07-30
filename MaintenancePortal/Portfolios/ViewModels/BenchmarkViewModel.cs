using System;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;
using Maintenance.Portfolios.Entities;

namespace Maintenance.Portfolios.ViewModels
{
    public class BenchmarkViewModel : ViewModelBaseSlim
    {
        public Benchmark Benchmark { get; private set; }

        public BenchmarkViewModel(Benchmark benchmark)
        {
            benchmark.ThrowIfNull("benchmark", "Must provide a benchmark to create the vm.");
            Benchmark = benchmark;
        }

        public Index Index
        {
            get { return Benchmark.Index; }
            set
            {
                if (value == Benchmark.Index) return;
                Benchmark.Index = value;
                Notify();
            }
        }

        public DateTime EffectiveDate
        {
            get { return Benchmark.EffectiveDate; }
            set
            {
                if (value == Benchmark.EffectiveDate) return;
                Benchmark.EffectiveDate = value;
                Notify();
            }
        }

        public override string ToString()
        {
            return Benchmark.ToString();
        }
    }
}
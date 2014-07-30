using System;
using System.Collections.Generic;
using System.Linq;
using Maintenance.Common;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;

namespace Maintenance.Portfolios.Entities
{
    public class Portfolio : Common.Entities.Portfolio
    {
        public Portfolio()
        {
            Benchmarks = new List<Benchmark>();
            AsOfInstruments = new HashSet<Instrument>();
        }

        private string assetClassFocusesAsOfInstruments;
        private string assetClassFocusAsOfPortfolio;

        public HashSet<Instrument> AsOfInstruments { get; private set; }

        /// <summary>
        /// Get the assigned benchmark. It is a reference inside the <see cref="Benchmark"/>.
        /// Attempt to set this property will throw <exception cref="InvalidOperationException" />.
        /// </summary>
        public override Index Index
        {
            get
            {
                return Benchmark != null ? Benchmark.Index : null;
            }
            set
            {
                throw new InvalidOperationException("Please add benchmark via BenchmarkRelationships.");
            }
        }

        /// <summary>
        /// Get the relationship with a benchmark.
        /// It always fetches the 1st item in the relationship list of history, if any exists;
        /// else it returns null.
        /// </summary>
        public Benchmark Benchmark
        {
            get
            {
                return Benchmarks.IsNullOrEmpty() ? null : Benchmarks[0]; // latest
            }
        }

        public string BenchmarkEffectiveDate
        {
            get
            {
                if (Benchmark == null)
                    return string.Empty;
                return Benchmark.EffectiveDate.IsoFormat();
            }
        }

        /// <summary>
        /// Get the whole list of benchmark relationship history.
        /// </summary>
        public List<Benchmark> Benchmarks { get; private set; }

        public string IndexCode { get { return Index != null ? Index.Code : string.Empty; } }

        public string PortfolioManagerName { get { return PortfolioManager != null ? PortfolioManager.Name : string.Empty; } }

        public string AssetClassFocusesAsOfInstruments { get { return assetClassFocusesAsOfInstruments; } }

        public string AssetClassFocusAsOfPortfolio { get { return assetClassFocusAsOfPortfolio; } }

        public void GenerateAssetClassFocuses(PortfolioExtendedInfo info)
        {
            // override instrument-level focus by ext-info's focus, if any
            if (info != null && !string.IsNullOrEmpty(info.AssetClassFocus))
            {
                assetClassFocusAsOfPortfolio = info.AssetClassFocus;
            }
            else
            {
                assetClassFocusAsOfPortfolio = Texts.NotSet;
            }

            if (AsOfInstruments.Count == 0)
            {
                assetClassFocusesAsOfInstruments = Texts.NotSet;
            }
            else
            {
                foreach (var focus in AsOfInstruments.Select(i => i.AssetClassFocus).Distinct()
                    .OrderBy(i => i))
                {
                    assetClassFocusesAsOfInstruments += focus + "/";
                }
                assetClassFocusesAsOfInstruments = assetClassFocusesAsOfInstruments.Trim('/');
            }
        }

        public override string ToString()
        {
            return Code + ", " + Name;
        }
    }
}
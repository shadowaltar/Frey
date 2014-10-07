using Maintenance.Common.Data;
using Maintenance.Grits.Entities;
using Maintenance.Grits.Utils;
using System.Data;

namespace Maintenance.Grits
{
    public class GritsDataAccess : FmsDataAccess
    {
        public DataTable GetAllFunds()
        {
            return Query("SELECT * FROM generic..Fund WHERE Active = 'A'");
        }

        public DataTable GetGritsFunds()
        {
            return Query("SELECT * FROM generic..GritsFund gf JOIN generic..Fund f ON f.FundCode = gf.FundCode");
        }

        public DataTable GetAllBenchmarks()
        {
            return Query("SELECT * FROM cloning..BenchMark WHERE Active = 'A'");
        }

        public DataTable GetGritsBenchmarks()
        {
            return Query("SELECT * FROM generic..GritsBenchmark gb JOIN cloning..BenchMark b ON b.BmCode = gb.BmCode");
        }

        public bool AddFund(string fundCode, GritsMode mode)
        {
            var rowCount = Execute(string.Format(@"INSERT INTO generic..GritsFund 
(FundCode, Mode, Enabled, LastModified, UserId) 
VALUES ('{0}', '{1}', 0, getdate(), suser_id())", fundCode, Helper.GetModeSymbol(mode)));
            return rowCount > 0;
        }

        public bool DisableFund(string fundCode)
        {
            return ChangeFundMode(fundCode, GritsMode.Disabled);
        }

        public bool ChangeFundMode(string fundCode, GritsMode mode)
        {
            var rowCount = Execute(string.Format(@"UPDATE generic..GritsFund SET Mode = '{0}' WHERE FundCode = '{1}'",
                Helper.GetModeSymbol(mode), fundCode));
            return rowCount > 0;
        }

        public bool AddBenchmark(string code, GritsMode mode, bool isLoadedAtNight = false)
        {
            var rowCount = Execute(string.Format(@"INSERT INTO generic..GritsBenchmark 
(BmCode, Mode, IsLate, Enabled, LastModified, UserId) 
VALUES ('{0}', '{1}', {2}, 0, getdate(), suser_id())"
                , code, Helper.GetModeSymbol(mode), isLoadedAtNight ? 1 : 0));
            return rowCount > 0;
        }

        public bool DisableBenchmark(string benchmarkCode)
        {
            return ChangeBenchmarkSettings(benchmarkCode, GritsMode.Disabled);
        }

        public bool ChangeBenchmarkSettings(string benchmarkCode,
            GritsMode mode, bool? isLoadedAtNight = null)
        {
            if (isLoadedAtNight == null)
            {
                var rowCount = Execute(string.Format(@"UPDATE generic..GritsBenchmark 
SET Mode = '{0}' WHERE BmCode = '{1}'",
                    Helper.GetModeSymbol(mode), benchmarkCode));
                return rowCount > 0;
            }
            else
            {
                var rowCount = Execute(string.Format(@"UPDATE generic..GritsBenchmark 
SET Mode = '{0}', IsLate = {1} WHERE BmCode = '{2}'"
                    , Helper.GetModeSymbol(mode), (bool)isLoadedAtNight ? 1 : 0, benchmarkCode));
                return rowCount > 0;
            }
        }
    }
}
using System.Data;
using Maintenance.Common.Data;

namespace Maintenance.Benchmarks
{
    public static class BenchmarkQueries
    {
        public static DataTable GetBenchmarks(this DataAccess access)
        {
            return access.Query(@"SELECT
B.BENCHMARK_ID ID, B.BENCHMARK_CODE CODE, 
B.BENCHMARK_TYPE_CODE TYPE, INDEX_ID IID
FROM BENCHMARK B");
        }

        public static DataTable GetIndexes(this DataAccess access)
        {
            return access.Query(@"SELECT
INDEX_ID ID, INDEX_SHORT_NAME CODE,
INDEX_LONG_NAME NAME
FROM INDEX_BASIC");
        }

        public static DataTable GetBenchmarkDependencies(this DataAccess access, string code)
        {
            return access.Query(@"SELECT 'Portfolio' AS TYPE,
  P.PORTFOLIO_SHORT_NAME AS CODE,
  P.PORTFOLIO_LONG_NAME  AS NAME
FROM PORTFOLIO_BMK_RELATIONSHIP PBR
JOIN BENCHMARK B
  ON PBR.BENCHMARK_ID = B.BENCHMARK_ID
JOIN PORTFOLIO P
  ON P.PORTFOLIO_ID = PBR.PORTFOLIO_ID
WHERE B.BENCHMARK_CODE = '{0}'
UNION
SELECT 'CompositeBenchmark' AS TYPE,
  TO_CHAR(CBI.COMPOSITE_BENCHMARK_ID) AS CODE,
  TO_CHAR(CBI.COMPOSITE_BENCHMARK_ID) AS NAME
FROM COMPOSITE_BENCHMARK_ITEM CBI
JOIN BENCHMARK B
  ON CBI.BENCHMARK_ID = B.BENCHMARK_ID
WHERE B.BENCHMARK_CODE = '{0}'", code);
        }

        public static bool AddIndexBenchmark(this DataAccess access, string code, long indexId)
        {
            var rowCount = access.Execute(@"INSERT INTO IMAP_COMMON.BENCHMARK
(BENCHMARK_ID, BENCHMARK_CODE, BENCHMARK_TYPE_CODE, INDEX_ID)
VALUES
(IMAP_COMMON.SEQ_BMK_ID.NEXTVAL, '{0}', 'INDEX', {1})", code, indexId);
            return rowCount > 0;
        }

        public static bool EditBenchmark(this DataAccess access, long id, string code, string newCode, long newIndexId)
        {
            var rowCount = access.Execute(@"UPDATE IMAP_COMMON.BENCHMARK
SET BENCHMARK_CODE = '{0}', BENCHMARK_TYPE_CODE = '{1}', INDEX_ID = {2}
WHERE BENCHMARK_ID = {3} AND BENCHMARK_CODE = '{4}'", newCode, "INDEX", newIndexId, id, code);
            return rowCount > 0;
        }

        public static bool DeleteBenchmark(this DataAccess access, string code)
        {
            var rowCount = access.Execute(@"DELETE IMAP_COMMON.BENCHMARK WHERE BENCHMARK_CODE = '{0}'", code);
            return rowCount > 0;
        }
    }
}
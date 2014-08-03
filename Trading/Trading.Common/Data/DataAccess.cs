namespace Trading.Common.Data
{
    public abstract class DataAccess
    {
        private static int sqlCount;
        private static readonly object sqlCountSyncRoot = new object();
        private static int tranxCount;
        private static readonly object tranxCountSyncRoot = new object();

        protected int currentTransactionNumber;
        protected int currentSqlNumber;

        /// <summary>
        /// Get the count of sql statements which has been executed after the application starts.
        /// It is a global counter in the process.
        /// </summary>
        /// <returns></returns>
        protected int GetSqlCount()
        {
            int result;
            lock (sqlCountSyncRoot)
            {
                sqlCount++;
                result = sqlCount;
            }
            return result;
        }

        /// <summary>
        /// Get the count of sql transactions which has been initialized after the application starts.
        /// It is a global counter in the process.
        /// </summary>
        /// <returns></returns>
        protected int GetTransactionCount()
        {
            int result;
            lock (tranxCountSyncRoot)
            {
                tranxCount++;
                result = tranxCount;
            }
            return result;
        }

        public abstract void OpenTransaction();

        public abstract void Rollback();

    }
}
using System.Collections.Generic;
using Trading.Common.Entities;

namespace Trading.Common.Data
{
    public class DataCache : IDataCache
    {
        private readonly object securitySyncRoot = new object();
        private readonly Dictionary<long, Security> securityCache = new Dictionary<long, Security>();
        public Dictionary<long, Security> SecurityCache { get { lock (securitySyncRoot) return securityCache; } }

        public void InvalidateCache()
        {
            lock (securitySyncRoot)
                SecurityCache.Clear();
        }
    }

    public interface IDataCache
    {
        Dictionary<long, Security> SecurityCache { get; }
        void InvalidateCache();
    }
}
using Automata.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Automata.Mechanisms
{
    public abstract class TradingContext
    {
        protected readonly ReaderWriterLockSlim PriceHistoryDataLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        protected readonly ReaderWriterLockSlim OrderHistoryDataLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        protected readonly ReaderWriterLockSlim TradeHistoryDataLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private readonly List<Price> priceHistory = new List<Price>(1000000);
        private readonly List<Order> orderHistory = new List<Order>(50000);
        private readonly List<Trade> tradeHistory = new List<Trade>(10000);

        public List<Price> PriceHistory
        {
            get
            {
                try
                {
                    PriceHistoryDataLock.EnterReadLock();
                    return priceHistory;
                }
                finally
                {
                    PriceHistoryDataLock.ExitReadLock();
                }
            }
        }

        public List<Order> OrderHistory
        {
            get
            {
                try
                {
                    OrderHistoryDataLock.EnterReadLock();
                    return orderHistory;
                }
                finally
                {
                    OrderHistoryDataLock.ExitReadLock();
                }
            }
        }

        public List<Trade> TradeHistory
        {
            get
            {
                try
                {
                    TradeHistoryDataLock.EnterReadLock();
                    return tradeHistory;
                }
                finally
                {
                    TradeHistoryDataLock.ExitReadLock();
                }
            }
        }

        protected void SavePricesToHistory(IEnumerable<Price> prices)
        {
            try
            {
                PriceHistoryDataLock.EnterWriteLock();
                foreach (var price in prices.OrderBy(p => p.Time).ThenBy(p => p.Security.Code))
                {
                    priceHistory.Add(price);
                }
            }
            finally
            {
                PriceHistoryDataLock.ExitWriteLock();
            }
        }

        protected void SaveOrdersToHistory(IEnumerable<Order> orders)
        {
            try
            {
                OrderHistoryDataLock.EnterWriteLock();
                orderHistory.AddRange(orders);
            }
            finally
            {
                OrderHistoryDataLock.ExitWriteLock();
            }
        }

        protected void SaveTradesToHistory(IEnumerable<Trade> trades)
        {
            try
            {
                TradeHistoryDataLock.EnterWriteLock();
                tradeHistory.AddRange(trades);
            }
            finally
            {
                TradeHistoryDataLock.ExitWriteLock();
            }
        }

        protected abstract void Trade();
    }
}

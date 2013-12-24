using Automata.Entities;
using Automata.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Automata.Mechanisms
{
    public abstract class TradingContext
    {
        protected readonly ReaderWriterLockSlim priceHistoryDataLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        protected readonly ReaderWriterLockSlim orderHistoryDataLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        protected readonly ReaderWriterLockSlim tradeHistoryDataLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private readonly List<Price> priceHistory = new List<Price>(1000000);
        private readonly List<Order> orderHistory = new List<Order>(50000);
        private readonly List<Trade> tradeHistory = new List<Trade>(10000);

        public virtual List<Price> PriceHistory
        {
            get
            {
                try
                {
                    priceHistoryDataLock.EnterReadLock();
                    return priceHistory;
                }
                finally
                {
                    priceHistoryDataLock.ExitReadLock();
                }
            }
        }

        public virtual List<Order> OrderHistory
        {
            get
            {
                try
                {
                    orderHistoryDataLock.EnterReadLock();
                    return orderHistory;
                }
                finally
                {
                    orderHistoryDataLock.ExitReadLock();
                }
            }
        }

        public virtual List<Trade> TradeHistory
        {
            get
            {
                try
                {
                    tradeHistoryDataLock.EnterReadLock();
                    return tradeHistory;
                }
                finally
                {
                    tradeHistoryDataLock.ExitReadLock();
                }
            }
        }

        protected virtual void SavePricesToHistory(IEnumerable<Price> prices)
        {
            try
            {
                priceHistoryDataLock.EnterWriteLock();
                if (prices.IsNullOrEmpty())
                    return;

                foreach (var price in prices.OrderBy(p => p.Time).ThenBy(p => p.Security.Code))
                {
                    priceHistory.Add(price);
                }
            }
            finally
            {
                priceHistoryDataLock.ExitWriteLock();
            }
        }

        protected virtual void SaveOrdersToHistory(IEnumerable<Order> orders)
        {
            try
            {
                orderHistoryDataLock.EnterWriteLock();
                if (orders.IsNullOrEmpty())
                    return;

                orderHistory.AddRange(orders);
            }
            finally
            {
                orderHistoryDataLock.ExitWriteLock();
            }
        }

        protected virtual void SaveTradesToHistory(IEnumerable<Trade> trades)
        {
            try
            {
                tradeHistoryDataLock.EnterWriteLock();
                if (trades.IsNullOrEmpty())
                    return;

                tradeHistory.AddRange(trades);
            }
            finally
            {
                tradeHistoryDataLock.ExitWriteLock();
            }
        }

        protected abstract void Trade();
    }
}

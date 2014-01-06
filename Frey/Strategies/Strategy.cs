using System.Linq;
using Automata.Entities;
using Automata.Mechanisms;
using System;
using System.Collections.Generic;

namespace Automata.Strategies
{
    public abstract class Strategy
    {
        public virtual TimeScale TimeScale { get { return TimeScale.Daily; } }

        public abstract bool IsTimeToStop { get; protected set; }
        public ITradingScope TradingScope { get; set; }

        public virtual void Initialize()
        {
        }

        public abstract List<Order> GenerateOrders(HashSet<Price> prices, Portfolio portfolio, DateTime orderTime);

        public virtual bool CheckIfStopTrading(IEnumerable<Price> prices, Portfolio portfolio, DateTime orderTime, out List<Order> exitOrders)
        {
            // check if meets stop trading criteria
            if (prices.Any(p => p.Time == TradingScope.End))
            {
                IsTimeToStop = true;
                // generate 'all close' orders
                exitOrders = portfolio.Select(ep => Order.CreateToClose(ep, orderTime)).ToList();
                return true;
            }
            exitOrders = null;
            return false;
        }
    }
}
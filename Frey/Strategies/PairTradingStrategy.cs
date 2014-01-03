using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automata.Core.Exceptions;
using Automata.Entities;
using Automata.Mechanisms;

namespace Automata.Strategies
{
    public class PairTradingStrategy : Strategy
    {
        public override bool IsTimeToStop { get; protected set; }

        public override void Initialize()
        {
            if (TradingScope == null)
                throw new InvalidStrategyBehaviorException();
            if (TradingScope.Securities.Count != 2)
                throw new InvalidStrategyBehaviorException();
        }

        public override List<Order> GenerateOrders(HashSet<Price> prices, Portfolio portfolio, DateTime orderTime)
        {
            throw new NotImplementedException();
        }

        protected override double ComputeQuantity(Portfolio portfolio, Security security, Price referencePrice)
        {
            throw new NotImplementedException();
        }
    }
}

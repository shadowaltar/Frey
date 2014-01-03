using System;
using System.Collections.Generic;
using Automata.Core.Exceptions;

namespace Automata.Mechanisms
{
    public class Portfolio : List<Position>
    {
        public CashPosition CashPosition { get; set; }

        private readonly List<PortfolioState> states = new List<PortfolioState>();
        public List<PortfolioState> States { get { return states; } }

        public void ClosePosition(Trade trade)
        {
            double balanceChange;
            double drawDown = Math.Min(0, trade.Profit);
            if (trade.ExitOrder.Side == Side.Long)
                balanceChange = -trade.ExitEquity;
            else if (trade.ExitOrder.Side == Side.Short)
                balanceChange = trade.ExitEquity;
            else
                throw new InvalidOrderBehaviorException();


            var ps = new PortfolioState(drawDown, balanceChange,
                trade.ExecutionTime, PortfolioAction.ClosePosition);
            States.Add(ps);

            CashPosition.Add(balanceChange);
            Remove(trade.Position);
        }

        public void EnterPosition(Position position)
        {
            double balanceChange;

            if (position.Order.Side == Side.Long)
                balanceChange = -position.Value;
            else if (position.Order.Side == Side.Short)
                balanceChange = position.Value;
            else
                throw new InvalidOrderBehaviorException();

            var ps = new PortfolioState(0, balanceChange,
                position.ExecutionTime, PortfolioAction.EnterPosition);
            States.Add(ps);

            CashPosition.Add(balanceChange);
            Add(position);
        }
    }
}
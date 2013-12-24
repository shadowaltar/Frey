using System;
using Automata.Entities;
using Automata.Core;

namespace Automata.Mechanisms
{
    public class Position
    {
        public Position(Order order, double actualPrice, double actualQuantity, DateTime executionTime)
        {
            Order = order;
            ActualEntryPrice = actualPrice;
            ActualQuantity = actualQuantity;
            ExecutionTime = executionTime;
        }

        public Security Security { get { return Order.Security; } }
        public Side Side { get { return Order.Side; } }

        public double ActualEntryPrice { get; private set; }
        public double ActualQuantity { get; private set; }

        public DateTime ExecutionTime { get; set; }

        public Order Order { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}: {3} x{4}",
                Utilities.BracketTime(ExecutionTime), Security.Code, Side, ActualEntryPrice, ActualQuantity);
        }
    }
}
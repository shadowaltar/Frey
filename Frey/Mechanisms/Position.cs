using Automata.Core;
using Automata.Entities;
using System;

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

        public double TransactionCost { get; set; }

        public DateTime ExecutionTime { get; set; }

        public Order Order { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}: {3}x{4}",
                ExecutionTime.PrintBracket(), Security.Code, Side, ActualEntryPrice, ActualQuantity);
        }
    }
}
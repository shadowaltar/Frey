using System;
using Automata.Entities;

namespace Automata.Mechanisms
{
    public class Position
    {
        public Position(Order order, double actualPrice, double actualQuantity, DateTime executionTime)
        {
            Order = order;
            ActualPrice = actualPrice;
            ActualQuantity = actualQuantity;
            ExecutionTime = executionTime;
        }

        public Security Security { get { return Order.Security; } }
        public Side Side { get { return Order.Side; } }

        public double ActualPrice { get; private set; }
        public double ActualQuantity { get; private set; }

        public DateTime ExecutionTime { get; set; }

        public Order Order { get; private set; }
    }
}
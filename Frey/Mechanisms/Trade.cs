using Automata.Core;
using Automata.Entities;
using System;

namespace Automata.Mechanisms
{
    public class Trade
    {
        public Position Position { get; set; }
        public Order ExitOrder { get; set; }
        public DateTime ExecutionTime { get; set; }
        public double ActuralExitPrice { get; set; }
        public double Return { get; set; }
        public double Profit { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}: {3}-{2} x{4} ={5}",
                Utilities.BracketTime(ExecutionTime), Position.Security.Code,
                ActuralExitPrice, Position.ActualEntryPrice, Position.ActualQuantity, Profit);
        }
    }
}
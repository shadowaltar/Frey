using Automata.Core;
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
            return string.Format("{0} {1}: ({2}-{3})x{4} ={5}",
                ExecutionTime.PrintBracket(), Position.Security.Code,
                ActuralExitPrice, Position.ActualEntryPrice, Position.ActualQuantity, Profit);
        }
    }
}
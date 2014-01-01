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
        public double ExitEquity { get { return Position.ActualQuantity * ActuralExitPrice; } }
        public double EntryEquity { get { return Position.Equity; } }

        /// <summary>
        /// Get the transaction cost to open the position (execute the entry order).
        /// </summary>
        public double OpenPositionTransactionCost { get { return Position.TransactionCost; } }

        /// <summary>
        /// Get or set the transaction cost to close the position (execute the exit order).
        /// </summary>
        public double ClosePositionTransactionCost { get; set; }

        /// <summary>
        /// Get the total transaction cost to complete the trade.
        /// </summary>
        public double TotalTransactionCost { get { return OpenPositionTransactionCost + ClosePositionTransactionCost; } }

        public override string ToString()
        {
            return string.Format("{0} {1}: ({2}-{3})x{4} ={5}",
                ExecutionTime.PrintBracket(), Position.Security.Code,
                ActuralExitPrice, Position.ActualEntryPrice, Position.ActualQuantity, Profit);
        }

        public string PrintCSVFriendly()
        {
            return string.Format("{0}|{1}|{2}|{3}|{4}|{5}",
                ExecutionTime.Print(), Position.Security.Code,
                ActuralExitPrice, Position.ActualEntryPrice, Position.ActualQuantity, Profit);
        }
    }
}
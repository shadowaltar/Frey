using System;
using Automata.Core;

namespace Automata.Mechanisms
{
    public class PortfolioState
    {
        public PortfolioState(double drawdown, double balanceChange, DateTime time, PortfolioAction action)
        {
            Drawdown = drawdown;
            BalanceChange = balanceChange;
            Time = time;
            Action = action;
        }

        public double Drawdown { get; set; }
        public double BalanceChange { get; set; }
        public DateTime Time { get; set; }
        public PortfolioAction Action { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {3} BalanceChange: {1}, Drawdown: {2} ",Time.PrintBracket(), BalanceChange, Drawdown,  Action);
        }
    }

    public enum PortfolioAction
    {
        EnterPosition,
        ClosePosition,
        ContributeCash,
        WithdrawCash,
    }
}
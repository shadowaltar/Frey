using System;
using System.Collections.Generic;
using Automata.Entities;
using Automata.Mechanisms;

namespace Automata.Strategies
{
    /// <summary>
    /// Find highest ranking equities, hold and sell after 4 weeks or if hit stoploss.
    /// Ranking criteria: (from high to low)
    ///     rank = geometric average daily return (4W) / standard deviation of the daily returns (4W).
    /// Stoploss criteria: (assuming init price is p0)
    ///     sl = 0.85*p0.
    /// Will do checking every 1-4th day of a month
    /// </summary>
    public class SharpeRankingStrategy : Strategy
    {
        public override bool IsTimeToStop { get { return false; } }

        public override List<Order> GenerateEntries(PriceCache data, List<Position> existingPositions)
        {
            return null;
        }

        public override List<Order> GenerateExits(PriceCache data, List<Position> existingPositions)
        {
            return null;
        }
    }
}
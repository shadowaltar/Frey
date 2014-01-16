using Automata.Core.Exceptions;
using Automata.Core.Extensions;
using Automata.Entities;
using Automata.Mechanisms;
using System;
using System.Collections.Generic;

namespace Automata.Core
{
    public abstract class DataAccess : IDisposable
    {
        public virtual void Dispose()
        {
        }

        public bool IsEnded { get; protected set; }

        public abstract HashSet<Price> GetNextTimeslotPrices();

        public abstract void Initialize(ITradingScope tradingScope);
    }

    public class DataProcessor
    {
        private readonly TimeSpan tickDuration;
        private readonly TimeSpan doubleTickDuration;

        public DataProcessor(TimeSpan tickDuration)
        {
            this.tickDuration = tickDuration;
            doubleTickDuration = tickDuration.Multiply(2);
        }

        public List<Price> CheckTimeGap(Price current, Price last)
        {
            if (last == null)
                return null;

            var elapsed = current.Start - last.Start;

            if (elapsed == tickDuration)
            {
                return null;
            }

            if (elapsed < tickDuration)
            {
                throw new InvalidTickDurationException();
            }

            if (elapsed > tickDuration && elapsed < doubleTickDuration)
            {
                throw new InvalidTickDurationException();
            }

            var factor = elapsed.Divide(tickDuration);
            if (!factor.IsIntApprox())
                throw new InvalidTickDurationException();

            var results = new List<Price>();
            for (int i = 1; i <= factor.ToInt(); i++)
            {
                var p = new Price(current);
                p.Start = p.Start + tickDuration.Multiply(i);
                results.Add(p);
            }

            return results;
        }
    }
}

using System;
using System.Collections.Generic;

namespace Trading.StrategyBuilder.Core
{
    /// <summary>
    /// Represents the time when the trigger would be invoked.
    /// </summary>
    [Equals]
    public class TriggerTime
    {
        public TimeSpan Frequency { get; set; }
        public bool IsOneTime { get; set; }

        /// <summary>
        /// By given a time, test if is to be triggered.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool CanGo(DateTime time)
        {
            throw new NotImplementedException();
        }

        public bool IsGood()
        {
            throw new NotImplementedException();
        }
    }
}
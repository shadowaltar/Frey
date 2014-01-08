using System;

namespace Automata.Entities
{
    public class TimeValue
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }

        public TimeValue(DateTime time, double value)
        {
            Time = time;
            Value = value;
        }
    }
}
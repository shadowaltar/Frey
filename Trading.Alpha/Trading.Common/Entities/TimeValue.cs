using System;

namespace Trading.Common.Entities
{
    public class TimeValue
    {
        public TimeValue(decimal value, DateTime at)
        {
            Value = value;
            At = at;
        }

        public decimal Value { get; set; } 
        public DateTime At { get; set; } 
    }
}
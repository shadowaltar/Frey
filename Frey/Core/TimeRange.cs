using Automata.Core.Extensions;
using System;

namespace Automata.Core
{
    public class TimeRange
    {
        public TimeRange(DateTime start, DateTime end)
        {
            if (start > end)
                throw new ArgumentException("Start time must be smaller than end time.");

            Start = start;
            End = end;
            Length = end - start;
        }

        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        public TimeSpan Length { get; private set; }

        public bool Contains(DateTime time)
        {
            return time >= Start && time <= End;
        }

        public bool Contains(TimeRange timeRange)
        {
            return timeRange.Start >= Start && timeRange.End <= End;
        }

        public TimeRange Add(TimeSpan timeSpan)
        {
            return new TimeRange(Start.Add(timeSpan), End.Add(timeSpan));
        }

        public TimeRange AddDays(double i)
        {
            return new TimeRange(Start.AddDays(i), End.AddDays(i));
        }

        public TimeRange AddWeeks(int i)
        {
            return new TimeRange(Start.AddWeeks(i), End.AddWeeks(i));
        }

        public TimeRange AddMonths(int i)
        {
            return new TimeRange(Start.AddMonths(i), End.AddMonths(i));
        }

        public override string ToString()
        {
            if (Start.Date == End.Date)
                return string.Format("[{0} {1} - {2}]", Start.PrintDate(), Start.PrintTime(), End.PrintTime());
            return string.Format("[{0} - {1}]", Start.Print(), End.Print());
        }
    }
}
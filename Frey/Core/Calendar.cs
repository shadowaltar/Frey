using System;
using System.Collections.Generic;
using System.Threading;

namespace Automata.Core
{
    public class Calendar
    {
        private readonly Dictionary<int, List<TimeRange>> tradingSessions = new Dictionary<int, List<TimeRange>>();
        public static Calendar ForexCalendar { get; set; }

    //    private ReaderWriterLockSlim initLock = new ReaderWriterLockSlim();

        public Dictionary<int,List<TimeRange>> TradingSessions
        {
            get { return tradingSessions; }
        }

        private Calendar()
        {

        }

        static Calendar()
        {
            lock (typeof(Calendar))
            {
                ForexCalendar = new Calendar();
            }
        }
    }
}
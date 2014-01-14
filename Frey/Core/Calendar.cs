using System;
using System.Collections.Generic;
using System.Threading;

namespace Automata.Core
{
    public class Calendar
    {
        private readonly Dictionary<int, List<Period>> tradingSessions = new Dictionary<int, List<Period>>();
        public static Calendar ForexCalendar { get; set; }

    //    private ReaderWriterLockSlim initLock = new ReaderWriterLockSlim();

        public Dictionary<int,List<Period>> TradingSessions
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

    public class Period
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
using System;
using Trading.Common.Entities;
using Trading.Common.ViewModels;

namespace Trading.TradeWatch.ViewModels.Entities
{
    public class PriceViewModel : ViewModelBaseSlim
    {
        private Security security;
        private DateTime at;
        private TimeSpan span;
        private decimal open;
        private decimal high;
        private decimal low;
        private decimal close;
        private long volume;

        public Security Security
        {
            get { return security; }
            set { SetNotify(ref security, value); }
        }

        public DateTime At
        {
            get { return at; }
            set { SetNotify(ref at, value); }
        }

        public TimeSpan Span
        {
            get { return span; }
            set { SetNotify(ref span, value); }
        }

        public decimal Open
        {
            get { return open; }
            set { SetNotify(ref open, value); }
        }

        public decimal High
        {
            get { return high; }
            set { SetNotify(ref high, value); }
        }

        public decimal Low
        {
            get { return low; }
            set { SetNotify(ref low, value); }
        }

        public decimal Close
        {
            get { return close; }
            set { SetNotify(ref close, value); }
        }

        public long Volume
        {
            get { return volume; }
            set { SetNotify(ref volume, value); }
        }
    }
}
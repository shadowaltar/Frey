﻿using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.Common.ViewModels;

namespace Trading.TradeWatch.ViewModels.Entities
{
    public class SecurityViewModel : ViewModelBaseSlim
    {
        public Security Security { get; private set; }

        public SecurityViewModel(Security security)
        {
            Security = security;
        }

        public string Market
        {
            get { return Security.Market.Code; }
        }

        public string Code
        {
            get { return Security.Code; }
            set
            {
                Security.Code = value;
                Notify();
            }
        }

        public string Type
        {
            get { return Security.Type; }
            set
            {
                Security.Type = value;
                Notify();
            }
        }

        public string Name
        {
            get { return Security.Name; }
            set
            {
                Security.Name = value;
                Notify();
            }
        }

        public string DisplayName
        {
            get { return Security.DisplayName; }
        }

        public override string ToString()
        {
            return Code;
        }
    }
}
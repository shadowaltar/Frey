using Trading.Common.Entities;
using Trading.Common.ViewModels;

namespace Trading.SecurityResearch.ViewModels.Entities
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
    }
}
using Trading.Common.Entities;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.CountryOverrides.Entities;

namespace Trading.CountryOverrides.ViewModels
{
    public class CustomCountryViewModel : ViewModelBaseSlim
    {
        public CustomCountryViewModel()
        {
            Country = new CustomCountry();
            IsExist = false;
        }
        public CustomCountryViewModel(CustomCountry country)
        {
            country.ThrowIfNull("country", "Must provide a country!");
            Country = country;
        }

        public CustomCountryViewModel(CustomCountry country, bool isExist)
            : this(country)
        {
            IsExist = isExist;
        }

        public CustomCountry Country { get; private set; }

        private bool isExist;
        public bool IsExist
        {
            get { return isExist; }
            set
            {
                SetNotify(ref isExist, value);
            }
        }

        public string FmrCode
        {
            get { return Country == null ? string.Empty : Country.FmrCode; }
            set
            {
                Country.FmrCode = value;
                Notify();
            }
        }

        public string Name
        {
            get { return Country == null ? string.Empty : Country.Name; }
            set
            {
                Country.Name = value;
                Notify();
            }
        }

        public string Code
        {
            get { return Country == null ? string.Empty : Country.Code; }
            set
            {
                Country.Code = value;
                Notify();
            }
        }
    }
}
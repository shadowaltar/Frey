using System.Collections.Generic;

namespace Maintenance.CountryOverrides.Entities
{
    public class FilterOptions : Dictionary<FilterOptionType, string>
    {
        public bool IsReset { get; set; }
    }

    public class FilterOption
    {
        public FilterOptionType Type { get; set; }
        public string Value { get; set; }

        public FilterOption(FilterOptionType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Type, Value);
        }
    }
}
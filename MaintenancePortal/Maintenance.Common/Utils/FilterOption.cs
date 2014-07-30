namespace Maintenance.Common.Utils
{
    public class FilterOption
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public FilterOption(string type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return Type + ": " + Value;
        }
    }
}
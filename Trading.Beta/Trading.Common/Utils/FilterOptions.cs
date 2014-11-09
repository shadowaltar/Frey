using System.Collections.Generic;

namespace Trading.Common.Utils
{
    public class FilterOptions : Dictionary<string, string>
    {
        public bool IsReset { get; set; }
    }
}
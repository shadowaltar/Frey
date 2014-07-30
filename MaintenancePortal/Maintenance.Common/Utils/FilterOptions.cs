using System.Collections.Generic;

namespace Maintenance.Common.Utils
{
    public class FilterOptions : Dictionary<string, string>
    {
        public bool IsReset { get; set; }
    }
}
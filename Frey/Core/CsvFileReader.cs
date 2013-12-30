using LumenWorks.Framework.IO.Csv;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Automata.Core
{
    public class CsvFileReader
    {
        public IEnumerable<string[]> Read(string file, bool hasHeader = false, char delimiter = ',')
        {
            using (var reader = new CsvReader(new StreamReader(file), hasHeader, delimiter))
            {
                return reader.ToList();
            }
        }
    }
}

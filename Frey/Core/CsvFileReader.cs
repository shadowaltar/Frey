using Automata.Core.Extensions;
using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Automata.Core
{
    public class CsvFileReader
    {
        public List<string[]> Read(string file, bool hasHeader = false, char delimiter = ',')
        {
            using (var reader = new CsvReader(new StreamReader(file), hasHeader, delimiter))
            {
                return reader.ToList();
            }
        }
    }
}

using System;
using Automata.Core.Extensions;
using LumenWorks.Framework.IO.Csv;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Automata.Core
{
    public class CsvFileAccess : IDisposable
    {
        public static IEnumerable<string[]> Read(string file, bool hasHeader = false, char delimiter = ',')
        {
            using (var reader = new CsvReader(new StreamReader(file), hasHeader, delimiter))
            {
                return reader.ToList();
            }
        }

        private CsvFileAccess() { }

        public static CsvFileAccess GetWriter(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            return new CsvFileAccess
            {
                writer = new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            };
        }

        public void WriteItemsLine(params object[] fields)
        {
            if (writer != null && !fields.IsNullOrEmpty())
            {
                var line = fields.Select(f => f.ToString())
                    .Aggregate((current, field) => current + "|" + field);
                writer.WriteLine(line);
            }
        }

        public void WriteLine(string line)
        {
            if (writer != null)
            {
                writer.WriteLine(line);
                writer.Flush();
            }
        }

        private StreamWriter writer;

        public void Dispose()
        {
            if (writer != null)
            {
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
        }
    }
}

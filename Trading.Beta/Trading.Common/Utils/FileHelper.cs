using CsvHelper;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Trading.Common.Utils
{
    public static class FileHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Find a file in upper level folders.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string FindFileInParents(string fileName)
        {
            var l = Assembly.GetEntryAssembly().Location;
            l = Path.GetDirectoryName(l);
            return RecursiveFindFileInParents(Path.GetDirectoryName(l), fileName);
        }

        private static string RecursiveFindFileInParents(string path, string fileName)
        {
            var parent = Directory.GetParent(path);
            var sameLevelDirs = parent.GetDirectories();
            foreach (var dir in sameLevelDirs)
            {
                var match = dir.EnumerateFiles().FirstOrDefault(f => f.Name.EqualsIgnoreCase(fileName));
                if (match != null)
                    return match.FullName;
            }
            return RecursiveFindFileInParents(parent.FullName, fileName);
        }

        public static List<T> Read<T>(string filePath)
        {
            using (var sr = new StreamReader(filePath))
            using (var cr = new CsvReader(sr))
            {
                return cr.GetRecords<T>().ToList();
            }
        }

        public static Dictionary<string, string> ReadAsDictionary(string filePath, char delimiter = ':')
        {
            var dict = new Dictionary<string, string>();
            if (!File.Exists(filePath))
                return dict;

            var contents = File.ReadAllLines(filePath);
            foreach (var content in contents)
            {
                var pieces = content.Split(delimiter);
                dict[pieces[0]] = content.Substring(pieces[0].Length + 1);
            }
            return dict;
        }

        public static void Write(string fileName, Dictionary<string, string> pairs)
        {
            var sb = new StringBuilder();
            foreach (var pair in pairs.OrderBy(p => p.Key))
            {
                sb.Append(pair.Key).Append(":").AppendLine(pair.Value);
            }
            try
            {
                File.WriteAllText(fileName, sb.ToString());
            }
            catch (Exception e)
            {
                Log.Warn(e);
            }
        }
    }
}
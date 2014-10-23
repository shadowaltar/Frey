using System.IO;
using System.Linq;
using System.Reflection;

namespace Trading.Common.Utils
{
    public static class FileHelper
    {
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Automata.Core
{
    public static class Utilities
    {
        public static string Now
        {
            get { return DateTime.Now.ToString("yyyyMMdd HH:mm:ss"); }
        }

        public static string BracketNow
        {
            get { return "[" + DateTime.Now.ToString("yyyyMMdd HH:mm:ss") + "]"; }
        }

        public static string RetrieveParentFolder(string containedFolder)
        {

        }
    }
}

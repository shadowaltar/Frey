using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Maintenance.Common.Utils
{
    public class Singularizer
    {
        private static readonly IList<string> Unpluralizables =
            new List<string>
                {
                    "equipment",
                    "information",
                    "rice",
                    "money",
                    "species",
                    "series",
                    "fish",
                    "sheep",
                    "deer"
                };

        private static readonly IDictionary<string, string> Singularizations =
            new Dictionary<string, string>
                {
                    // Start with the rarest cases, and move to the most common
                    {"people", "person"},
                    {"oxen", "ox"},
                    {"children", "child"},
                    {"feet", "foot"},
                    {"teeth", "tooth"},
                    {"geese", "goose"},
                    // And now the more standard rules.
                    {"ives", "ife"},
                    {"ves", "f"},
                    // ie, wolf, wife
                    {"men", "man"},
                 //   {"(.+[aeiou])ys$", "$1y"},
                 //   {"(.+[^aeiou])ies$", "$1y"},
                  //  {"zes", "$1"},
                  //  {"([m|l])ice$", "$1ouse"},
                    {"matrices", @"matrix"},
                    {"indices", @"index"},
                 //   {"(.+[^aeiou])ices$","$1ice"},
                 //   {"(.*)ices", @"$1ex"},
                    // ie, Matrix, Index
                //    {"(octop|vir)i$", "$1us"},
                    {"ies", "y"},
                    {"xes", "x"},
                    {"ches", "ch"},
                    {"shes", "sh"},
                    {"thes", "th"},
                    {"ses", "s"},
                    {"s", ""}
                };

        public static string Singularize(string word)
        {
            if (Unpluralizables.Contains(word.ToLowerInvariant()))
            {
                return word;
            }

            foreach (var rule in Singularizations)
            {
                if (word.EndsWith(rule.Key))
                {
                    return word.Substring(0, word.Length - rule.Key.Length) + rule.Value;
                }
            }

            return word;
        }

        public static bool IsPlural(string word)
        {
            if (Unpluralizables.Contains(word.ToLowerInvariant()))
            {
                return true;
            }

            foreach (var singularization in Singularizations)
            {
                if (Regex.IsMatch(word, singularization.Key))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
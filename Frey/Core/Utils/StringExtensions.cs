using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Automata.Core.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex EnglishOnlyRegex = new Regex("^[A-Za-z0-9 !\"#$%&'()*+,./:;<=>?@\\^_`{|}~-]*$");
        private static readonly char[] InvalidFileOrPathChars = Path.GetInvalidFileNameChars()
            .Concat(Path.GetInvalidPathChars()).Distinct().ToArray();

        /// <summary>
        /// Determines whether a string equals to another string, using ordinal case-insensitive comparison.
        /// See <see cref="StringComparison.OrdinalIgnoreCase"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string value, string target)
        {
            return string.Equals(value, target, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether a string is started with another string, using ordinal case-insensitive comparison.
        /// See <see cref="StringComparison.OrdinalIgnoreCase"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static bool StartsWithIgnoreCase(this string value, string prefix)
        {
            return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether a string is ended with another string, using ordinal case-insensitive comparison.
        /// See <see cref="StringComparison.OrdinalIgnoreCase"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static bool EndsWithIgnoreCase(this string value, string suffix)
        {
            return value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether a string is started and ended with two specific strings.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static bool SurroundsWith(this string value, string prefix, string suffix)
        {
            return value.StartsWith(prefix, StringComparison.Ordinal)
                && value.EndsWith(suffix, StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether a string is started and ended with two specific strings,
        /// using ordinal case-insensitive comparison. See <see cref="StringComparison.OrdinalIgnoreCase"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static bool SurroundsWithIgnoreCase(this string value, string prefix, string suffix)
        {
            return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
                && value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether a string contains another substring, case insensitive,
        /// </summary>
        /// <param name="value"></param>
        /// <param name="substring"></param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this string value, string substring)
        {
            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(value, substring, CompareOptions.IgnoreCase) != -1;
        }

        /// <summary>
        /// See if a given string is null, or contains whitespaces only.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrWhitespace(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return true;
            return value.IsWhitespace();
        }

        /// <summary>
        /// See if a string converted from the given object is null, or contains whitespaces only.
        /// Also returns true if the value is null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrWhitespace(this object value)
        {
            if (value == null)
                return true;
            if (string.IsNullOrEmpty(value.ToString()))
                return true;
            return value.ToString().IsWhitespace();
        }

        /// <summary>
        /// See if a given string contains whitespaces only.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsWhitespace(this string value)
        {
            value.ThrowIfNull();
            return value.Trim().Length == 0;
        }

        /// <summary>
        /// See if a string converted from the given object contains whitespaces only.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsWhitespace(this object value)
        {
            value.ThrowIfNull();
            return value.Trim().Length == 0;
        }

        /// <summary>
        /// See if a string converted from the given object contains not only whitespaces.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNotWhitespace(this object value)
        {
            value.ThrowIfNull();
            return value.ToString().Trim().Length != 0;
        }

        /// <summary>
        /// Trim a given string; if null, return <see cref="string.Empty"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TrimOrDefault(this string value)
        {
            return value.IsNullOrWhitespace() ? string.Empty : value.Trim();
        }

        /// <summary>
        /// Trim a string converted from the given object; if null, return <see cref="string.Empty"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TrimOrDefault(this object value)
        {
            return value.IsNullOrWhitespace() ? string.Empty : value.Trim();
        }

        /// <summary>
        /// Trim a string converted from the given object; throw exception if it is null.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Trim(this object value)
        {
            value.ThrowIfNull();
            return value.ToString().Trim();
        }

        /// <summary>
        /// Trim a string, and further remove the string tail if it is longer than the specified
        /// length.
        /// If <paramref name="allowNull"/> is true, an input null value will return a null value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxAllowedLength"></param>
        /// <param name="allowNull"></param>
        /// <returns></returns>
        public static string TrimToLength(this string value, int maxAllowedLength, bool allowNull)
        {
            if (!allowNull)
                value.ThrowIfNull();
            else if (value == null)
                return null;

            value = value.Trim();

            return value.Length <= maxAllowedLength ? value : value.Substring(0, maxAllowedLength);
        }

        /// <summary>
        /// Check if a string contains only alphabetical, digital and punctuation characters.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEnglishOnly(this string value)
        {
            return EnglishOnlyRegex.IsMatch(value.Trim());
        }

        /// <summary>
        /// Convert the string to title/proper case.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string value)
        {
            value.ThrowIfNull("value", "You cannot convert a null string to title case.");
            return value.Length == 0 ? string.Empty
                : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower());
        }

        /// <summary>
        /// Convert the string to title/proper case with specific <see cref="CultureInfo"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static string ToTitleCase(this string value, CultureInfo cultureInfo)
        {
            value.ThrowIfNull("value", "You cannot convert a null string to title case.");
            cultureInfo.ThrowIfNull("cultureInfo", "You cannot convert a string with null CultureInfo.");

            return value.Length == 0 ? string.Empty
                : cultureInfo.TextInfo.ToTitleCase(value.ToLower(cultureInfo));
        }

        /// <summary>
        /// Find and fix invalid chars in terms of file name or path, for a given string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="substitution"></param>
        /// <returns></returns>
        public static string FixFileOrPathCharsBy(this string value, char substitution)
        {
            if (value == null)
                return value;
            return InvalidFileOrPathChars.Aggregate(value, (current, c) => current.Replace(c, substitution));
        }

        /// <summary>
        /// Find out whether a collection contains a string, case-insensitive.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this IEnumerable<string> list, string value)
        {
            var item = list.FirstOrDefault(i => i.Equals(value, StringComparison.InvariantCultureIgnoreCase));
            return item != null;
        }

        /// <summary>
        /// Find out the first item's index in a list of strings, case-insensitive.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int IndexOfIgnoreCase(this IList<string> list, string value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (string.Equals(list[i], value, StringComparison.InvariantCultureIgnoreCase))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Find all indexes for a substring in a string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<int> AllIndexesOf(this string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    break;
                yield return index;
            }
        }

        /// <summary>
        /// Convert a string to a SecureString.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static SecureString ToSecureString(this string value)
        {
            if (value == null)
                return null;
            var chars = value.ToCharArray();
            var ss = new SecureString();
            foreach (var c in chars)
            {
                ss.AppendChar(c);
            }
            return ss;
        }

        /// <summary>
        /// Convert a SecureString to a string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FromSecureString(this SecureString message)
        {
            var unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(message);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        /// <summary>
        /// Encrypt a string to another string presented in hex numbers and symbols.
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>
        public static string Encrypt(this string clearText)
        {
            var clearTextBytes = Encoding.UTF8.GetBytes(clearText);

            var sa = SymmetricAlgorithm.Create();
            var ms = new MemoryStream();
            var rgbIV = Encoding.ASCII.GetBytes("gklpwpaercslypxc");
            var key = Encoding.ASCII.GetBytes("bsunnqacixtjuapslsgookiewpwpxkjd");
            var cs = new CryptoStream(ms, sa.CreateEncryptor(key, rgbIV), CryptoStreamMode.Write);

            cs.Write(clearTextBytes, 0, clearTextBytes.Length);
            cs.Close();

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Decrypt into a string from a string presented in hex numbers and symbols.
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <returns></returns>
        public static string Decrypt(this string encryptedText)
        {
            var encryptedTextBytes = Convert.FromBase64String(encryptedText);

            var ms = new MemoryStream();
            var sa = SymmetricAlgorithm.Create();

            var rgbIV = Encoding.ASCII.GetBytes("gklpwpaercslypxc");
            var key = Encoding.ASCII.GetBytes("bsunnqacixtjuapslsgookiewpwpxkjd");

            var cs = new CryptoStream(ms, sa.CreateDecryptor(key, rgbIV), CryptoStreamMode.Write);
            cs.Write(encryptedTextBytes, 0, encryptedTextBytes.Length);
            cs.Close();

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}

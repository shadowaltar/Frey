using System;
using System.Collections.Generic;
using System.Linq;

namespace Automata.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static void ThrowIfNull(this object source, string paramName, string message)
        {
            if (null == source)
            {
                if (paramName != string.Empty)
                {
                    if (message != string.Empty)
                    {
                        throw new ArgumentNullException(paramName, message);
                    }
                    throw new ArgumentNullException(paramName);
                }
                throw new ArgumentNullException();
            }
        }
        public static void ThrowIfNull(this object source, string paramName)
        {
            ThrowIfNull(source, paramName, "The object is null.");
        }
        public static void ThrowIfNull(this object source)
        {
            ThrowIfNull(source, string.Empty, "The object is null.");
        }

        public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T> source, string paramName, string message)
        {
            source.ThrowIfNull(paramName, message);
            if (source.Count() == 0)
            {
                throw new ArgumentNullException(paramName, message + @" (Empty Collection)");
            }
        }
        public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T> source, string paramName)
        {
            ThrowIfNullOrEmpty(source, paramName, "The enumerable is null or empty");
        }
        public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T> source)
        {
            ThrowIfNullOrEmpty(source, string.Empty, "The enumerable is null or empty");
        }

        public static void ThrowIfNullOrEmpty(this string source, string paramName, string message)
        {
            if (string.IsNullOrEmpty(source))
            {
                if (paramName != string.Empty)
                {
                    if (message != string.Empty)
                    {
                        throw new ArgumentNullException(paramName, message);
                    }
                    throw new ArgumentNullException(paramName);
                }
                throw new ArgumentNullException();
            }
        }
        public static void ThrowIfNullOrEmpty(this string source, string paramName)
        {
            ThrowIfNullOrEmpty(source, paramName, "The string is null or empty.");
        }
        public static void ThrowIfNullOrEmpty(this string source)
        {
            ThrowIfNullOrEmpty(source, string.Empty, "The string is null or empty.");
        }

        public static T CastOrThrow<T>(this object source, string paramName, string message)
        {
            if (!(source is T))
            {
                if (paramName != string.Empty)
                {
                    if (message != string.Empty)
                    {
                        throw new ArgumentException(message, paramName);
                    }
                    throw new ArgumentException(message);
                }
                throw new ArgumentException();
            }
            return (T)source;
        }
        public static T CastOrThrow<T>(this object source, string paramName)
        {
            return CastOrThrow<T>(source, paramName, "Type casting failed.");
        }
        public static T CastOrThrow<T>(this object source)
        {
            return CastOrThrow<T>(source, string.Empty, "Type casting failed.");
        }

        public static void ThrowIfNotAssignable<T>(this Type subType, string paramName, string message)
        {
            subType.ThrowIfNull();
            if (!typeof(T).IsAssignableFrom(subType))
            {
                if (paramName != string.Empty)
                {
                    if (message != string.Empty)
                    {
                        throw new ArgumentException(message, paramName);
                    }
                    throw new ArgumentException(message);
                }
                throw new ArgumentException();
            }
        }
        public static void ThrowIfNotAssignable<T>(this Type subType, string paramName)
        {
            ThrowIfNotAssignable<T>(subType, paramName, "Invalid type assignment.");
        }
        public static void ThrowIfNotAssignable<T>(this Type subType)
        {
            ThrowIfNotAssignable<T>(subType, string.Empty, "Invalid type assignment.");
        }

        public static void ThrowIfZero(this int value, string paramName, string message)
        {
            if (value == 0)
            {
                if (paramName != string.Empty)
                {
                    if (message != string.Empty)
                    {
                        throw new ArgumentException(message, paramName);
                    }
                    throw new ArgumentException(message);
                }
                throw new ArgumentException();
            }
        }
        public static void ThrowIfZero(this int value, string paramName)
        {
            ThrowIfZero(value, paramName, "Zero value is not allowed.");
        }
        public static void ThrowIfZero(this int value)
        {
            ThrowIfZero(value, string.Empty, "Zero value is not allowed.");
        }

        public static void ThrowIfZero(this decimal value, string paramName, string message)
        {
            if (value == 0)
            {
                if (paramName != string.Empty)
                {
                    if (message != string.Empty)
                    {
                        throw new ArgumentException(message, paramName);
                    }
                    throw new ArgumentException(message);
                }
                throw new ArgumentException();
            }
        }
        public static void ThrowIfZero(this decimal value, string paramName)
        {
            ThrowIfZero(value, paramName, "Zero value is not allowed.");
        }
        public static void ThrowIfZero(this decimal value)
        {
            ThrowIfZero(value, string.Empty, "Zero value is not allowed.");
        }

        public static void ThrowIfNegative(this int value, string paramName, string message)
        {
            if (value < 0)
            {
                if (paramName != string.Empty)
                {
                    if (message != string.Empty)
                    {
                        throw new ArgumentException(message, paramName);
                    }
                    throw new ArgumentException(message);
                }
                throw new ArgumentException();
            }
        }
        public static void ThrowIfNegative(this int value, string paramName)
        {
            ThrowIfNegative(value, paramName, "Negative value is not allowed.");
        }
        public static void ThrowIfNegative(this int value)
        {
            ThrowIfNegative(value, string.Empty, "Negative value is not allowed.");
        }

        public static void ThrowIfNegative(this decimal value, string paramName, string message)
        {
            if (value < 0)
            {
                if (paramName != string.Empty)
                {
                    if (message != string.Empty)
                    {
                        throw new ArgumentException(message, paramName);
                    }
                    throw new ArgumentException(message);
                }
                throw new ArgumentException();
            }
        }
        public static void ThrowIfNegative(this decimal value, string paramName)
        {
            ThrowIfNegative(value, paramName, "Negative value is not allowed.");
        }
        public static void ThrowIfNegative(this decimal value)
        {
            ThrowIfNegative(value, string.Empty, "Negative value is not allowed.");
        }

        public static void ThrowIfZeroOrNegative(this int value, string paramName, string message)
        {
            if (value <= 0)
            {
                if (paramName != string.Empty)
                {
                    if (message != string.Empty)
                    {
                        throw new ArgumentException(message, paramName);
                    }
                    throw new ArgumentException(message);
                }
                throw new ArgumentException();
            }
        }
        public static void ThrowIfZeroOrNegative(this int value, string paramName)
        {
            ThrowIfZeroOrNegative(value, paramName, "Negative or zero value is not allowed.");
        }
        public static void ThrowIfZeroOrNegative(this int value)
        {
            ThrowIfZeroOrNegative(value, string.Empty, "Negative or zero value is not allowed.");
        }

        public static void ThrowIfZeroOrNegative(this decimal value, string paramName, string message)
        {
            if (value <= 0)
            {
                if (paramName != string.Empty)
                {
                    if (message != string.Empty)
                    {
                        throw new ArgumentException(message, paramName);
                    }
                    throw new ArgumentException(message);
                }
                throw new ArgumentException();
            }
        }
        public static void ThrowIfZeroOrNegative(this decimal value, string paramName)
        {
            ThrowIfZeroOrNegative(value, paramName, "Negative or zero value is not allowed.");
        }
        public static void ThrowIfZeroOrNegative(this decimal value)
        {
            ThrowIfZeroOrNegative(value, string.Empty, "Negative or zero value is not allowed.");
        }

        public static void ThrowIfEquals<T>(this IComparable value, T comparor, string paramName, string message)
        {
            if (value.CompareTo(comparor) == 0)
            {
                if (paramName != string.Empty)
                {
                    if (message != string.Empty)
                    {
                        throw new ArgumentException(message, paramName);
                    }
                    throw new ArgumentException(message);
                }
                throw new ArgumentException();
            }
        }
        public static void ThrowIfEquals<T>(this IComparable value, T comparor, string paramName)
        {
            ThrowIfEquals(value, comparor, paramName, "Shall not be equal.");
        }
        public static void ThrowIfEquals<T>(this IComparable value, T comparor)
        {
            ThrowIfEquals(value, comparor, string.Empty, "Shall not be equal.");
        }
    }
}
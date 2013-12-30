using System;
using System.Collections.Generic;
using System.Linq;

namespace Automata.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static void ThrowIfNull(this object source, string paramName, string message = "The object is null.")
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

        public static void ThrowIfNull(this object source)
        {
            ThrowIfNull(source, string.Empty);
        }

        public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T> source, string paramName, string message = "The enumerable is null or empty")
        {
            source.ThrowIfNull(paramName, message);
            if (!source.Any())
            {
                throw new ArgumentNullException(paramName, message + @" (Empty Collection)");
            }
        }

        public static void ThrowIfNullOrEmpty<T>(this IEnumerable<T> source)
        {
            ThrowIfNullOrEmpty(source, string.Empty);
        }

        public static void ThrowIfNullOrEmpty(this string source, string paramName, string message = "The string is null or empty.")
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

        public static void ThrowIfNullOrEmpty(this string source)
        {
            ThrowIfNullOrEmpty(source, string.Empty);
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

        public static void ThrowIfZero(this int value, string paramName, string message = "Zero value is not allowed.")
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

        public static void ThrowIfZero(this int value)
        {
            ThrowIfZero(value, string.Empty);
        }

        public static void ThrowIfZero(this decimal value, string paramName, string message = "Zero value is not allowed.")
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

        public static void ThrowIfZero(this decimal value)
        {
            ThrowIfZero(value, string.Empty);
        }

        public static void ThrowIfNegative(this int value, string paramName, string message = "Negative value is not allowed.")
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

        public static void ThrowIfNegative(this int value)
        {
            ThrowIfNegative(value, string.Empty);
        }

        public static void ThrowIfNegative(this decimal value, string paramName, string message = "Negative value is not allowed.")
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

        public static void ThrowIfNegative(this decimal value)
        {
            ThrowIfNegative(value, string.Empty);
        }

        public static void ThrowIfZeroOrNegative(this int value, string paramName, string message = "Negative or zero value is not allowed.")
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

        public static void ThrowIfZeroOrNegative(this int value)
        {
            ThrowIfZeroOrNegative(value, string.Empty);
        }

        public static void ThrowIfZeroOrNegative(this decimal value, string paramName, string message = "Negative or zero value is not allowed.")
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

        public static void ThrowIfZeroOrNegative(this decimal value)
        {
            ThrowIfZeroOrNegative(value, string.Empty);
        }

        public static void ThrowIfEquals<T>(this IComparable value, T comparor, string paramName, string message = "Shall not be equal.")
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

        public static void ThrowIfEquals<T>(this IComparable value, T comparor)
        {
            ThrowIfEquals(value, comparor, string.Empty);
        }
    }
}
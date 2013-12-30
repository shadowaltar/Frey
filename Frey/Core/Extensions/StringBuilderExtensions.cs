using System.Text;

namespace Automata.Core.Extensions
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Appends a formatted string to this instance, and then appends the default line terminator
        /// to its end.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static StringBuilder AppendFormatLine(this StringBuilder builder, string format, params object[] args)
        {
            builder.ThrowIfNull();
            return builder.AppendFormat(format, args).AppendLine();
        }

        /// <summary>
        /// Appends the default line terminator, and then appends a formatted string to this instance
        /// to its end.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static StringBuilder AppendLineFormat(this StringBuilder builder, string format, params object[] args)
        {
            builder.ThrowIfNull();
            return builder.AppendLine().AppendFormat(format, args);
        }

        /// <summary>
        /// Appends an object which converted to string to this instance, and then appends a tab character
        /// to its end.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static StringBuilder AppendTab(this StringBuilder builder, object value)
        {
            builder.ThrowIfNull();
            return builder.Append(value).Append('\t');
        }

        /// <summary>
        /// Appends a tab character to this instance.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static StringBuilder AppendTab(this StringBuilder builder)
        {
            builder.ThrowIfNull();
            return builder.Append('\t');
        }
    }
}

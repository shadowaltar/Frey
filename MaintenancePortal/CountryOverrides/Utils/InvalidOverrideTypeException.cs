using System;

namespace Maintenance.CountryOverrides.Utils
{
    public class InvalidOverrideTypeException : InvalidOperationException
    {
        public OverrideType? InvalidType { get; set; }

        public InvalidOverrideTypeException()
        {
        }

        public InvalidOverrideTypeException(string message)
            : base(message)
        {
        }

        public InvalidOverrideTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidOverrideTypeException(OverrideType? type)
            : base("Invalid type " + type + " is used.")
        {
            InvalidType = type;
        }

        public InvalidOverrideTypeException(OverrideType? type, string message)
            : base(message)
        {
            InvalidType = type;
        }

        public InvalidOverrideTypeException(OverrideType? type, string message, Exception innerException)
            : base(message, innerException)
        {
            InvalidType = type;
        }
    }
}
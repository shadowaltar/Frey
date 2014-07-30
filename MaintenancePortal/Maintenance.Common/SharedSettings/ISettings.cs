using System.Collections.Generic;

namespace Maintenance.Common.SharedSettings
{
    /// <summary>
    /// Interface for classes which store settings for a group of screens. For example,
    /// the database connection information for IMAP or FMS systems.
    /// </summary>
    public interface ISettings
    {
        Dictionary<string, string> Environments { get; }
        string DefaultEnvironment { get; }
    }
}
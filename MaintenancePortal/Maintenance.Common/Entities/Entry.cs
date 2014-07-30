using System;

namespace Maintenance.Common.Entities
{
    /// <summary>
    /// Simple entry object which has an Id, creation and update related fields.
    /// </summary>
    public class Entry
    {
        public long Id { get; set; }

        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Updater { get; set; }
        public DateTime UpdateTime { get; set; } 
    }
}
using System;

namespace Maintenance.Common.Entities.Decorators
{
    public interface IEffectiveTimeRange
    {
        /// <summary>
        /// Gets or sets the date that this entity becomes effective (inclusive).
        /// </summary>
        DateTime EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets the date that this entity becomes expired or deactivated (effectiveness exclusive).
        /// </summary>
        DateTime ExpiryDate { get; set; }
    }
}
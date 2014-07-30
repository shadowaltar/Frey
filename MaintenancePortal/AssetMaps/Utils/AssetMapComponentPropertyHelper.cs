using System.Collections.Generic;
using Maintenance.AssetMaps.ViewModels;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;

namespace Maintenance.AssetMaps.Utils
{
    public static class AssetMapComponentPropertyHelper
    {
        private static readonly Dictionary<string, List<object>> availableProperties = new Dictionary<string, List<object>>();
        private static readonly Dictionary<string, List<int>> allowedLevels = new Dictionary<string, List<int>>();

        static AssetMapComponentPropertyHelper()
        {
            availableProperties.Add("INCLUDE_IN_FI_DATA", new List<object> { "TRUE", "FALSE" });
            availableProperties.Add("INCLUDE_IN_MATRIX", new List<object> { "TRUE", "FALSE" });
            availableProperties.Add("LEVEL_COUNTRY", new List<object> { "1", "2", "3", "4", "5", "6" });
            availableProperties.Add("LEVEL_REGION", new List<object> { "1", "2", "3", "4", "5", "6" });

            allowedLevels.Add("INCLUDE_IN_FI_DATA", new List<int> { 0, 1, 2, 3, 4, 5, 6 });
            allowedLevels.Add("INCLUDE_IN_MATRIX", new List<int> { 0, 1, 2, 3, 4, 5, 6 });
            allowedLevels.Add("LEVEL_COUNTRY", new List<int> { 0 });
            allowedLevels.Add("LEVEL_REGION", new List<int> { 0 });
        }

        /// <summary>
        /// Get the list of options for a kind of property, defined by the key string.
        /// </summary>
        /// <param name="propertyKey"></param>
        /// <returns></returns>
        public static List<object> GetOptions(string propertyKey)
        {
            List<object> values;
            if (string.IsNullOrEmpty(propertyKey))
                return null;
            availableProperties.TryGetValue(propertyKey, out values);
            return values;
        }

        /// <summary>
        /// Check if this kind of property could be set for a specific level (depth in a tree) of
        /// an asm component.
        /// </summary>
        /// <param name="propertyKey"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static bool CheckIfAllowed(string propertyKey, int level)
        {
            List<int> values;
            if (string.IsNullOrEmpty(propertyKey))
                return false;
            if (allowedLevels.TryGetValue(propertyKey, out values))
            {
                return values.Contains(level);
            }
            return false;
        }

        /// <summary>
        /// Add the allowed property keys for the given component by its level.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="propertyViewModel"></param>
        public static void AddAllowedProperties(AssetMapComponent component,
            ComponentPropertyViewModel propertyViewModel)
        {
            component.ThrowIfNull("component", "must provide a component.");
            propertyViewModel.ThrowIfNull("component", "must provide a property's view model.");

            propertyViewModel.Options.Clear();

            var level = AssetMapComponent.GetLevel(component);
            foreach (var option in availableProperties.Keys)
            {
                if (CheckIfAllowed(option, level))
                    propertyViewModel.PropertyKeys.Add(option);
            }
        }

        /// <summary>
        /// Get the max possible count of properties for a component.
        /// </summary>
        public static int MaxPropertyPerComponent { get { return availableProperties.Count; } }
    }
}
using System.Configuration;

namespace Maintenance.Portal.ApplicationSettings
{
    public class ScreenSection : ConfigurationSection
    {
        /// <summary>
        /// For db env default values.
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(ScreenElementCollection), AddItemName = "Screen")]
        public ScreenElementCollection Screens
        {
            get { return ((ScreenElementCollection)(base[""])); }
        }
    }
}
using System.Configuration;

namespace Maintenance.Common.ApplicationSettings
{
    public class DatabaseSection : ConfigurationSection
    {
        /// <summary>
        /// For db env default values.
        /// </summary>
        [ConfigurationProperty("DefaultDatabaseEnvironments")]
        public DatabaseDefaultEnvironmentElementCollection DatabaseDefaultEnvironments
        {
            get { return ((DatabaseDefaultEnvironmentElementCollection)(base["DefaultDatabaseEnvironments"])); }
        }

        /// <summary>
        /// For db env connection details.
        /// </summary>
        [ConfigurationProperty("DatabaseEnvironments")]
        public DatabaseEnvironmentElementCollection DatabaseEnvironments
        {
            get { return ((DatabaseEnvironmentElementCollection)(base["DatabaseEnvironments"])); }
        }
    }
}
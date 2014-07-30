using System;
using System.Collections.Generic;
using System.Configuration;
using log4net;

namespace Maintenance.Common.ApplicationSettings
{
    /// <summary>
    /// A helper class to read application settings from app.config files.
    /// </summary>
    public static class SettingsHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string GetDefaultDatabaseEnviroment(string system)
        {
            try
            {
                var section = ConfigurationManager.GetSection("Databases") as DatabaseSection;
                if (section != null)
                {
                    var defaultDbs = section.DatabaseDefaultEnvironments;
                    foreach (DatabaseDefaultEnvironmentElement elem in defaultDbs)
                    {
                        if (elem.System == system)
                        {
                            return elem.Environment;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Log.Error("Cannot read the setting of server environment \"DefaultEnvironment\" from config file, for system " + system);
            }
            return string.Empty;
        }

        public static Dictionary<string, string> GetDatabaseConnections(string system)
        {
            var environments = new Dictionary<string, string>(); 
            try
            {
                var section = ConfigurationManager.GetSection("Databases") as DatabaseSection;
                if (section != null)
                {
                    var defaultDbs = section.DatabaseEnvironments;
                    foreach (DatabaseEnvironmentElement elem in defaultDbs)
                    {
                        if (elem.System == system)
                        {
                            environments[elem.Environment] = elem.EncryptedConnection;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Log.Error("Cannot read the setting of server environment \"DatabaseEnvironments\" from config file, for system " + system);
            }
            return environments;
        }

        public static Dictionary<string, string> GetDatabaseServerNames(string system)
        {
            var environments = new Dictionary<string, string>();
            try
            {
                var section = ConfigurationManager.GetSection("Databases") as DatabaseSection;
                if (section != null)
                {
                    var defaultDbs = section.DatabaseEnvironments;
                    foreach (DatabaseEnvironmentElement elem in defaultDbs)
                    {
                        if (elem.System == system)
                        {
                            environments[elem.Environment] = elem.ServerName;
                        }
                    }
                }
            }
            catch (Exception)
            {
                Log.Error("Cannot read the setting of server environment \"DatabaseEnvironments\" from config file, for system " + system);
            }
            return environments;
        }
    }
}
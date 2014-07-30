using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Maintenance.CountryOverrides
{
    public sealed class Settings
    {
        private Settings()
        {
            overrideTypes.AddRange(Enum.GetValues(typeof(OverrideType)).Cast<OverrideType>());

            ReadEnvironments();
        }

        public const int DatabaseError = 10;
        private static readonly string[] ExpectedEnvironmentLabels = { "DEV", "QA", "SIT", "UAT", "PROD" };
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Lazy<Settings> Lazy = new Lazy<Settings>(() => new Settings());
        public static Settings Instance { get { return Lazy.Value; } }

        private string defaultEnvironment;
        public static string DefaultEnvironment { get { return Instance.defaultEnvironment; } }

        private readonly List<OverrideType> overrideTypes = new List<OverrideType>();
        public static List<OverrideType> OverrideTypes { get { return Instance.overrideTypes; } }
  
        private readonly Dictionary<string, string> environments = new Dictionary<string, string>();
        public static Dictionary<string, string> Environments { get { return Instance.environments; } }

        private void ReadEnvironments()
        {
            defaultEnvironment = ReadAppSettings("DefaultEnvironment");
            if (string.IsNullOrWhiteSpace(defaultEnvironment))
            {
                Log.Error("Cannot read the setting of server environment \"DefaultEnvironment\" from config file.");
                throw new InvalidOperationException("Cannot read the setting of server environment \"DefaultEnvironment\" from config file.");
            }

            foreach (var name in ExpectedEnvironmentLabels)
            {
                var value = ReadAppSettings(name);
                if (string.IsNullOrWhiteSpace(value))
                {
                    Log.Warn("Cannot read the setting of server label \"" + name +
                             "\" from config file. It will not be able to be chosen in UI.");
                }
                else
                {
                    environments[name] = value;
                }
            }

            if (environments.Count == 0)
            {
                Log.Error("No server environments are defined in your application config file.");
                throw new InvalidOperationException("No server environments are defined in your application config file.");
            }
        }

        private static string ReadAppSettings(string key, string defaultValue = "")
        {
            try
            {
                return ConfigurationManager.AppSettings[key];
            }
            catch
            {
                Log.WarnFormat("Cannot find application setting \"" + key + "\", default value \"" + defaultValue + "\" will be used.");
                return defaultValue;
            }
        }

        /// <summary>
        /// Get the database service name, defined in "SERVICE_NAME = {0}".
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetDbServiceName(string key)
        {
            string value;
            if (Environments.TryGetValue(key, out value))
            {
                var id1 = value.IndexOf("SERVICE_NAME", StringComparison.InvariantCultureIgnoreCase);
                var id2 = value.IndexOf(")", id1, StringComparison.InvariantCultureIgnoreCase);
                if (id1 != -1 && id2 > id1)
                {
                    return value.Substring(id1, id2 - id1)
                        .Replace("SERVICE_NAME", string.Empty).Trim().Trim('=').Trim();
                }
            }
            return string.Empty;
        }
    }
}
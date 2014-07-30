using System;
using System.Collections.Generic;
using log4net;
using Maintenance.Common.ApplicationSettings;

namespace Maintenance.Common.SharedSettings
{
    public class FmsSettings : ISettings
    {
        public FmsSettings()
        {
            Environments = new Dictionary<string, string>();
            ReadEnvironments();
        }

        private static readonly string[] ExpectedEnvironmentLabels = { "DEV", "QA", "SIT", "UAT", "PROD" };
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Dictionary<string, string> Environments { get; private set; }
        public string DefaultEnvironment { get; private set; }

        /// <summary>
        /// Read the environment of IMAP DB settings.
        /// </summary>
        private void ReadEnvironments()
        {
            DefaultEnvironment = SettingsHelper.GetDefaultDatabaseEnviroment("FMS");
            if (string.IsNullOrWhiteSpace(DefaultEnvironment))
            {
                Log.Error("Cannot read the setting of server environment \"DefaultEnvironment\" from config file, the expected system is FMS.");
                throw new InvalidOperationException("Cannot read the setting of server environment \"DefaultEnvironment\" from config file, the expected system is FMS.");
            }

            var info = SettingsHelper.GetDatabaseServerNames("FMS");

            if (info.Count == 0)
            {
                Log.Error("No server environments are defined in your application config file.");
                throw new InvalidOperationException("No server environments are defined in your application config file.");
            }

            foreach (var name in ExpectedEnvironmentLabels)
            {
                string server;

                if (!info.TryGetValue(name, out server))
                {
                    Log.Warn("Cannot read the setting of server label \"" + name +
                             "\" from config file. It will not be able to be chosen in UI.");
                }
                else
                {
                    Environments[name] = server;
                }
            }

            if (Environments.Count == 0)
            {
                Log.Error("No server environments are defined in your application config file.");
                throw new InvalidOperationException("No server environments are defined in your application config file.");
            }
        }
    }
}
using System;
using System.Collections.Generic;
using log4net;
using Maintenance.Common.ApplicationSettings;
using Maintenance.Common.Utils;

namespace Maintenance.Common.SharedSettings
{
    public sealed class ImapSettings : ISettings
    {
        public ImapSettings()
        {
            Environments = new Dictionary<string, string>();
            ReadEnvironments();
            ReadAssetClassFocusOptions();
        }

        private static readonly string[] ExpectedEnvironmentLabels = { "DEV", "QA", "SIT", "UAT", "PROD" };
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Dictionary<string, string> Environments { get; private set; }
        public string DefaultEnvironment { get; private set; }

        private readonly List<string> assetClassFocuses = new List<string>();
        public List<string> AssetClassFocuses { get { return assetClassFocuses; } }

        /// <summary>
        /// Read the environment of IMAP DB settings.
        /// </summary>
        private void ReadEnvironments()
        {
            DefaultEnvironment = SettingsHelper.GetDefaultDatabaseEnviroment("IMAP");
            if (string.IsNullOrWhiteSpace(DefaultEnvironment))
            {
                Log.Error("Cannot read the setting of server environment \"DefaultEnvironment\" from config file, the expected system is IMAP.");
                throw new InvalidOperationException("Cannot read the setting of server environment \"DefaultEnvironment\" from config file, the expected system is IMAP.");
            }

            var info = SettingsHelper.GetDatabaseConnections("IMAP");

            if (info.Count == 0)
            {
                Log.Error("No server environments are defined in your application config file.");
                throw new InvalidOperationException("No server environments are defined in your application config file.");
            }

            var crypto = new AesEncryption();
            foreach (var name in ExpectedEnvironmentLabels)
            {
                string encrypted;

                if (!info.TryGetValue(name, out encrypted))
                {
                    Log.Warn("Cannot read the setting of server label \"" + name +
                             "\" from config file. It will not be able to be chosen in UI.");
                }
                else
                {
                    Environments[name] = crypto.DecryptString(encrypted, "MaintenancePortal");
                }
            }

            if (Environments.Count == 0)
            {
                Log.Error("No server environments are defined in your application config file.");
                throw new InvalidOperationException("No server environments are defined in your application config file.");
            }
        }

        private void ReadAssetClassFocusOptions()
        {
            assetClassFocuses.Add("Equity");
            assetClassFocuses.Add("Property");
            assetClassFocuses.Add("Bond");
            assetClassFocuses.Add("Commodity");
            assetClassFocuses.Add("Cash");
        }

        /// <summary>
        /// Get the database service name, defined in "SERVICE_NAME = {0}".
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetOracleConnectionServiceName(string key)
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
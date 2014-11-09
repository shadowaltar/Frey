using System;
using System.Collections.Generic;
using log4net;
using Trading.Common.ApplicationSettings;

namespace Trading.Common.SharedSettings
{
    public class InfrastructureSettings : ISettings
    {
        public InfrastructureSettings()
        {
        }

        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    }
}
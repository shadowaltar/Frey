using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using log4net;

namespace Maintenance.Portal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Current.DispatcherUnhandledException += OnUnhandledExceptionThrown;
        }

        private void OnUnhandledExceptionThrown(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            log.Error("Unknown exception is thrown (handled " + e.Handled + ")", e.Exception);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Current.DispatcherUnhandledException -= OnUnhandledExceptionThrown;
        }
    }
}

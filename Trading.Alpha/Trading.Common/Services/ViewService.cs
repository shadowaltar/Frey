using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Trading.Common.ViewModels;

namespace Trading.Common.Services
{

    public class ViewService : IViewService
    {
        private MetroWindow window;

        /// <summary>
        /// Initialize a view service which can bring up metro-style dialogs / windows,
        /// or other controlling actions.
        /// The view being operated on is usually the main view, which depends on the
        /// place where the dependency injection happens.
        /// </summary>
        /// <param name="wm"></param>
        public ViewService(IWindowManager wm)
        {
            WindowManager = wm;
        }

        public IWindowManager WindowManager { private set; get; }

        public MetroWindow Window
        {
            private get { return window; }
            set { window = value; }
        }

        public Task<string> ShowInputDialog(string title, string message, MetroDialogSettings settings = null)
        {
            CheckMetroWindowExists();
            return Window.ShowInputAsync(title, message, settings);
        }

        public Task<MessageDialogResult> ShowMessage(string title, string message,
            bool isAffirmative = true, bool isAccentedColor = true,
            string affirmativeButtonText = "Ok", string negativeButtonText = "Cancel")
        {
            CheckMetroWindowExists();
            return Window.ShowMessageAsync(title, message,
                isAffirmative ? MessageDialogStyle.Affirmative
                    : MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
                    {
                        ColorScheme = isAccentedColor ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme,
                        UseAnimations = true,
                        AffirmativeButtonText = affirmativeButtonText,
                        NegativeButtonText = negativeButtonText
                    });
        }

        public Task<ProgressDialogController> ShowProgress(string title, string message,
            bool isAccentedColor = false)
        {
            CheckMetroWindowExists();
            return Window.ShowProgressAsync(title, message, false, new MetroDialogSettings
            {
                UseAnimations = true,
                ColorScheme = isAccentedColor ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme,
            });
        }

        public void ShowWindow(ViewModelBase viewModel)
        {
            CheckMetroWindowExists();
            var cfg = new Dictionary<string, object>
            {
                { "WindowStartupLocation", WindowStartupLocation.CenterOwner },
                { "Owner", Window },
            };

            WindowManager.ShowWindow(viewModel, null, cfg);
        }

        public void ShowIndependentWindow(ViewModelBase viewModel)
        {
            CheckMetroWindowExists();
            var cfg = new Dictionary<string, object>
            {
                { "WindowStartupLocation", WindowStartupLocation.CenterOwner },
            };

            WindowManager.ShowWindow(viewModel, null, cfg);
        }

        public bool? ShowDialog(ViewModelBase viewModel)
        {
            CheckMetroWindowExists();
            var cfg = new Dictionary<string, object>
            {
                { "WindowStartupLocation", WindowStartupLocation.CenterOwner },
                { "Owner", Window },
            };

            return WindowManager.ShowDialog(viewModel, null, cfg);
        }

        public Task<ProgressDialogController> ShowLoading(string message = "")
        {
            return ShowProgress("Loading", message == "" ?
                "Data is being loaded from the database." : message);
        }

        public void ToFront()
        {
            CheckMetroWindowExists();
            Window.Topmost = false;
            Window.Activate();
        }

        public void Shrink()
        {
            CheckMetroWindowExists();
            Window.WindowState = WindowState.Minimized;
        }

        public Task Darken()
        {
            CheckMetroWindowExists();
            return Window.ShowOverlayAsync();
        }

        public Task LightUp()
        {
            CheckMetroWindowExists();
            return Window.HideOverlayAsync();
        }

        private void CheckMetroWindowExists()
        {
            if (Window == null)
            {
                throw new InvalidOperationException("Must assign the window instance before using ViewService.");
            }
        }
    }

    public interface IViewService
    {
        IWindowManager WindowManager { get; }
        MetroWindow Window { set; }
        Task<string> ShowInputDialog(string title, string message, MetroDialogSettings settings = null);

        Task<MessageDialogResult> ShowMessage(string title, string message,
            bool isAffirmative = true, bool isAccentedColor = true,
            string affirmativeButtonText = "Ok", string negativeButtonText = "Cancel");

        Task<ProgressDialogController> ShowProgress(string title, string message,
            bool isAccentedColor = false);

        void ShowWindow(ViewModelBase viewModel);
        void ShowIndependentWindow(ViewModelBase viewModel);
        bool? ShowDialog(ViewModelBase viewModel);
        Task<ProgressDialogController> ShowLoading(string message = "");
        void ToFront();
        void Shrink();
        Task Darken();
        Task LightUp();
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Trading.Common.ViewModels;

namespace Trading.Common.Utils
{
    public class ViewService : IViewService
    {
        private MetroWindow window;
        private ViewModelBase mainViewModel;

        /// <summary>
        /// Initialize a view service which can bring up metro-style dialogs / windows,
        /// or other controlling actions.
        /// The view being operated on is usually the main view, which depends on the
        /// place where the dependency injection happens. If you don't assign <see cref="Window"/>
        /// property, you cannot use any of the methods except <see cref="RegisterChild(Trading.Common.Utils.IUseParentViewService[])"/>.
        /// </summary>
        /// <param name="wm"></param>
        public ViewService(IWindowManager wm)
        {
            WindowManager = wm;
        }

        public IWindowManager WindowManager { private set; get; }

        public MetroWindow Window
        {
            get { return window; }
            set
            {
                window = value;
                try
                {
                    mainViewModel = (ViewModelBase)window.DataContext;
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException("The provided view must have an instance of type ViewModelBase as its DataContext.", e);
                }
            }
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

        public async void ShowWindow(ViewModelBase viewModel)
        {
            CheckMetroWindowExists();
            var cfg = new Dictionary<string, object>
            {
                { "WindowStartupLocation", WindowStartupLocation.CenterOwner },
                { "Owner", Window },
            };

            await Task.Factory.StartNew(() => WindowManager.ShowWindow(viewModel, null, cfg),
                CancellationToken.None, TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public async void ShowIndependentWindow(ViewModelBase viewModel)
        {
            CheckMetroWindowExists();
            var cfg = new Dictionary<string, object>
            {
                { "WindowStartupLocation", WindowStartupLocation.CenterOwner },
            };

            await Task.Factory.StartNew(() => WindowManager.ShowWindow(viewModel, null, cfg),
                CancellationToken.None, TaskCreationOptions.None,
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        public Task<bool?> ShowDialog(ViewModelBase viewModel)
        {
            CheckMetroWindowExists();
            var cfg = new Dictionary<string, object>
            {
                { "WindowStartupLocation", WindowStartupLocation.CenterOwner },
                { "Owner", Window },
            };

            return Task.Factory.StartNew(() => WindowManager.ShowDialog(viewModel, null, cfg),
                   CancellationToken.None, TaskCreationOptions.None,
                   TaskScheduler.FromCurrentSynchronizationContext());
        }

        public Task<MessageDialogResult> AskSave()
        {
            return ShowMessage(Texts.BeforeSavePromptTitle, Texts.BeforeSavePromptContent,
                false, true,
                Texts.BeforeSavePromptPositiveDecision, Texts.BeforeSavePromptNegativeDecision);
        }

        public Task<MessageDialogResult> AskDelete()
        {
            return ShowMessage(Texts.BeforeDeletePromptTitle, Texts.BeforeDeletePromptContent,
                false, true,
                Texts.BeforeDeletePromptPositiveDecision, Texts.BeforeDeletePromptNegativeDecision);
        }

        public Task<MessageDialogResult> AskWarning(string message = "")
        {
            return ShowMessage(Texts.BeforeWarningPromptTitle, message == "" ? Texts.BeforeWarningPromptContent : message,
                false, true,
                Texts.BeforeDeletePromptPositiveDecision, Texts.BeforeDeletePromptNegativeDecision);
        }

        public Task<MessageDialogResult> ShowError(string message = "")
        {
            return ShowMessage(Texts.ErrorPromptTitle, message == "" ? Texts.ErrorPromptContent : message
                , true, true, Texts.ErrorPromptDecision);
        }

        public Task<ProgressDialogController> ShowLoading(string message = "")
        {
            return ShowProgress("Loading", message == "" ?
                "Data is being loaded from the database." : message);
        }

        public Task<MessageDialogResult> ShowWarning(string message = "")
        {
            return ShowMessage(Texts.WarningPromptTitle, message == "" ? Texts.WarningPromptContent : message
                , true, true, Texts.WarningPromptDecision);
        }

        public bool HasError<T>() where T : DependencyObject
        {
            var e = window.FindChildren<T>();
            foreach (var element in e)
            {
                if (Validation.GetHasError(element))
                {
                    return true;
                }
            }
            return false;
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

    public static class ViewServiceExtensions
    {
        /// <summary>
        /// Stop the progress indicator.
        /// </summary>
        /// <param name="progress"></param>
        /// <returns></returns>
        public static Task Stop(this ProgressDialogController progress)
        {
            return progress.IsOpen ? progress.CloseAsync() : null;
        }

        /// <summary>
        /// Append a new piece of message to the initial message for the progress indicator.
        /// </summary>
        /// <param name="progress"></param>
        /// <param name="additionalMessage"></param>
        /// <returns></returns>
        public static void AppendMessageForLoading(this ProgressDialogController progress,
            string additionalMessage)
        {
            progress.SetMessage(progress.InitialMessage + Environment.NewLine + additionalMessage);
        }
    }
}
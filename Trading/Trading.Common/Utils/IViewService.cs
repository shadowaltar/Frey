using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Trading.Common.ViewModels;

namespace Trading.Common.Utils
{
    public interface IViewService
    {
        IWindowManager WindowManager { get; }
        MetroWindow Window { set; }

        /// <summary>
        /// Show an input dialog.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        Task<string> ShowInputDialog(string title, string message, MetroDialogSettings settings = null);

        /// <summary>
        /// Show a message.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="isAffirmative"></param>
        /// <param name="isAccentedColor"></param>
        /// <param name="affirmativeButtonText"></param>
        /// <param name="negativeButtonText"></param>
        /// <returns></returns>
        Task<MessageDialogResult> ShowMessage(string title, string message,
            bool isAffirmative = true, bool isAccentedColor = false,
            string affirmativeButtonText = "Ok", string negativeButtonText = "Cancel");

        /// <summary>
        /// Show a metro-style progress indicator in the window.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="isAccentedColor"></param>
        /// <returns></returns>
        Task<ProgressDialogController> ShowProgress(string title, string message, bool isAccentedColor = false);

        /// <summary>
        /// Show a window by its ViewModel, while setting the Owner to be the caller.
        /// </summary>
        /// <param name="viewModel"></param>
        void ShowWindow(ViewModelBase viewModel);

        /// <summary>
        /// Show a window by its ViewModel, without setting the Owner to be the caller.
        /// </summary>
        /// <param name="viewModel"></param>
        void ShowIndependentWindow(ViewModelBase viewModel);

        /// <summary>
        /// Show a modal dialog by its ViewModel.
        /// </summary>
        /// <param name="viewModel"></param>
        Task<bool?> ShowDialog(ViewModelBase viewModel);

        /// <summary>
        /// Dim the window.
        /// </summary>
        /// <returns></returns>
        Task Darken();

        /// <summary>
        /// Brighten the window (reverse the Dim()).
        /// </summary>
        /// <returns></returns>
        Task LightUp();

        /// <summary>
        /// Bring the window to the front.
        /// </summary>
        void ToFront();

        /// <summary>
        /// Minimize the window to the taskbar.
        /// </summary>
        void Shrink();

        /// <summary>
        /// Ask wanna save?
        /// </summary>
        /// <returns></returns>
        Task<MessageDialogResult> AskSave();

        /// <summary>
        /// Ask confirm to delete?
        /// </summary>
        /// <returns></returns>
        Task<MessageDialogResult> AskDelete();

        /// <summary>
        /// Ask wanna save?
        /// </summary>
        /// <returns></returns>
        Task<MessageDialogResult> AskWarning(string message = "");

        /// <summary>
        /// Show an error message, with button OK.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<MessageDialogResult> ShowError(string message = "");

        /// <summary>
        /// Show a loading message with progress indicator.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<ProgressDialogController> ShowLoading(string message = "");

        /// <summary>
        /// Show a warning message, with button OK.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<MessageDialogResult> ShowWarning(string message = "");

        /// <summary>
        /// Check if any <see cref="DependencyObject"/> in the UI has error.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool HasError<T>() where T : DependencyObject;
    }
}
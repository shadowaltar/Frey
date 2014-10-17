using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Trading.Common;
using Trading.Common.SharedSettings;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.Portal.ApplicationSettings;
using Trading.Portal.ViewModels;
using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;

namespace Trading.Portal
{
    public sealed class PortalViewModel : ViewModelBase, IPortalViewModel,
        IHandle<ActivityMessage<IPortalViewModel>>
    {
        public PortalViewModel(IViewService viewService,
            IEventAggregator eventAggregator,
            IOptionsFlyoutViewModel optionsFlyout,
            InfrastructureSettings infrastructureSettings)
        {
            this.viewService = viewService;
            EventAggregator = eventAggregator;
            EventAggregator.Subscribe(this);
            DisplayName = "Trading Portal";

            OptionsFlyout = optionsFlyout;

            environment = infrastructureSettings.DefaultEnvironment;

            desiredTilesGridDimensions[1] = new[] { 1, 1 };
            desiredTilesGridDimensions[2] = new[] { 1, 2 };
            desiredTilesGridDimensions[3] = new[] { 2, 2 };
            desiredTilesGridDimensions[4] = new[] { 2, 2 };
            desiredTilesGridDimensions[5] = new[] { 3, 2 };
            desiredTilesGridDimensions[6] = new[] { 3, 2 };
            desiredTilesGridDimensions[7] = new[] { 3, 3 };
            desiredTilesGridDimensions[8] = new[] { 3, 3 };
            desiredTilesGridDimensions[9] = new[] { 3, 3 };
            desiredTilesGridDimensions[10] = new[] { 4, 3 };
            desiredTilesGridDimensions[11] = new[] { 4, 3 };
            desiredTilesGridDimensions[12] = new[] { 4, 3 };
            desiredTilesGridDimensions[13] = new[] { 5, 3 };
            desiredTilesGridDimensions[14] = new[] { 5, 3 };
            desiredTilesGridDimensions[15] = new[] { 5, 3 };
        }

        private readonly IViewService viewService;
        private readonly JumpList jumpList = new JumpList();

        private readonly Dictionary<string, ScreenInformation> screens = new Dictionary<string, ScreenInformation>();
        private readonly List<ViewModelBase> openedScreens = new List<ViewModelBase>();

        private readonly Dictionary<int, int[]> desiredTilesGridDimensions = new Dictionary<int, int[]>();
        private int screenCount;
        private WrapPanel buttonContainer;

        public IOptionsFlyoutViewModel OptionsFlyout { get; set; }

        private bool isOptionsFlyoutOpen;
        private string environment = "DEV";

        public bool IsOptionsFlyoutOpen
        {
            get { return isOptionsFlyoutOpen; }
            set { SetNotify(ref isOptionsFlyoutOpen, value); }
        }

        protected override void OnViewLoaded(object view)
        {
            var window = (MetroWindow)view;
            base.OnViewLoaded(window);
            viewService.Window = window;

            buttonContainer = window.FindChild<WrapPanel>("Screens");
            InitializeScreenSettings();

            SetWindowLocation(window);
            InitializeJumpList();
        }

        /// <summary>
        /// Open a Trading screen.
        /// </summary>
        /// <param name="screenKey"></param>
        public void Launch(string screenKey)
        {
            try
            {
                if (!screens[screenKey].IsLoaded)
                {
                    Load(Bootstrapper.Kernel, screens[screenKey]);
                }

                var vm = Bootstrapper.Kernel.Get(screens[screenKey].MainViewModelInterfaceType) as ViewModelBase;
                if (vm != null)
                {
                    if (vm is IHasEnvironment)
                    {
                        ((IHasEnvironment)vm).Environment = environment;
                    }
                    viewService.ShowIndependentWindow(vm);
                    openedScreens.Add(vm);
                }
                else
                {
                    throw new InvalidOperationException("Encountered a screen object resolved which is not as of type ViewModelBase");
                }
            }
            catch (Exception e)
            {
                Log.Error("Cannot launch screen " + screenKey, e);
                viewService.ShowError("Cannot launch screen \"" + screenKey + "\"");
            }
        }

        public void ToggleOptions()
        {
            IsOptionsFlyoutOpen = !IsOptionsFlyoutOpen;
            OptionsFlyout.SelectedEnvironment = environment;
        }

        private async void InitializeScreenSettings()
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // in case cannot get the assembly path, close the app.
            if (folder == null)
            {
                Log.Error("Failed to get the directory of current assembly's execution file.");
                await viewService.ShowError("Failed to initialize screen settings. The application would be closed.");
                Application.Current.Shutdown(1);
                return;
            }

            // check if in app.config file the screens settings are there.
            ScreenSection section;
            try
            {
                section = ConfigurationManager.GetSection("Screens") as ScreenSection;
            }
            catch (Exception)
            {
                Log.Error("Cannot read the screen information. The application would be closed.");
                Application.Current.Shutdown(1);
                return;
            }

            if (section != null)
            {
                var elems = section.Screens;
                foreach (ScreenElement elem in elems)
                {
                    try
                    {
                        var tileColor = ColorConverter.ConvertFromString(elem.TileColor);
                        if (tileColor == null)
                        {
                            Log.Warn("Cannot initialize color for screen setting in app.config: " + elem);
                        }

                        var si = new ScreenInformation
                        {
                            AssemblyName = elem.AssemblyName,
                            AssemblyFilePath = Path.Combine(folder, elem.ExecutableName),
                            DisplayName = elem.Name,
                            ThemeColor = tileColor == null ? Colors.Black : (Color)tileColor,
                        };

                        screens[si.AssemblyName] = si;

                        // build tile button
                        buttonContainer.Children.Add(new PortalTile(si));
                        screenCount++;
                    }
                    catch (Exception e)
                    {
                        Log.Error("Cannot initialize the screen info!", e);
                    }
                }
            }
        }

        /// <summary>
        /// initialize the taskbar jumplist; must be run after init of screen info.
        /// </summary>
        private void InitializeJumpList()
        {
            foreach (var config in screens)
            {
                var jumpTask = new JumpTask
                {
                    Title = config.Value.DisplayName,
                    IconResourcePath = config.Value.AssemblyFilePath,
                    IconResourceIndex = 0,
                    ApplicationPath = config.Value.AssemblyFilePath,
                };
                jumpList.JumpItems.Add(jumpTask);
            }
            jumpList.Apply();
            JumpList.SetJumpList(Application.Current, jumpList);
        }

        /// <summary>
        /// Get all assemblies (of type DLL or EXE) in the same folder of the current
        /// executing application.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetScreenAssemblyFilePaths()
        {
            var cd = Directory.GetCurrentDirectory();
            return Directory.EnumerateFiles(cd, "Trading*.*")
                .Where(f => f.ToUpper().EndsWith("DLL") || f.ToUpper().EndsWith("EXE"));
        }

        /// <summary>
        /// Load a screen from the assembly file, prepare the bindings for Ninject and
        /// Caliburn Micro.
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="info"></param>
        public void Load(IKernel kernel, ScreenInformation info)
        {
            Type mainViewModelInterface = null;
            Type mainViewModelImpl = null;

            var assembly = Assembly.LoadFrom(info.AssemblyFilePath);
            foreach (var type in assembly.GetTypes())
            {
                if (type.Name == "Module" && type.IsSubclassOf(typeof(NinjectModule)))
                {
                    // let Ninject be able to do IoC using info defined in the Module classes.
                    kernel.Load((NinjectModule)Activator.CreateInstance(type));

                    info.IsLoaded = true;
                }
                if (type.Name.Contains("IMainViewModel") && type.IsInterface)
                {
                    mainViewModelInterface = type;
                }
                if (type.Name.Contains("MainViewModel") && type.IsClass)
                {
                    mainViewModelImpl = type;
                }
            }

            // if mainVM is an impl of mainVMInterface, store this relationship.
            if (mainViewModelImpl != null && mainViewModelInterface != null
                && mainViewModelImpl.GetInterfaces().Contains(mainViewModelInterface))
            {
                info.MainViewModelInterfaceType = mainViewModelInterface;
                info.MainViewModelImplementationType = mainViewModelImpl;
            }

            // tell caliburn.micro that this assembly contains MVVM bindings.
            AssemblySource.Instance.Add(assembly);
        }

        public void HandleShortcutKeys(KeyEventArgs e)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                switch (e.Key)
                {
                    case Key.O: ToggleOptions(); break;
                }
            }
            if (e.Key == Key.Escape)
            {
                if (IsOptionsFlyoutOpen) ToggleOptions();
            }
        }

        public void Handle(ActivityMessage<IPortalViewModel> message)
        {
            switch (message.Type)
            {
                case ActivityType.ChangeEnvironment:
                    environment = message.Item as string;
                    break;
            }
        }

        private void SetWindowLocation<T>(T window) where T : Window
        {
            int[] dimensions;
            if (desiredTilesGridDimensions.TryGetValue(screenCount, out dimensions))
            {
                var width = 436 + 316 * (dimensions[0] - 1);
                var height = 317 + 156 * (dimensions[1] - 1);

                window.Height = height;
                window.Width = width;
                window.Left = (SystemParameters.FullPrimaryScreenWidth - width) / 2;
                window.Top = (SystemParameters.FullPrimaryScreenHeight - height) / 2;
                window.WindowStartupLocation = WindowStartupLocation.Manual;
            }
            else
            {
                window.WindowState = WindowState.Maximized;
            }
        }

        /// <summary>
        /// Handles close window command; ask user if want to close all windows or only portal.
        /// </summary>
        /// <param name="callback"></param>
        public async override void CanClose(Action<bool> callback)
        {
            if (openedScreens.Count == 0)
            {
                callback(true);
                return;
            }

            var result = await viewService.ShowMessage("Do you want to close all the windows?",
                    "Yes to close the portal and all screens." + Environment.NewLine + "No to close portal only."
                    , false, true, "Yes", "No");
            if (result == MessageDialogResult.Affirmative)
            {
                foreach (var screen in openedScreens)
                {
                    screen.TryClose();
                }
            }
            callback(true);
        }
    }
}
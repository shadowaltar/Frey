using Caliburn.Micro;
using MahApps.Metro.Controls;
using Trading.Common.Utils;
using Trading.Common.ViewModels;
using Trading.Portal.ApplicationSettings;
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
using System.Windows.Media;
using System.Windows.Shell;

namespace Trading.Portal
{
    public sealed class PortalViewModel : ViewModelBase, IPortalViewModel
    {
        public PortalViewModel(IViewService viewService)
        {
            this.viewService = viewService;
            DisplayName = "Trading Portal";

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

        private readonly Dictionary<string, ScreenInformation> screens = new Dictionary<string, ScreenInformation>();
        private readonly List<ViewModelBase> openedScreens = new List<ViewModelBase>();

        private readonly Dictionary<int, int[]> desiredTilesGridDimensions = new Dictionary<int, int[]>();
        private int screenCount;
        private WrapPanel buttonContainer;

        protected override void OnLoaded(MetroWindow view)
        {
            EventAggregator.Subscribe(this);
            viewService.Window = view;
            buttonContainer = view.FindChild<WrapPanel>("Screens");
            InitializeScreenSettings();

            SetWindowLocation(view);
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
        public override void CanClose(Action<bool> callback)
        {
            if (openedScreens.Count == 0)
            {
                callback(true);
                return;
            }

            foreach (var screen in openedScreens)
            {
                screen.TryClose();
            }
            callback(true);
        }
    }
}
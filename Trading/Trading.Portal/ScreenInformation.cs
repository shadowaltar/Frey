using System;
using System.Windows.Media;

namespace Trading.Portal
{
    /// <summary>
    /// Class that represents a screen (only) in Portal. It contains info about the screen's
    /// assembly information and start-up class information.
    /// </summary>
    public class ScreenInformation
    {
        /// <summary>
        /// Get/set the assembly name of the screen exe/dll, which is usually its namespace.
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Get/set the name of the screen being shown in the portal.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Get/set the color of the tile button for this screen being shown in the portal.
        /// </summary>
        public Color ThemeColor { get; set; }

        /// <summary>
        /// Get/set the full file path (folder plus file name) for the screen's exe/dll.
        /// </summary>
        public string AssemblyFilePath { get; set; }

        /// <summary>
        /// Get/set the type instance of the start-up class's implementing interface. Conventionally
        /// it should be the IMainViewModel.
        /// </summary>
        public Type MainViewModelImplementationType { get; set; }

        /// <summary>
        /// Get/set the type instance of the start-up class. Conventionally
        /// it should be the MainViewModel.
        /// </summary>
        public Type MainViewModelInterfaceType { get; set; }

        /// <summary>
        /// Get/set if this screen's assembly loaded into the memory by <see cref="PortalViewModel"/>.
        /// </summary>
        public bool IsLoaded { get; set; }
    }
}
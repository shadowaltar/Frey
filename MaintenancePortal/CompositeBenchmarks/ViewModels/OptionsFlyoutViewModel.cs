using System.Collections.Generic;
using Maintenance.Common;
using Maintenance.Common.SharedSettings;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;

namespace Maintenance.CompositeBenchmarks.ViewModels
{
    public class OptionsFlyoutViewModel : OptionsViewModelBase<IMainViewModel>, IOptionsFlyoutViewModel
    {
        public OptionsFlyoutViewModel(ISettings settings)
        {
            Settings = settings;
        }

        private bool isShowNonDefaultCompositeBenchmarks;
        public bool IsShowInactiveCompositeBenchmarks
        {
            get { return isShowNonDefaultCompositeBenchmarks; }
            set
            {
                SetNotify(ref isShowNonDefaultCompositeBenchmarks, value);
                if (IsPublishingPreference)
                {
                    Publish<IMainViewModel>(ActivityType.ChangePreference,
                        new KeyValuePair<string, bool>("IsShowNonDefaultCompositeBenchmarks", value));
                }
            }
        }

        private bool isShowExpiredCompositeBenchmarks;
        public bool IsShowExpiredCompositeBenchmarks
        {
            get { return isShowExpiredCompositeBenchmarks; }
            set
            {
                SetNotify(ref isShowExpiredCompositeBenchmarks, value);
                if (IsPublishingPreference)
                {
                    Publish<IMainViewModel>(ActivityType.ChangePreference,
                        new KeyValuePair<string, bool>("IsShowExpiredCompositeBenchmarks", value));
                }
            }
        }

        private bool isShowAllBenchmarkComponents;
        public bool IsShowAllBenchmarkComponents
        {
            get { return isShowAllBenchmarkComponents; }
            set
            {
                SetNotify(ref isShowAllBenchmarkComponents, value);
                if (IsPublishingPreference)
                {
                    Publish<IMainViewModel>(ActivityType.ChangePreference,
                        new KeyValuePair<string, bool>("IsShowAllBenchmarkComponents", value));
                }
            }
        }

        private bool isAutoPopulateNewCompositeBenchmark;
        public bool IsAutoPopulateNewCompositeBenchmark
        {
            get { return isAutoPopulateNewCompositeBenchmark; }
            set
            {
                SetNotify(ref isAutoPopulateNewCompositeBenchmark, value);
                if (IsPublishingPreference)
                {
                    Publish<IMainViewModel>(ActivityType.ChangePreference,
                        new KeyValuePair<string, bool>("IsAutoPopulateNewCompositeBenchmark", value));
                }
            }
        }

        /// <summary>
        /// Get/set if the option view publish the preference changes to the listeners.
        /// </summary>
        public bool IsPublishingPreference { get; set; }
    }
}
﻿using Maintenance.Common;
using Maintenance.Common.SharedSettings;
using Maintenance.Common.ViewModels;

namespace Maintenance.CountryOverrides.ViewModels
{
    public class OptionsFlyoutViewModel : OptionsViewModelBase<IMainViewModel>, IOptionsFlyoutViewModel
    {
        public OptionsFlyoutViewModel(ISettings settings)
        {
            Settings = settings;
        }
    }
}
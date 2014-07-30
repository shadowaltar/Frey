using Caliburn.Micro;
using Maintenance.AssetMaps.Utils;
using Maintenance.Common.Entities;
using Maintenance.Common.Utils;
using Maintenance.Common.ViewModels;

namespace Maintenance.AssetMaps.ViewModels
{
    public class ComponentPropertyViewModel : ViewModelBaseSlim
    {
        public AssetMapComponentProperty Property { get; private set; }

        public ComponentPropertyViewModel(AssetMapComponentProperty property)
        {
            Property = property;
        }

        public string Key
        {
            get
            {
                return Property.Key;
            }
            set
            {
                // populate possible values when a key is selected.
                if (Property.Key == null && value != null)
                {
                    Options.ClearAndAddRange(AssetMapComponentPropertyHelper.GetOptions(value));
                }
                if (!Equals(Property.Key, value))
                {
                    Property.Key = value;
                    Notify();
                }
            }
        }

        public object Value
        {
            get
            {
                return Property.Value;
            }
            set
            {
                if (!Equals(Property.Value, value))
                {
                    Property.Value = value;
                    Notify();
                }
            }
        }

        private readonly BindableCollection<string> propertyKeys = new BindableCollection<string>();
        public BindableCollection<string> PropertyKeys { get { return propertyKeys; } }

        private readonly BindableCollection<object> options = new BindableCollection<object>();
        public BindableCollection<object> Options { get { return options; } }
    }
}
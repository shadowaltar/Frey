using Maintenance.Common.ViewModels;

namespace Maintenance.Benchmarks.ViewModels
{
    public class FilterFlyoutViewModel : FilterViewModelBase<IMainViewModel>, IFilterFlyoutViewModel
    {
        public bool IsReady { get; set; }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (SetNotify(ref name, value) && IsReady) // empty string is useful here
                    Filter("StartOfName", value);
            }
        }

        private string code;
        public string Code
        {
            get { return code; }
            set
            {
                if (SetNotify(ref code, value) && IsReady) // empty string is useful here
                    Filter("StartOfCode", value);
            }
        }

        /// <summary>
        /// Clear all fields.
        /// </summary>
        public void ClearAll()
        {
            Name = string.Empty;
            Code = string.Empty;

            CurrentOptions.Clear();
        }

        /// <summary>
        /// Clear all fields and also reset main view's list of items.
        /// </summary>
        public void Reset()
        {
            ClearAll();
            ResetFilterTarget();
        }
    }
}
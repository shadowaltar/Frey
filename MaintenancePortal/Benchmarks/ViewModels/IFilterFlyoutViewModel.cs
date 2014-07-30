namespace Maintenance.Benchmarks.ViewModels
{
    public interface IFilterFlyoutViewModel
    {
        bool IsReady { get; set; }
        string Name { get; set; }
        string Code { get; set; }

        /// <summary>
        /// Clear all fields and also reset main view's list of items.
        /// </summary>
        void Reset();

        /// <summary>
        /// Clear all fields.
        /// </summary>
        void ClearAll();
    }
}
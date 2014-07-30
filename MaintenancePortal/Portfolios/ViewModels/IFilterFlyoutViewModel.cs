namespace Maintenance.Portfolios.ViewModels
{
    public interface IFilterFlyoutViewModel
    {
        bool IsReady { get; set; }
        string Id { get; }
        string Code { get; set; }
        string Name { get; set; }
        string PortfolioManagerName { get; set; }
        string BenchmarkCode { get; set; }

        void ClearAllFields();
    }
}
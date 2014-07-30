namespace Maintenance.Common.Utils
{
    public interface IHasViewService
    {
        IViewService ViewService { get; set; }
    }
}
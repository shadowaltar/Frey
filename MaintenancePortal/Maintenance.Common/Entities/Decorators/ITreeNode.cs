namespace Maintenance.Common.Entities.Decorators
{
    /// <summary>
    /// Interface for the classes where they are expected to have
    /// UI tree node behaviors.
    /// </summary>
    public interface ITreeNode
    {
        bool IsSelected { get; set; }
        bool IsExpanded { get; set; }
        bool IsVisible { get; set; }
    }
}
using Caliburn.Micro;

namespace Trading.Common.Entities.Decorators
{
    /// <summary>
    /// Indicates if an object has a ViewModel-awared list of children items.
    /// It is used for a parent node in a tree.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasChildNodes<T>
    {
        BindableCollection<T> Children { get; }
    }
}
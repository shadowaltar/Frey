using System.ComponentModel;

namespace Trading.Common.Entities.Decorators
{
    /// <summary>
    /// Indicates if an object has a ViewModel-awared list of children items.
    /// It is used for a parent node in a tree.
    /// It is different from <see cref="IHasChildNodes{T}"/>, as the children
    /// is grouped inside a <see cref="BindingList{T}"/>, which any property change
    /// of a list element would raise changed event.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasBindingChildNodes<T>
    {
        BindingList<T> Children { get; }
    }
}
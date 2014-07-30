using Caliburn.Micro;
using System.Runtime.CompilerServices;

namespace GritsMaintenance.Utils
{
    public static class UiExtensions
    {
        /// <summary>
        /// User CallerMemberName to get the name of the caller property
        /// as a string. Requires Microsoft.Bcl package.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="screen"></param>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool SetNotify<T>(this PropertyChangedBase screen, ref T target, T value,
            [CallerMemberName]string propertyName = null)
        {
            if (!Equals(target, value))
            {
                target = value;
                screen.NotifyOfPropertyChange(propertyName);
                return true;
            }
            return false;
        }
    }
}
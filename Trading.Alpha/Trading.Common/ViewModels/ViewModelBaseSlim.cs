using System.Runtime.CompilerServices;
using Caliburn.Micro;

namespace Trading.Common.ViewModels
{
    public class ViewModelBaseSlim : PropertyChangedBase
    {
        public bool SetNotify<T>(ref T target, T value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(target, value))
            {
                target = value;
                NotifyOfPropertyChange(propertyName);
                return true;
            }
            return false;
        }

        public void Notify([CallerMemberName] string propertyName = null)
        {
            NotifyOfPropertyChange(propertyName);
        }
    }
}
using System.Runtime.CompilerServices;
using Caliburn.Micro;

namespace Automata.Dashboard
{
    public class ViewModelBase : Screen
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
    }
}
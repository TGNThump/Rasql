using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace GroupProjectRASQL.Framework
{
    public abstract class Reactive : INotifyPropertyChanging, INotifyPropertyChanged
    {
        // Use https://github.com/Fody/PropertyChanged to automagicly generate PropertyChanging/Changed Notifications
        //  so Neutronium can hook properties reactivly to Vue.
        #pragma warning disable 0067  
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
        #pragma warning restore 0067

        public bool Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(property, value))
                return false;

            PropertyIsChanging(propertyName);
            property = value;
            PropertyHasChanged(propertyName);
            return true;
        }

        protected void PropertyHasChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void PropertyIsChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }
    }
}

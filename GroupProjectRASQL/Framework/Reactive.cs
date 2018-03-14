using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}

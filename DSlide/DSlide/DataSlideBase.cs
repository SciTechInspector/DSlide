using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DSlide
{
    public class DataSlideBase : INotifyPropertyChanged
    {
        protected DataManager dataManager;

        public event PropertyChangedEventHandler PropertyChanged;

        public DataSlideBase()
        {
            this.dataManager = DataManager.Current;
        }

        protected void SendNotification(PropertyChangedEventArgs args)
        {
            this.PropertyChanged?.Invoke(this, args);
        }



        public void NotifyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetSourceValue<T>(T newValue, Action doNotify = null,[CallerMemberName] string propertyName = null) 
        {
            this.dataManager.SetSourceValue(newValue, this, propertyName, doNotify ?? (() => this.NotifyChanged(propertyName)));
        }
        public T GetSourceValue<T>(Action doNotify = null, [CallerMemberName] string propertyName = null) 
        {
            return (T)this.dataManager.GetSourceValue(this, propertyName, doNotify ?? (() => this.NotifyChanged(propertyName)));
        }

        public T GetComputedValue<T>(Func<object> computer, Action doNotify = null, [CallerMemberName] string propertyName = null) 
        {
            return (T)this.dataManager.GetComputedValue(computer, this, propertyName, doNotify ?? (() => this.NotifyChanged(propertyName)));
        }
    }
}

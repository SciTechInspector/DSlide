using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DSlide
{
    public class BaseClass : INotifyPropertyChanged
    {
        DataManager dataManager = new DataManager();

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetSourceValue<T>(T newValue, [CallerMemberName] string propertyName = null) 
        {
            this.dataManager.SetSourceValue(newValue, this, propertyName, () => this.NotifyChanged());
        }
        public T GetSourceValue<T>([CallerMemberName] string propertyName = null) 
        {
            return this.dataManager.GetSourceValue<T>(this, propertyName, () => this.NotifyChanged());
        }

        public T GetComputedValue<T>(Func<T> computer, [CallerMemberName] string propertyName = null) 
        {
            return this.dataManager.GetComputedValue<T>(computer, this, propertyName, () => this.NotifyChanged());
        }
    }
}

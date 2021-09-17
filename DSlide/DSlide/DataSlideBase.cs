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


        public void NotifyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetSourceValue<T>(T newValue, [CallerMemberName] string propertyName = null) 
        {
            this.dataManager.SetSourceValue(newValue, this, propertyName, () => this.NotifyChanged(propertyName));
        }
        public T GetSourceValue<T>([CallerMemberName] string propertyName = null) 
        {
            return (T)this.dataManager.GetSourceValue(this, propertyName, () => this.NotifyChanged(propertyName));
        }

        public T GetComputedValue<T>(Func<object> computer, [CallerMemberName] string propertyName = null) 
        {
            return (T)this.dataManager.GetComputedValue(computer, this, propertyName, () => this.NotifyChanged(propertyName));
        }
    }
}

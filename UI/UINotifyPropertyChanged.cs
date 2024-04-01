using System.ComponentModel;

namespace xLibV100.UI
{
    public abstract class UINotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event xPropertyChangedEventHandler<object, PropertyChangedEventHandlerArg<object>> ValuePropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(string propertyName, object state)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            ValuePropertyChanged?.Invoke(this, new PropertyChangedEventHandlerArg<object>
            {
                Name = propertyName,
                State = state
            });
        }
    }
}

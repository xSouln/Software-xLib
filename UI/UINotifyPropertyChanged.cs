using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace xLibV100.UI
{
    public abstract class UINotifyPropertyChanged : INotifyPropertyChanged, IDisposable, INotifyCollectionChanged
    {
        /// <summary>
        /// событие изменения значения свойства
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// событие изменений коллекций
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// событие изменения значения свойства с возможностью просмотра нового состояния
        /// </summary>
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

        public virtual void Dispose()
        {
            PropertyChanged = null;
            ValuePropertyChanged = null;
            CollectionChanged = null;
        }

        protected virtual void OnCollectionReset(IEnumerable collection)
        {
            CollectionChanged?.Invoke(collection, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected virtual void OnCollectionAdd(IEnumerable collection, object element)
        {
            CollectionChanged?.Invoke(collection, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, element));
        }

        protected virtual void OnCollectionRemove(IEnumerable collection, object element)
        {
            CollectionChanged?.Invoke(collection, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, element));
        }
    }
}

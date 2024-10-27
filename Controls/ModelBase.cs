using System.Collections;
using System.Collections.Specialized;
using xLibV100.Interfaces;
using xLibV100.UI;

namespace xLibV100.Controls
{
    /// <summary>
    /// класс определяющий базовый набор для модели
    /// </summary>
    public abstract class ModelBase : UINotifyPropertyChanged, IInheritable, INotifyCollectionChanged
    {
        protected object parent;
        protected string name;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ModelBase()
        {

        }

        public ModelBase(object parent)
        {
            this.parent = parent;
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

        public virtual object Parent => parent;

        /// <summary>
        /// имя модели
        /// </summary>
        public string Name
        {
            get => name;
            set
            {
                if (value != name)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name), name);
                }
            }
        }

        public virtual object GetParent()
        {
            return parent;
        }

        public override void Dispose()
        {
            base.Dispose();

            CollectionChanged = null;
        }
    }

    /// <summary>
    /// класс определяющий базовый набор для модели
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    public class ModelBase<TParent> : ModelBase where TParent : class
    {
        public new TParent Parent
        {
            get => parent != null ? (TParent)parent : null;
            set => parent = value;
        }

        public ModelBase(TParent model)
        {
            Parent = model;
        }
    }
}

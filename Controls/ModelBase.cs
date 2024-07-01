using xLibV100.UI;

namespace xLibV100.Controls
{
    /// <summary>
    /// класс определяющий базовый набор для модели
    /// </summary>
    public abstract class ModelBase : UINotifyPropertyChanged
    {
        protected object parent;
        protected string name;

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
    }

    /// <summary>
    /// класс определяющий базовый набор для модели
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    public class ModelBase<TParent> : ModelBase where TParent : ModelBase
    {
        public TParent Parent
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

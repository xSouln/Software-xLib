﻿using System;
using xLibV100.Interfaces;
using xLibV100.UI;

namespace xLibV100.Controls
{
    /// <summary>
    /// класс определяющий базовый набор для модели
    /// </summary>
    public abstract class ModelBase : UINotifyPropertyChanged, IInheritable, IDisposable
    {
        protected object parent;
        protected string name;

        public ModelBase()
        {

        }

        public ModelBase(object parent)
        {
            this.parent = parent;
        }

        public virtual object Parent => parent;

        /// <summary>
        /// имя модели
        /// </summary>
        public virtual string Name
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

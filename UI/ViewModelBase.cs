using xLibV100.UI;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace xLibV100.UI
{
    public class ViewModelBase : UINotifyPropertyChanged, IDisposable
    {
        public class ChildWindowEventListenerArg
        {
            public object Initiator;
            public FrameworkElement View;
            public string PropertyName;
            public object Property;
            public object Content;
        }

        public class ViewModelEventListenerArg
        {
            public ViewModelBase Initiator;
            public object Event;
            public object EventArg;
            public (string description, object arg)[] Content;

            public TResult FindConentArg<TResult>()
            {
                foreach (var element in Content)
                {
                    if (element.arg is TResult)
                    {
                        return (TResult)element.arg;
                    }
                }

                return default;
            }
        }

        public delegate void ViewModelCellSizeChangedHandler(ViewModelBase viewModel, ICellElement element);
        public delegate void ViewModelVisibilityChangedHandler(ViewModelBase viewModel, object arg);
        public delegate void ViewModelUpdateEventHandler(ViewModelBase viewModel);
        public delegate void ViewModelViewUpdateEventHandler(ViewModelBase viewModel);
        public delegate void ViewModelApplyEventHandler(ViewModelBase viewModel, object result);

        protected string name;
        protected object model;
        protected object view;
        protected object parent;

        protected DataTemplate template;
        protected ObservableCollection<object> properties;
        protected ObservableCollection<object> models;

        public event ViewModelCellSizeChangedHandler CellSizeChanged;
        public event ViewModelUpdateEventHandler UpdateEvent;
        public event ViewModelViewUpdateEventHandler ViewUpdateEvent;
        public event ViewModelApplyEventHandler ApplyEvent;

        public event EventHandler<ChildWindowEventListenerArg> ViewEventListener;
        public event EventHandler<ViewModelEventListenerArg> EventListener;

        public ObservableCollection<RelayCommand> Commands { get; set; } = new ObservableCollection<RelayCommand>();

        public object SelectedModel { get; set; }

        public virtual void PropertyChangedRequestListener(object sender, object dataContext, object value, object description)
        {

        }

        public void SendToViewEventListener(ChildWindowEventListenerArg arg)
        {
            ViewEventListener?.Invoke(this, arg);

            //OnPropertyChanged("ViewUpdate", arg.View);
        }

        public void SendToEventListener(ViewModelEventListenerArg arg)
        {
            EventListener?.Invoke(this, arg);
        }

        public void OnCellSizeChanged(ICellElement element)
        {
            CellSizeChanged?.Invoke(this, element);
        }

        protected void OnUpdateEvent()
        {
            UpdateEvent?.Invoke(this);
        }

        public void OnViewUpdateEvent()
        {
            ViewUpdateEvent?.Invoke(this);
        }

        protected void OnApplyEvent(object result)
        {
            ApplyEvent?.Invoke(this, result);
        }

        public ObservableCollection<object> Properties
        {
            get => properties;
            set
            {
                properties = value;
                OnPropertyChanged(nameof(Properties));
            }
        }

        public ObservableCollection<object> Models
        {
            get => models;
            set
            {
                models = value;
                OnPropertyChanged(nameof(Models));
            }
        }

        public virtual UIElement UIModel
        {
            get => view as UIElement;
            set
            {
                if (view != value)
                {
                    view = value;
                    OnPropertyChanged(nameof(UIModel));
                }
            }
        }

        public virtual DataTemplate Template
        {
            get { return template; }
            set
            {
                if (value != template)
                {
                    template = value;
                    OnPropertyChanged(nameof(Template), template);
                }
            }
        }

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name), name);
                }
            }
        }

        public virtual void Update()
        {
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(View));
            OnPropertyChanged(nameof(UIModel));
            OnPropertyChanged(nameof(Template));
        }

        public virtual ViewModelBase Clone()
        {
            return null;
        }

        public object Model
        {
            get => model;
        }

        public virtual object GetModel() => model;

        public object View
        {
            get => view;
            set => view = value;
        }

        public object Parent
        {
            get => parent;
            set
            {
                if (parent != value)
                {
                    parent = value;
                    OnPropertyChanged(nameof(Parent), parent);
                }
            }
        }

        public virtual void Dispose()
        {
            CellSizeChanged = null;
            UpdateEvent = null;
            ViewUpdateEvent = null;
            ApplyEvent = null;

            ViewEventListener = null;
            EventListener = null;

            if (models != null)
            {
                foreach (var element in models)
                {
                    if (element is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                models.Clear();
            }

            if (properties != null)
            {
                foreach (var property in properties)
                {
                    if (property is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                properties.Clear();
            }
        }

        public virtual void Close()
        {

        }
    }

    public class ViewModelBase<TModel> : ViewModelBase where TModel : class
    {
        public new TModel Model
        {
            get => model != null ? (TModel)model : null;
            set
            {
                if (value != model)
                {
                    model = value;
                    OnPropertyChanged(nameof(Model), model);
                }
            }
        }

        public ViewModelBase(TModel model)
        {
            Properties = new ObservableCollection<object>();
            Models = new ObservableCollection<object>();

            this.model = model;
        }
    }

    public class ViewModelBase<TModel, TView> : ViewModelBase<TModel> where TModel : class where TView : class, new()
    {
        public ViewModelBase(TModel model) : base(model)
        {
            View = new TView();

            if (view is FrameworkElement frameworkElement)
            {
                frameworkElement.DataContext = this;
            }
            /*View = new TView();

            if (View is FrameworkElement element)
            {
                element.DataContext = this;

                var frameworkElement = new FrameworkElementFactory(typeof(TView));
                frameworkElement.SetValue(FrameworkElement.DataContextProperty, this);

                Template = new DataTemplate { VisualTree = frameworkElement };
            }*/
        }

        public new TView View
        {
            get => view as TView;
            set
            {
                if (value != view)
                {
                    view = value;
                    OnPropertyChanged(nameof(View));
                    OnPropertyChanged(nameof(UIModel));
                }
            }
        }
    }
}

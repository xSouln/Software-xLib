namespace xLibV100
{
    public delegate void ActionAccessUI(xAction request, object arg);
    public delegate void ActionAccessUI<TRequest>(xAction action, TRequest arg);

    public delegate TResult xAction<TResult, TRequest>(TRequest request);
    public delegate void xAction<TRequest>(TRequest request);
    public delegate void xAction(object request);

    public delegate void UIAction<TContext, TRequest>(TContext context, TRequest request);

    //public delegate void Event<TArgument>(object obj, TArgument arg);
    public delegate void xEvent<TArgument>(object obj, TArgument arg);
    public delegate void xEvent<TObject, TArgument>(TObject obj, TArgument arg);
    public delegate void xEvent(object arg);

    public delegate void xEventChangeState<TObject, TState>(TObject obj, TState state);

    public class xPropertyChangedEventHandlerArgs
    {
        public string Name;
        public object Value;
        public object LastState;
    }

    public class PropertyChangedEventHandlerArg
    {
        protected object state;

        public string Name { get; set; }

        public object State
        {
            get => state; set => state = value;
        }
    }

    public class PropertyChangedEventHandlerArg<T> : PropertyChangedEventHandlerArg
    {
        public new T State
        {
            get => state != null ? (T)state : default;
            set => state = value;
        }
    }

    public delegate void xPropertyChangedEventHandler<T1>(T1 sender, xPropertyChangedEventHandlerArgs args);
    public delegate void xPropertyChangedEventHandler<T1, T2>(T1 sender, T2 args);
}

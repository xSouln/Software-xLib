namespace xLibV100.UI
{
    public interface IViewBase
    {
        void Apply(object context);
    }

    public interface IViewBase<T>
    {
        T ViewModel { get; set; }
        void Apply(T context);
    }
}

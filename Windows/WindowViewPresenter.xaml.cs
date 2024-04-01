using System.Windows;

namespace xLibV100.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowViewPresenter.xaml
    /// </summary>
    public partial class WindowViewPresenter : Window
    {
        protected UIElement view;

        public UIElement View
        {
            get => view;
            set
            {
                if (value == null)
                {
                    if (view != null)
                    {
                        Container.Children.Remove(view);
                    }
                    return;
                }

                if (value != view)
                {
                    Container.Children.Remove(view);
                    view = value;
                    Container.Children.Add(view);
                }
            }
        }

        public WindowViewPresenter()
        {
            InitializeComponent();

            MaxWidth = SystemParameters.PrimaryScreenWidth;
        }

        public WindowViewPresenter(UIElement view) : this()
        {
            View = view;

            Container.Children.Remove(view);

            if (view != null)
            {
                Container.Children.Add(view);
            }

            Closed += (source, e) =>
            {
                Container.Children.Remove(view);
            };
        }
    }
}

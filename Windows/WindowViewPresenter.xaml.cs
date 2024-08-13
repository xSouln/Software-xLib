using System.Windows;
using xLibV100.UI;

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

        public static void Open(ViewModelBase viewModel)
        {
            var presenter = new WindowViewPresenter();
            presenter.View = viewModel.View as UIElement;

            presenter.Show();
        }

        public WindowViewPresenter(ViewModelBase viewModel = null)
        {
            InitializeComponent();

            if (viewModel != null)
            {
                View = viewModel.View as UIElement;
            }

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

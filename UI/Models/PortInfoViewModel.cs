using xLibV100.UI;
using xLibV100.UI.CellElements;
using xLibV100.UI.Views;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using xLibV100.Ports;

namespace xLibV100.Common.UI
{
    public class PortInfoViewModel : ViewModelBase<PortBase>, IPortInfoViewModel
    {
        public RelayCommand ToggleVisibilityCommand { get; protected set; }

        public Visibility Visibility { get; set; } = Visibility.Collapsed;

        public bool IsAvailable => true;

        public PortInfoViewModel(PortBase model) : base(model)
        {
            Name = "Info";

            ToggleVisibilityCommand = new RelayCommand(ToggleVisibilityCommandHandler);

            var properties = model.GetType().GetProperties();
            List<object> optionsProperties = new List<object>();

            foreach (var property in properties)
            {
                PortPropertyAttribute portPropertyAttribute = property.GetCustomAttribute(typeof(PortPropertyAttribute)) as PortPropertyAttribute;

                if (portPropertyAttribute != null)
                {
                    var row = new ListViewRow(portPropertyAttribute.Name);
                    row.AddElement(new ContentControlCellElement(model, property.Name, "Info"));

                    if (portPropertyAttribute.Key == "Common info")
                    {
                        Properties.Add(row);
                    }
                    else
                    {
                        optionsProperties.Add(row);
                    }
                }
            }

            foreach (var property in optionsProperties)
            {
                Properties.Add(property);
            }

            var frameworkElement = new FrameworkElementFactory(typeof(PortInfoView));
            frameworkElement.SetValue(FrameworkElement.DataContextProperty, this);

            Template = new DataTemplate { VisualTree = frameworkElement };

            CellSizeChanged += CellSizeChangedHandler;
            ViewUpdateEvent += ViewUpdateEventHandler;
        }

        private void ViewUpdateEventHandler(ViewModelBase viewModel)
        {
            OnUpdateEvent();
        }

        private void CellSizeChangedHandler(ViewModelBase viewModel, ICellElement element)
        {
            OnUpdateEvent();
        }

        private void ToggleVisibilityCommandHandler(object obj)
        {
            Visibility = Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            OnPropertyChanged(nameof(Visibility));

            OnViewUpdateEvent();
        }

        public override void Dispose()
        {
            base.Dispose();

            Model.ConnectionStateChanged -= ConnectionStateChangedHandler;
        }

        private void ConnectionStateChangedHandler(PortBase port, ConnectionStateChangedEventHandlerArg arg)
        {
            OnPropertyChanged(nameof(IsAvailable));
        }
    }
}

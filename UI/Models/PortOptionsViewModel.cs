using xLibV100.UI;
using xLibV100.UI.CellElements;
using xLibV100.UI.Views;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using xLibV100.Ports;

namespace xLibV100.Common.UI
{
    public class PortOptionsViewModel : ViewModelBase<PortBase>, IPortOptionsViewModel
    {
        public RelayCommand ToggleVisibilityCommand { get; protected set; }

        public Visibility Visibility { get; set; } = Visibility.Collapsed;

        public bool IsAvailable => Model.State == States.Idle;

        public PortOptionsViewModel(PortBase model) : base(model)
        {
            Name = "Options";

            ToggleVisibilityCommand = new RelayCommand(ToggleVisibilityCommandHandler);

            var properties = model.GetType().GetProperties();
            List<object> optionsProperties = new List<object>();

            foreach (var property in properties)
            {
                PortPropertyAttribute portPropertyAttribute = property.GetCustomAttribute(typeof(PortPropertyAttribute)) as PortPropertyAttribute;

                if (portPropertyAttribute != null && portPropertyAttribute.Key == "Options")
                {
                    var row = new ListViewRow(portPropertyAttribute.Name);
                    row.AddElement(new TextBoxCellElement(model, property.Name, "Value") { Parent = this });
                    Properties.Add(row);
                }
            }

            foreach (var property in optionsProperties)
            {
                Properties.Add(property);
            }

            Model.ConnectionStateChanged += ConnectionStateChangedHandler;

            var frameworkElement = new FrameworkElementFactory(typeof(PortOptionsView));
            frameworkElement.SetValue(FrameworkElement.DataContextProperty, this);

            Template = new DataTemplate { VisualTree = frameworkElement };

            OnPropertyChanged(nameof(IsAvailable));
        }

        private void ToggleVisibilityCommandHandler(object obj)
        {
            Visibility = Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            OnPropertyChanged(nameof(Visibility));

            OnUpdateEvent();
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

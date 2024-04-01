using xLibV100.UI.CellElements;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace xLibV100.UI.ViewElements
{
    /// <summary>
    /// Логика взаимодействия для ToggleButtonViewElement.xaml
    /// </summary>
    public partial class ToggleButtonViewElement : UserControl
    {
        public ToggleButtonViewElement()
        {
            InitializeComponent();

            DataContextChanged += DataContextChangedHandler;
        }

        private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ListViewRow.Element element)
            {
                CustomButton.DataContext = element.Model;
                BindingOperations.SetBinding(CustomButton, Button.ContentProperty, new Binding(element.PropertyName));
            }
        }

        private void CustomButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is ListViewRow.Element element)
            {
                var property = element.Model.GetType().GetProperty(element.PropertyName);
                property.SetValue(element.Model, (bool)((Button)sender).Content ^ true);
            }
        }
    }
}

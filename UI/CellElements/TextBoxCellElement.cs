using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace xLibV100.UI.CellElements
{
    public class TextBoxCellElement : ListViewRow.Element, ICellElement
    {
        protected FrameworkElementFactory frameworkElement;

        public string Name => "";

        public FrameworkElementFactory Element => frameworkElement;

        public TextBoxCellElement(object model, string propertyName, string column)
        {
            Model = model;
            PropertyName = propertyName;
            Column = column;

            frameworkElement = new FrameworkElementFactory(typeof(TextBox));
            frameworkElement.SetBinding(TextBox.TextProperty, new Binding { Path = new PropertyPath(nameof(Model) + "." + propertyName) });
            frameworkElement.SetValue(TextBox.BackgroundProperty, null);
            frameworkElement.SetValue(TextBox.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            frameworkElement.SetValue(TextBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch);
            frameworkElement.SetValue(FrameworkElement.MarginProperty, new Thickness(0, 0, 0, 0));
            frameworkElement.SetValue(TextBox.FontSizeProperty, 18.0);
            frameworkElement.SetValue(FrameworkElement.WidthProperty, double.NaN);
            frameworkElement.SetValue(FrameworkElement.MinWidthProperty, 100.0);
            frameworkElement.SetValue(TextBox.PaddingProperty, new Thickness(-2));
            frameworkElement.SetValue(TextBox.ForegroundProperty, UIProperty.GetBrush("#FFDEC316"));
            frameworkElement.SetValue(TextBox.BorderBrushProperty, UIProperty.GetBrush("#FF834545"));
            frameworkElement.SetValue(TextBox.CaretBrushProperty, UIProperty.GetBrush("#FFDEC316"));
            frameworkElement.SetValue(TextBox.DataContextProperty, this);

            frameworkElement.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler(TextChangedEventHandler));

            Template = new DataTemplate { VisualTree = frameworkElement };
        }

        public TextBoxCellElement(object model, string propertyName, string column, bool readOnly) : this(model, propertyName, column)
        {
            frameworkElement.SetValue(TextBox.IsReadOnlyProperty, readOnly);
            frameworkElement.SetBinding(TextBox.TextProperty, new Binding
            {
                Path = new PropertyPath(nameof(Model) + "." + propertyName),
                Mode = readOnly ? BindingMode.OneWay : BindingMode.Default
            });
        }

        private void TextChangedEventHandler(object sender, TextChangedEventArgs e)
        {
            if (Parent is ViewModelBase viewModel)
            {
                viewModel.OnCellSizeChanged(this);
            }
        }

        public DataTemplate GetTemplate(object sender, string key)
        {
            return key == Column ? Template : null;
        }

        public void TemplateLoaded(object sender, object component)
        {

        }
    }
}

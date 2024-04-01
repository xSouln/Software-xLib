using System.Windows;
using xLibV100.UI;
using System.Windows.Controls;
using System.Windows.Data;

namespace xLibV100.UI.CellElements
{
    public class ButtonCellElement : ListViewRow.Element, ICellElement
    {
        protected FrameworkElementFactory frameworkElement;
        public RelayCommand ClickCommand { get; set; }

        public string Name => throw new System.NotImplementedException();

        public ButtonCellElement(object model, string propertyName, string column)
        {
            Model = model;
            PropertyName = propertyName;
            Column = column;

            frameworkElement = new FrameworkElementFactory(typeof(Button));
            frameworkElement.SetBinding(Button.ContentProperty, new Binding { Path = new PropertyPath(nameof(Model) + "." + propertyName) });
            frameworkElement.SetBinding(Button.BackgroundProperty, new Binding { Path = new PropertyPath("Background") });
            frameworkElement.SetValue(Button.FontSizeProperty, 18.0);
            frameworkElement.SetValue(Button.MinWidthProperty, 150.0);
            frameworkElement.SetValue(Button.ForegroundProperty, UIProperty.GetBrush("#FFDEC316"));
            frameworkElement.SetValue(Button.PaddingProperty, new Thickness(-2));
            frameworkElement.SetValue(Button.CommandProperty, new Binding { Path = new PropertyPath(nameof(ClickCommand)) });
            frameworkElement.SetValue(Button.CommandParameterProperty, new Binding { Path = new PropertyPath(nameof(Model) + "." + propertyName) });
            frameworkElement.SetValue(Button.DataContextProperty, this);

            Template = new DataTemplate { VisualTree = frameworkElement };
        }

        public virtual DataTemplate GetTemplate(object sender, string key)
        {
            return key == Column ? Template : null;
        }

        public virtual void TemplateLoaded(object sender, object component)
        {

        }
    }
}

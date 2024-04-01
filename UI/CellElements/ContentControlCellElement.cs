using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace xLibV100.UI.CellElements
{
    public class ContentControlCellElement : ListViewRow.Element, ICellElement
    {
        protected FrameworkElementFactory frameworkElement;

        public ContentControlCellElement(object model, string propertyName, string column)
        {
            Model = model;
            PropertyName = propertyName;
            Column = column;

            frameworkElement = new FrameworkElementFactory(typeof(ContentControl));
            frameworkElement.SetBinding(ContentControl.ContentProperty, new Binding { Path = new PropertyPath(nameof(Model) + "." + propertyName) });
            frameworkElement.SetValue(FrameworkElement.MinWidthProperty, 150.0);
            frameworkElement.SetValue(FrameworkElement.DataContextProperty, this);

            Template = new DataTemplate { VisualTree = frameworkElement };
        }

        public string Name => throw new System.NotImplementedException();

        public DataTemplate GetTemplate(object sender, string key)
        {
            return key == Column ? Template : null;
        }

        public void TemplateLoaded(object sender, object component)
        {

        }
    }
}

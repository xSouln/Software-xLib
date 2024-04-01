using System;
using System.Windows;

namespace xLibV100.UI.CellElements
{
    public class UserTemplateCellElement : ListViewRow.Element, ICellElement
    {
        protected FrameworkElementFactory frameworkElement;

        public UserTemplateCellElement(object model, string propertyName, string column, Type type)
        {
            Model = model;
            PropertyName = propertyName;
            Column = column;

            frameworkElement = new FrameworkElementFactory(type);
            frameworkElement.SetValue(FrameworkElement.DataContextProperty, this);
            frameworkElement.SetValue(FrameworkElement.MinWidthProperty, 150.0);

            Template = new DataTemplate { VisualTree = this.frameworkElement };
        }

        public UserTemplateCellElement(object model, string propertyName, string column, DataTemplate template)
        {
            Model = model;
            PropertyName = propertyName;
            Column = column;

            Template = template;
        }

        public string Name => throw new NotImplementedException();

        public DataTemplate GetTemplate(object sender, string key)
        {
            return key == Column ? Template : null;
        }

        public void TemplateLoaded(object sender, object component)
        {

        }
    }
}

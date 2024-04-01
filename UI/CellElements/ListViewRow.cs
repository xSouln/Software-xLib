using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xLibV100.UI;

namespace xLibV100.UI.CellElements
{
    public class ListViewRow : ICellElement
    {
        public class Element : UINotifyPropertyChanged
        {
            protected object model;

            public object Parent { get; set; }

            public object Model
            {
                get => model;
                set => model = value;
            }

            //public int Id { get; set; }

            public string PropertyName { get; set; }

            public string Column { get; set; }

            public DataTemplate Template { get; set; }
        }

        public string Name { get; set; }

        public List<Element> Elements { get; set; } = new List<Element>();

        public ListViewRow(string name)
        {
            Name = name;
        }

        public virtual void AddElement(object model, string propertyName, string column)
        {
            var frameworkElement = new FrameworkElementFactory(typeof(ContentControl));
            frameworkElement.SetBinding(ContentControl.ContentProperty, new Binding { Path = new PropertyPath("Elements[" + Elements.Count + "].Model." + propertyName) });
            frameworkElement.SetValue(FrameworkElement.MinWidthProperty, 150.0);
            frameworkElement.SetValue(FrameworkElement.DataContextProperty, this);

            Elements.Add(new Element
            {
                Model = model,
                PropertyName = propertyName,
                Column = column,
                Template = new DataTemplate { VisualTree = frameworkElement }
            });
        }

        public virtual void AddElement(Element element)
        {
            if (element != null)
            {
                Elements.Add(element);
            }
        }

        public virtual DataTemplate GetTemplate(object sender, string key)
        {
            foreach (var element in Elements)
            {
                if (element.Column == key)
                {
                    return element.Template;
                }
            }
            return null;
        }

        public virtual void TemplateLoaded(object sender, object component)
        {

        }
    }
}

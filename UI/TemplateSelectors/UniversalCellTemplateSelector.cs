using System.Windows.Controls;
using System.Windows;
using xLibV100.UI;

namespace xLibV100.UI.TemplateSelectors
{
    public class UniversalCellTemplateSelector : DataTemplateSelector
    {
        private static DataTemplate emptyCellDataTemplate = new DataTemplate { VisualTree = new FrameworkElementFactory(typeof(FrameworkElement)) };

        /*public class DependencyProperties : DependencyObject
        {
            public static readonly DependencyProperty ColumnKeyProperty = DependencyProperty.Register("ColumnKey", typeof(string), typeof(UniversalValueCellTemplateSelector), new PropertyMetadata(null));
        }*/

        public string ColumnKey { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate template = null;

            if (item is ICellElement cellElement)
            {
                template = cellElement.GetTemplate(this, ColumnKey);
            }
            else if (item is ViewModelBase viewModel)
            {
                template = viewModel.Template;
            }

            return template ?? emptyCellDataTemplate;
        }
    }
}

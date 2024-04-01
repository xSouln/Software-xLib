using System.Windows;
using xLibV100.UI;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Data;

namespace xLibV100.UI.CellElements
{
    public class LabelCellElement : ListViewRow.Element, ICellElement
    {
        public Brush Background { get; set; } = UIProperty.TRANSPARENT;

        public string Name => throw new System.NotImplementedException();

        protected FrameworkElementFactory frameworkElement;

        public LabelCellElement(object model, string propertyName, string column)
        {
            Model = model;
            PropertyName = propertyName;
            Column = column;

            frameworkElement = new FrameworkElementFactory(typeof(Label));
            frameworkElement.SetBinding(Label.ContentProperty, new Binding { Path = new PropertyPath(nameof(Model) + "." + propertyName) });
            frameworkElement.SetBinding(Label.BackgroundProperty, new Binding { Path = new PropertyPath(nameof(Background)) });
            frameworkElement.SetValue(Label.FontSizeProperty, 18.0);
            frameworkElement.SetValue(Label.MinWidthProperty, 150.0);
            frameworkElement.SetValue(Label.HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch);
            frameworkElement.SetValue(Label.ForegroundProperty, UIProperty.GetBrush("#FFDEC316"));
            frameworkElement.SetValue(Label.PaddingProperty, new Thickness(-2));
            frameworkElement.SetValue(Label.DataContextProperty, this);

            frameworkElement.AddHandler(Label.SizeChangedEvent, new SizeChangedEventHandler(SizeChangedHandler));

            Template = new DataTemplate { VisualTree = frameworkElement };
        }

        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
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

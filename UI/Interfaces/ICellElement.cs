using System.Windows;

namespace xLibV100.UI
{
    public interface ICellElement
    {
        string Name { get; }

        DataTemplate GetTemplate(object sender, string key);

        void TemplateLoaded(object sender, object component);
    }
}

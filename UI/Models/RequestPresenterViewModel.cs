using xLibV100.UI;
using xLibV100.UI.CellElements;
using System.Reflection;
using System.Windows;
using xLibV100.Windows;

namespace xLibV100.Common.UI
{
    public class RequestPresenterViewModel : ViewModelBase<object, FrameworkElement>
    {
        protected DialogWindow dialogWindow { get; set; }

        public RequestPresenterViewModel(object model) : base(model)
        {
            Name = "Request presenter";

            var properties = model.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute(typeof(UIPropertyAttribute)) is UIPropertyAttribute propertyAttribute)
                {
                    var row = new ListViewRow(propertyAttribute.Name ?? property.Name);
                    row.AddElement(new TextBoxCellElement(model, property.Name, "Value") { Parent = this });

                    Properties.Add(row);
                }
            }
        }

        public void OpenDialog()
        {
            dialogWindow = new DialogWindow();
            dialogWindow.DataContext = this;


            if (dialogWindow.ShowDialog() == true)
            {
                OnApplyEvent(Model);
            }
        }
    }
}

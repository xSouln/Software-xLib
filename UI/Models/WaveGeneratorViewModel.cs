using xLibV100.Common.WaveGenerators;
using xLibV100.UI.CellElements;
using System.Reflection;
using System.Windows;
using xLibV100.Controls;
using xLibV100.UI;

namespace xLibV100.Common.UI
{
    public class WaveGeneratorViewModel : ViewModelBase<WaveGenerator, FrameworkElement>
    {
        protected double amplitude;
        protected double frequency;
        protected double offset;
        protected uint points;

        public RelayCommand GenerateCommand { get; set; }

        public WaveGeneratorViewModel(WaveGenerator model) : base(model)
        {
            Name = model.Name;

            GenerateCommand = new RelayCommand(GenerateCommandHandler);

            var properties = model.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute(typeof(ModelPropertyAttribute)) is ModelPropertyAttribute propertyAttribute)
                {
                    var row = new ListViewRow(propertyAttribute.Name ?? property.Name);

                    switch (propertyAttribute.Key)
                    {
                        default:
                            row.AddElement(new TextBoxCellElement(model, property.Name, "Value") { Parent = this });
                            break;
                    }

                    Properties.Add(row);
                }
            }
        }

        private void GenerateCommandHandler(object obj)
        {
            Model.Generate();
        }
    }

    public class WaveGeneratorViewModel<TView> : WaveGeneratorViewModel where TView : FrameworkElement, new()
    {
        public WaveGeneratorViewModel(WaveGenerator model) : base(model)
        {
            View = new TView();
            View.DataContext = this;

            var frameworkElement = new FrameworkElementFactory(typeof(TView));
            frameworkElement.SetValue(FrameworkElement.DataContextProperty, this);

            Template = new DataTemplate { VisualTree = frameworkElement };
        }
    }
}

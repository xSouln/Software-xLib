using System;
using System.Windows;
using System.Windows.Controls;

namespace xLib.UI.ViewElements
{
    /// <summary>
    /// Логика взаимодействия для CollectionPresenterViewElement.xaml
    /// </summary>
    public partial class CollectionPresenterViewElement : UserControl
    {
        public CollectionPresenterViewElement()
        {
            InitializeComponent();

            DataContextChanged += DataContextChangedHandler;
        }

        private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}

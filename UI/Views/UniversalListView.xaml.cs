using xLibV100.UI.CellElements;
using xLibV100.UI.TemplateSelectors;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using xLibV100.UI;

namespace xLibV100.UI.Views
{
    /// <summary>
    /// Логика взаимодействия для UniversalListView.xaml
    /// </summary>
    public partial class UniversalListView : UserControl, IListViewHolder
    {
        public class Parameter
        {
            public object Model;
            public object[] Descriptions;
        }

        public ViewModelBase ViewModel
        {
            get => DataContext as ViewModelBase;
            set => DataContext = value;
        }

        private List<string> ListViewColumsName = new List<string>();

        public UniversalListView()
        {
            InitializeComponent();

            //ListViewContextMenu.ItemsSource = ListViewContextMenuCommands;
            //ListViewContextMenu.DataContext = this;

            DataContextChanged += DataContextChangedHandler;

            Unloaded += UnloadedHandler;
            LayoutUpdated += LayoutUpdatedhandler;
            SizeChanged += SizeChangedHandler;
        }

        private void SizeChangedHandler(object sender, SizeChangedEventArgs e)
        {
            /*Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                ViewModel?.SendToViewEventListener(new ViewModelBase.ChildWindowEventListenerArg
                {
                    View = this,
                    PropertyName = nameof(SizeChanged),
                    Content = e
                });
            }));*/
        }

        private void LayoutUpdatedhandler(object sender, EventArgs e)
        {
            
        }        

        private void UnloadedHandler(object sender, RoutedEventArgs e)
        {

        }

        public UniversalListView(ViewModelBase viewModel) : this()
        {
            DataContext = viewModel;
        }

        public void SetParameters(Parameter[] parameters)
        {
            ListViewColumsName.Clear();
            StackPanelControl.Children.Clear();

            foreach (var parameter in parameters)
            {
                if (parameter.Model is RelayCommand[] comands)
                {
                    if (comands != null)
                    {
                        foreach (var command in comands)
                        {
                            StackPanelControl.Children.Add(new Button()
                            {
                                Content = command.Content,
                                Command = command,
                            });
                        }
                    }
                }
            }
        }

        public UniversalListView(ViewModelBase viewModel, RelayCommand[] comands) : this()
        {
            DataContext = viewModel;

            if (comands != null)
            {
                foreach (var command in comands)
                {
                    StackPanelControl.Children.Add(new Button()
                    {
                        Content = command.Content,
                        Command = command,
                    });
                }
            }
        }

        private void DataContextChangedHandler(object sender, DependencyPropertyChangedEventArgs e)
        {
            GridView gridView = ListView.View as GridView;

            ListViewColumsName.Clear();

            gridView.Columns.Clear();
            gridView.Columns.Add(new GridViewColumn
            {
                Header = "Name",
                DisplayMemberBinding = new Binding("Name")
            });

            if (ViewModel == null || ViewModel.Properties == null)
            {
                return;
            }

            ViewModel.Properties.CollectionChanged += ViewModelPropertiesCollectionChangedHandler;
            ViewModel.ViewEventListener += ViewEventListener;

            if (ViewModel.View == null)
            {
                ViewModel.View = this;
            }

            ViewModel.Properties.CollectionChanged += Properties_CollectionChanged;

            foreach (object property in ViewModel.Properties)
            {
                if (property is ListViewRow row)
                {
                    foreach (var element in row.Elements)
                    {
                        if (!ListViewColumsName.Contains(element.Column))
                        {
                            ListViewColumsName.Add(element.Column);

                            gridView.Columns.Add(new GridViewColumn
                            {
                                Header = element.Column,
                                CellTemplateSelector = new UniversalCellTemplateSelector { ColumnKey = element.Column },
                                Width = double.NaN
                            });
                        }
                    }
                }
                else if (property is ViewModelBase viewModel)
                {
                    if (!ListViewColumsName.Contains("Model"))
                    {
                        ListViewColumsName.Add("Model");

                        gridView.Columns.Add(new GridViewColumn
                        {
                            Header = "Model",
                            CellTemplateSelector = new UniversalCellTemplateSelector(),
                            Width = double.NaN
                        });
                    }
                }
            }

            //StackPanelControl.Children.Clear();

            List<(string Key, List<Button> Buttons)> buttonsControl = new List<(string Key, List<Button> Buttons)>();
            var properties = ViewModel.GetType().GetProperties();

            Button previousButton = null;

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute(typeof(ButtonConstructorAttribute)) is ButtonConstructorAttribute attribute)
                {
                    object element = null;

                    foreach (var buttonControl in buttonsControl)
                    {
                        if (buttonControl.Key == attribute.Subgroup)
                        {
                            element = buttonControl;
                            buttonControl.Buttons.Add(new Button()
                            {
                                Content = attribute.Content,
                                Command = property.GetValue(ViewModel) as ICommand,
                            });
                            break;
                        }
                    }

                    if (element == null)
                    {
                        var buttonsControlElement = (attribute.Subgroup, new List<Button>());

                        Button button = new Button()
                        {
                            Content = attribute.Content,
                            Command = property.GetValue(ViewModel) as ICommand
                        };

                        if (previousButton != null)
                        {
                            button.Margin = new Thickness(5, 10, 5, 0);
                        }

                        previousButton = button;

                        buttonsControlElement.Item2.Add(button);
                        buttonsControl.Add(buttonsControlElement);
                    }
                }
                /*else if(property.GetCustomAttribute(typeof(ContextMenuConstructorAttribute)) is ContextMenuConstructorAttribute contextMenuAttribute)
                {
                    ListViewContextMenuCommands.Add(new ContextMenuElement
                    {
                        DisplayName = contextMenuAttribute.Content as string,
                        Command = property.GetValue(ViewModel) as ICommand
                    });
                }*/
            }

            foreach (var control in buttonsControl)
            {
                foreach (var button in control.Buttons)
                {
                    StackPanelControl.Children.Add(button);
                }
            }

            StackPanelControl.UpdateLayout();
        }

        private void ViewEventListener(object sender, ViewModelBase.ChildWindowEventListenerArg e)
        {
            if (e.View != this)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
                {
                    UpdateListViewSize();
                    UpdateLayout();
                }));
            }
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ViewUpdate")
            {
                UpdateLayout();
            }
        }

        private void ViewModelPropertiesCollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                GridView gridView = ListView.View as GridView;
                object property = e.NewItems[0];

                if (property is ListViewRow row)
                {
                    foreach (var element in row.Elements)
                    {
                        if (!ListViewColumsName.Contains(element.Column))
                        {
                            gridView.Columns.Add(new GridViewColumn
                            {
                                Header = element.Column,
                                CellTemplateSelector = new UniversalCellTemplateSelector { ColumnKey = element.Column }
                            });

                            ListViewColumsName.Add(element.Column);
                        }
                    }
                }
                else if (property is ViewModelBase viewModel)
                {
                    if (!ListViewColumsName.Contains("Model"))
                    {
                        gridView.Columns.Add(new GridViewColumn
                        {
                            Header = "Model",
                            CellTemplateSelector = new UniversalCellTemplateSelector()
                        });

                        ListViewColumsName.Add("Model");
                    }
                }

                Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
                {
                    UpdateListViewSize();
                    UpdateLayout();
                }));
            }
        }

        private void Properties_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }

        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            //ViewModel?.OnViewUpdateEvent();

            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                ViewModel?.SendToViewEventListener(new ViewModelBase.ChildWindowEventListenerArg
                {
                    View = this,
                    PropertyName = nameof(Loaded),
                    Content = e
                });

                UpdateListViewSize();
                UpdateLayout();
            }));

            /*UpdateLayout();

            ViewModel?.SendToViewEventListener(new ViewModelBase.ChildWindowEventListenerArg
            {
                View = this,
                PropertyName = nameof(LoadedEvent),
                Content = e
            });*/
        }

        public void UpdateListViewSize()
        {
            if (ListView.View is GridView gridView)
            {
                foreach (var column in gridView.Columns)
                {
                    column.Width = 0;
                    column.Width = double.NaN;
                }
            }

            UpdateLayout();
        }

        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            /*
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                ViewModel?.SendToViewEventListener(new ViewModelBase.ChildWindowEventListenerArg
                {
                    View = this,
                    PropertyName = nameof(SizeChanged),
                    Content = e
                });
            }));*/
        }
    }
}

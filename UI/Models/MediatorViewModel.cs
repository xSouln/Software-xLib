using xLibV100.UI.ViewElements;
using xLibV100.UI;
using xLibV100.UI.CellElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using xLibV100.Controls;
using xLibV100.Windows;
using xLibV100.UI.Views;

namespace xLibV100.Common.UI
{
    public class MediatorViewModel : ViewModelBase<object, FrameworkElement>, ICellElement
    {
        public class Options
        {
            public object Model;
            public object[] Descriptions;
        }

        public string CellElementKey { get; set; }

        public MediatorViewModel(object model) : base(model)
        {

        }

        public class ContextMenuElement
        {
            public string DisplayName { get; set; }
            public ICommand Command { get; set; }
            public object Parameter { get; set; }
        }

        public class CommandElement
        {
            public string Name { get; set; }
            public ICommand Command { get; set; }
            public object[] Parameters { get; set; }
        }

        public ObservableCollection<ContextMenuElement> ListViewContextMenuCommands { get; set; } = new ObservableCollection<ContextMenuElement>();
        public ObservableCollection<CommandElement> Commands { get; set; } = new ObservableCollection<CommandElement>();

        protected void AddProperty(object model, ModelPropertyAttribute attribute, PropertyInfo info, string propertyName, string columnName)
        {
            ListViewRow row = null;

            foreach (ListViewRow item in properties.Cast<ListViewRow>())
            {
                if (item.Name == info.Name)
                {
                    foreach (var element in item.Elements)
                    {
                        if (element.PropertyName == propertyName)
                        {
                            return;
                        }
                    }

                    row = item;
                    break;
                }
            }

            if (row == null)
            {
                row = new ListViewRow(propertyName);
            }

            if (columnName == null)
            {
                columnName = "Value";
            }

            if (attribute != null && attribute.ReadOnly)
            {
                row.AddElement(new ContentControlCellElement(model, propertyName, columnName));

            }
            else if (info.PropertyType == typeof(bool))
            {
                row.AddElement(new UserTemplateCellElement(model, propertyName, columnName, typeof(ToggleButtonViewElement)));
            }
            else if (info.PropertyType.IsClass && info.PropertyType != typeof(string))
            {
                return;
            }
            else if (typeof(ICollection).IsAssignableFrom(info.PropertyType))
            {
                ICollection collection = (ICollection)info.GetValue(model);

                foreach (var element in collection)
                {
                    var viewModel = new MediatorViewModel<UniversalListView>(this, new Options[]
                    {
                        new Options
                        {
                            Model = element,
                            Descriptions = new object[] { new ModelPropertyAttribute() }
                        }
                    });

                    viewModel.Name = columnName;
                    viewModel.ViewEventListener += SubViewEventListener;

                    Properties.Add(viewModel);
                }

                return;
            }
            else
            {
                row.AddElement(new TextBoxCellElement(model, propertyName, columnName) { Parent = this });
            }

            Properties.Add(row);
        }

        private void SubViewEventListener(object sender, ChildWindowEventListenerArg e)
        {
            SendToViewEventListener(e);
        }

        protected void RelayCommandHandler(RelayCommand relayCommand, object arg)
        {
            object model = relayCommand.Parameters[0];
            MethodInfo method = relayCommand.Parameters[1] as MethodInfo;

            if (model != null && method != null)
            {
                ParameterInfo[] parameters = method.GetParameters();

                List<object> methodParameters = new List<object>();

                foreach (var parameter in parameters)
                {
                    methodParameters.Add(Activator.CreateInstance(parameter.ParameterType));
                }

                if (methodParameters.Count > 0)
                {
                    var viewModel = new MediatorViewModel<UniversalListView>(this);
                    WindowViewPresenter viewPresenter = new WindowViewPresenter(viewModel.View);

                    viewModel.Commands.Add(new CommandElement
                    {
                        Name = "Apply",
                        Command = new RelayCommand((command, parameter) =>
                        {
                            WindowViewPresenter windowViewPresenter = command.Content as WindowViewPresenter;
                            windowViewPresenter.DialogResult = true;
                        })
                        {
                            Content = viewPresenter
                        },
                    });

                    viewModel.ApplyOptions(new Options[]
                    {
                        new Options{ Descriptions = new object[] { new ModelPropertyAttribute() }, Model = methodParameters[0] }
                    });

                    viewPresenter.ShowDialog();


                }

                method.Invoke(model, methodParameters.ToArray());
            }
        }


        protected class ModelFunctionAttributeParser
        {
            protected int Depth;

            public ModelFunctionAttribute Description;
            public int MaxDepth;

            public ModelFunctionAttributeParser(ModelFunctionAttribute description, int maxDepth)
            {
                Description = description;
                MaxDepth = maxDepth;
            }

            public int Parse(MediatorViewModel viewModel, object model)
            {
                if (Depth >= MaxDepth)
                {
                    return 0;
                }

                Depth++;

                var functions = model.GetType().GetMethods();

                foreach (var function in functions)
                {
                    if (function.GetCustomAttribute(typeof(ModelFunctionAttribute)) is ModelFunctionAttribute attribute)
                    {
                        viewModel.Commands.Add(new CommandElement
                        {
                            Name = attribute.Name == null ? function.Name : attribute.Name,
                            Command = new RelayCommand(viewModel.RelayCommandHandler) { Parameters = new object[] { model, function } },
                            Parameters = new object[] { model, function }
                        });
                    }
                }

                return 0;
            }
        }

        protected class ModelPropertyAttributeParser
        {
            protected int Depth;

            public ModelPropertyAttribute Description;
            public int MaxDepth;

            public ModelPropertyAttributeParser(ModelPropertyAttribute description, int maxDepth)
            {
                Description = description;
                MaxDepth = maxDepth;
            }

            public int Parse(MediatorViewModel viewModel, object model)
            {
                if (Depth >= MaxDepth)
                {
                    return 0;
                }

                Depth++;

                var propterties = model?.GetType().GetProperties();

                foreach (var property in propterties)
                {
                    if (property.GetCustomAttribute(typeof(ModelPropertyAttribute)) is ModelPropertyAttribute propertyAttribute)
                    {
                        if (typeof(ICollection).IsAssignableFrom(property.PropertyType))
                        {
                            continue;
                        }
                        else if (property.PropertyType == typeof(string))
                        {

                        }
                        else if (property.PropertyType.IsClass)
                        {
                            Parse(viewModel, property.GetValue(model));
                            continue;
                        }

                        if (Description.Group == propertyAttribute.Group || Description.Group == null)
                        {
                            viewModel.AddProperty(model, propertyAttribute, property, property.Name, propertyAttribute.Group);
                        }
                    }
                }

                return 0;
            }
        }

        public MediatorViewModel(object model, Options[] options) : this(model)
        {
            ApplyOptions(options);
        }

        public void ApplyOptions(Options[] options)
        {
            foreach (var option in options)
            {
                var propterties = option.Model?.GetType().GetProperties();

                if (option.Descriptions != null)
                {
                    foreach (var description in option.Descriptions)
                    {
                        switch (description)
                        {
                            case ModelPropertyAttribute attribute:
                                {
                                    ModelPropertyAttributeParser parser = new ModelPropertyAttributeParser(attribute, 2);
                                    parser.Parse(this, option.Model);
                                    break;
                                }

                            case ModelFunctionAttribute attribute:
                                {
                                    ModelFunctionAttributeParser parser = new ModelFunctionAttributeParser(attribute, 2);
                                    parser.Parse(this, option.Model);
                                    break;
                                }

                            case ContextMenuConstructorAttribute attribute:
                                foreach (var property in propterties)
                                {
                                    if (property.GetCustomAttribute(typeof(ContextMenuConstructorAttribute)) is ContextMenuConstructorAttribute propertyAttribute)
                                    {
                                        ListViewContextMenuCommands.Add(new ContextMenuElement
                                        {
                                            Command = property.GetValue(option.Model) as ICommand,
                                            DisplayName = propertyAttribute.Content as string,
                                            Parameter = model
                                        });
                                    }
                                }
                                break;

                            case string groupe:
                                foreach (var property in propterties)
                                {
                                    AddProperty(option.Model, null, property, property.Name, groupe);
                                }
                                break;

                            case ContextMenuElement contextMenuElement:
                                ListViewContextMenuCommands.Add(contextMenuElement);
                                break;

                            case ContextMenuElement[] contextMenuElements:
                                foreach (var contextMenuElement in contextMenuElements)
                                {
                                    ListViewContextMenuCommands.Add(contextMenuElement);
                                }
                                break;
                        }
                    }
                }
            }

            foreach (var property in model.GetType().GetProperties())
            {
                if (property.GetCustomAttribute(typeof(ContextMenuConstructorAttribute)) is ContextMenuConstructorAttribute contextMenuAttribute)
                {
                    ListViewContextMenuCommands.Add(new ContextMenuElement
                    {
                        DisplayName = contextMenuAttribute.Content as string,
                        Command = property.GetValue(model) as ICommand,
                        Parameter = model
                    });
                }
            }
        }

        public void ApplyViewTemplate<TView>() where TView : FrameworkElement, new()
        {
            View = new TView
            {
                DataContext = this
            };

            var frameworkElement = new FrameworkElementFactory(typeof(TView));
            frameworkElement.SetValue(FrameworkElement.DataContextProperty, this);

            Template = new DataTemplate { VisualTree = frameworkElement };
        }

        public DataTemplate GetTemplate(object sender, string key)
        {
            return CellElementKey == key ? Template : null;
        }

        public void TemplateLoaded(object sender, object component)
        {
            throw new System.NotImplementedException();
        }
    }

    public class MediatorViewModel<TView> : MediatorViewModel where TView : FrameworkElement, new()
    {
        public MediatorViewModel(object model) : base(model)
        {
            View = new TView
            {
                DataContext = this
            };

            var frameworkElement = new FrameworkElementFactory(typeof(TView));
            frameworkElement.SetValue(FrameworkElement.DataContextProperty, this);

            Template = new DataTemplate { VisualTree = frameworkElement };
        }

        public MediatorViewModel(object model, Options[] options) : base(model, options)
        {
            View = new TView
            {
                DataContext = this
            };

            var frameworkElement = new FrameworkElementFactory(typeof(TView));
            frameworkElement.SetValue(FrameworkElement.DataContextProperty, this);

            Template = new DataTemplate { VisualTree = frameworkElement };
        }
    }
}

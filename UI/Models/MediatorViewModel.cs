﻿using xLibV100.UI.ViewElements;
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
using System.ComponentModel;
using xLib.UI.ViewElements;
using System.Runtime.InteropServices;

namespace xLibV100.Common.UI
{
    public class MediatorViewModel : ViewModelBase<object, FrameworkElement>, ICellElement
    {
        [Flags]
        protected enum ParseOptionsFlags
        {
            None = 0,

            ReadOnly = 1 << 0,
            WriteOnly = 1 << 1,
        }

        public class Options
        {
            public object Model;
            public object[] Descriptions;
        }

        protected class ParseOptions
        {
            public ParseOptionsFlags Flags;
            public ModelPropertyAttribute Attribute;
            public PropertyInfo Info;
            public string PropertyName;
            public string ColumnName;
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
        public ObservableCollection<RelayCommand> Commands { get; set; } = new ObservableCollection<RelayCommand>();

        protected void AddProperty(object model, ParseOptions options)
        {
            ListViewRow row = null;

            foreach (ListViewRow item in properties.Cast<ListViewRow>())
            {
                if (item.Name == options.Info.Name)
                {
                    foreach (var element in item.Elements)
                    {
                        if (element.PropertyName == options.PropertyName)
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
                row = new ListViewRow(options.PropertyName);
            }

            if (options.ColumnName == null)
            {
                options.ColumnName = "Value";
            }

            if (options.Attribute != null && options.Attribute.ReadOnly)
            {
                row.AddElement(new ContentControlCellElement(model, options.PropertyName, options.ColumnName));

            }
            else if (options.Info.PropertyType == typeof(bool))
            {
                row.AddElement(new UserTemplateCellElement(model, options.PropertyName, options.ColumnName, typeof(ToggleButtonViewElement)));
            }
            else if (options.Info.PropertyType.IsEnum && options.Attribute.IsBitsField)
            {
                row.AddElement(new BitFieldCellElement(model, options.Info, options.PropertyName, options.ColumnName));
            }
            else if (options.Info.PropertyType.IsClass && options.Info.PropertyType != typeof(string))
            {
                return;
            }
            else if (typeof(ICollection).IsAssignableFrom(options.Info.PropertyType))
            {
                ICollection collection = (ICollection)options.Info.GetValue(model);

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

                    viewModel.Name = options.ColumnName;
                    viewModel.ViewEventListener += SubViewEventListener;

                    Properties.Add(viewModel);
                }

                return;
            }
            else
            {
                row.AddElement(new TextBoxCellElement(model, options.PropertyName, options.ColumnName) { Parent = this });
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

                    viewModel.Commands.Add(new RelayCommand((command, parameter) =>
                    {
                        WindowViewPresenter windowViewPresenter = command.Content as WindowViewPresenter;
                        windowViewPresenter.DialogResult = true;
                    })
                    {
                        Name = "Apply",
                        Content = viewPresenter
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
                        viewModel.Commands.Add(new RelayCommand(viewModel.RelayCommandHandler)
                        {
                            Name = attribute.Name ?? function.Name,
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
                            ParseOptions parseOptions = new ParseOptions();
                            parseOptions.Attribute = propertyAttribute;
                            parseOptions.Info = property;
                            parseOptions.PropertyName = property.Name;
                            parseOptions.ColumnName = propertyAttribute.Group;

                            viewModel.AddProperty(model, parseOptions);
                        }
                    }
                }

                return 0;
            }
        }

        public class BitFieldCellElement : UserTemplateCellElement
        {
            public ObservableCollection<BitFieldProperty> Field { get; set; } = new ObservableCollection<BitFieldProperty>();

            public BitFieldCellElement(object model, PropertyInfo property, string propertyName, string column) : base(model, propertyName, column, typeof(BitsFieldViewElement))
            {
                var enums = property.PropertyType.GetFields(BindingFlags.Public | BindingFlags.Static);

                foreach (var enumElement in enums)
                {
                    Field.Add(new BitFieldProperty(model, property, enumElement.GetValue(null)));
                }
            }
        }

        public class BitFieldProperty : UINotifyPropertyChanged, IDisposable
        {
            private object model;
            private ulong flagMask;
            private object flagName;
            private PropertyInfo propertyInfo;

            public RelayCommand ClickCommand { get; set; }

            public BitFieldProperty(object model, PropertyInfo propertyInfo, object flagMask)
            {
                this.model = model;
                this.flagMask = Convert.ToUInt64(flagMask);
                this.propertyInfo = propertyInfo;
                flagName = flagMask;

                ClickCommand = new RelayCommand(ClickCommandHandler);

                if (model is UINotifyPropertyChanged notification)
                {
                    notification.PropertyChanged += OnPropertyChangedHandler;
                }

                OnPropertyChanged(nameof(FlagIsEnabled));
                OnPropertyChanged(nameof(StateName));
            }

            private void ClickCommandHandler(object obj)
            {
                ulong value = Convert.ToUInt64(propertyInfo.GetValue(model));

                if (flagMask == 0)
                {
                    propertyInfo.SetValue(model, 0);
                }
                else if ((value & flagMask) > 0)
                {
                    value &= ~flagMask;
                    propertyInfo.SetValue(model, Enum.ToObject(propertyInfo.PropertyType, value));
                }
                else
                {
                    value |= flagMask;
                    propertyInfo.SetValue(model, Enum.ToObject(propertyInfo.PropertyType, value));
                }
            }

            private void OnPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == propertyInfo.Name)
                {
                    OnPropertyChanged(nameof(FlagIsEnabled));
                    OnPropertyChanged(nameof(StateName));
                }
            }

            public string StateName
            {
                get
                {
                    string state = FlagIsEnabled ? " Is Enabled" : " Is Disabled";

                    return "" + flagName + state;
                }
            }


            public bool FlagIsEnabled
            {
                get
                {
                    var value = Convert.ToUInt64(propertyInfo.GetValue(model));
                    
                    return flagMask == 0 ? value == 0 : (value & flagMask) == flagMask;
                }
            }

            public void Dispose()
            {
                if (model is UINotifyPropertyChanged notification)
                {
                    notification.PropertyChanged -= OnPropertyChangedHandler;
                }
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
                                    ParseOptions parseOptions = new ParseOptions();
                                    parseOptions.Info = property;
                                    parseOptions.PropertyName = property.Name;
                                    parseOptions.ColumnName = groupe;

                                   AddProperty(option.Model, parseOptions);
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
        public new TView View
        {
            get => view as TView;
            set
            {
                if (value != view)
                {
                    view = value;
                    OnPropertyChanged(nameof(View));
                    OnPropertyChanged(nameof(UIModel));
                }
            }
        }

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

    public class MediatorViewModel<TModel, TView> : MediatorViewModel<TView>
        where TView : FrameworkElement, new()
        where TModel : class
    {
        public new TModel Model
        {
            get => model as TModel;
            set
            {
                if (value != model)
                {
                    model = value;
                    OnPropertyChanged(nameof(Model), model);
                }
            }
        }

        public MediatorViewModel(object model) : base(model)
        {

        }

        public MediatorViewModel(object model, Options[] options) : base(model, options)
        {

        }
    }
}

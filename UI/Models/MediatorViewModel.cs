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
using System.ComponentModel;
using xLib.UI.ViewElements;
using static xLibV100.UI.CellElements.ListViewRow;
using System.Runtime.InteropServices;

namespace xLibV100.Common.UI
{
    public partial class MediatorViewModel : ViewModelBase<object, FrameworkElement>, ICellElement
    {
        [Flags]
        public enum ParseOptionsFlags
        {
            None = 0,

            SetReadTemplate = 1 << 0,
            WriteOnly = 1 << 1,
            BitsFieldIgnore = 1 << 2,
            ClassesIgnore = 1 << 3,
        }

        public class ParseParameters
        {
            public ParseOptionsFlags Flags;

            public string[] WritePropertiesName;
            public string[] ReadPropertiesName;
            public string[] ExcludedPropertiesNames;
        }

        public class Options
        {
            public object Model;
            public object[] Descriptions;
            public ParseParameters Parameters;
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

        public ObservableCollection<ContextMenuElement> ListViewContextMenuCommands { get; set; } = new ObservableCollection<ContextMenuElement>();

        public MediatorViewModel(object model) : base(model)
        {

        }

        public MediatorViewModel(object model, Options[] options) : this(model)
        {
            ApplyOptions(options);
        }

        public override void Dispose()
        {
            base.Dispose();
        }

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
                if (typeof(IEnumerable).IsAssignableFrom(options.Info.PropertyType) && options.Info.PropertyType != typeof(string))
                {
                    /*if (options.Info.PropertyType.GetElementType().IsPrimitive)
                    {
                        row.AddElement(new CollectionCellElement(model, options.Info, options.ColumnName));
                        goto end;
                    }*/

                    row.AddElement(new CollectionCellElement(model, options.Info, options.ColumnName));
                }
                else
                {
                    row.AddElement(new ContentControlCellElement(model, options.PropertyName, options.ColumnName));
                }
            }
            else if (options.Info.PropertyType == typeof(bool))
            {
                if ((options.Flags & ParseOptionsFlags.SetReadTemplate) == ParseOptionsFlags.SetReadTemplate)
                {
                    row.AddElement(new ContentControlCellElement(model, options.PropertyName, options.ColumnName));
                }
                else
                {
                    row.AddElement(new UserTemplateCellElement(model, options.PropertyName, options.ColumnName, typeof(ToggleButtonViewElement)));
                }
            }
            else if (options.Info.PropertyType.IsEnum && options.Attribute.IsBitsField)
            {
                if ((options.Flags & ParseOptionsFlags.BitsFieldIgnore) > 0)
                {
                    row.AddElement(new ContentControlCellElement(model, options.PropertyName, options.ColumnName));
                }
                else
                {
                    row.AddElement(new BitFieldCellElement(model, options.Info, options.PropertyName, options.ColumnName, (options.Flags & ParseOptionsFlags.SetReadTemplate) == 0));
                }
            }
            else if (options.Info.PropertyType.IsEnum)
            {
                if ((options.Flags & ParseOptionsFlags.SetReadTemplate) > 0)
                {
                    row.AddElement(new ContentControlCellElement(model, options.PropertyName, options.ColumnName));
                }
                else
                {
                    row.AddElement(new EnumCellElement(model, options.Info, options.ColumnName));
                }
            }
            else if (options.Info.PropertyType.IsClass && options.Info.PropertyType != typeof(string))
            {
                return;
            }
            else if (typeof(IEnumerable).IsAssignableFrom(options.Info.PropertyType))
            {
                row.AddElement(new CollectionCellElement(model, options.Info, options.ColumnName));

                /*if (options.Info.PropertyType.GetElementType().IsPrimitive)
                {
                    row.AddElement(new CollectionCellElement(model, options.Info, options.ColumnName));
                    goto end;
                }*/

                /*IEnumerable collection = (IEnumerable)options.Info.GetValue(model);

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
                }*/

                return;
            }
            else
            {
                row.AddElement(new TextBoxCellElement(model,
                    options.PropertyName,
                    options.ColumnName,
                    (options.Flags & ParseOptionsFlags.SetReadTemplate) != 0) { Parent = this });
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
                    DialogWindowPresenter viewPresenter = null;
                    var viewModel = new MediatorViewModel<UniversalListView>(this);

                    viewModel.ApplyOptions(new Options
                    {
                        Descriptions = new object[] { new ModelPropertyAttribute() },
                        Model = methodParameters[0],
                        Parameters = new ParseParameters()
                    });

                    viewPresenter = new DialogWindowPresenter(viewModel);
                    viewPresenter.ShowDialog();

                    if (viewPresenter != null && viewPresenter.DialogResult == true)
                    {
                        method.Invoke(model, methodParameters.ToArray());
                    }
                }
                else
                {
                    method.Invoke(model, methodParameters.ToArray());
                }
            }
        }

        public bool OpenDialog(object requestParameters)
        {
            DialogWindowPresenter viewPresenter = null;
            var viewModel = new MediatorViewModel<UniversalListView>(this);

            viewModel.ApplyOptions(new Options
            {
                Model = requestParameters,
                Descriptions = new object[] { new ModelPropertyAttribute() },
                Parameters = new ParseParameters() { Flags = ParseOptionsFlags.WriteOnly }
            });

            viewPresenter = new DialogWindowPresenter(viewModel);
            viewPresenter.ShowDialog();

            return (bool)viewPresenter.DialogResult;
        }

        public static bool OpenDialog(object requestParameters, object parent = null, Window window = null)
        {
            DialogWindowPresenter viewPresenter = null;
            var viewModel = new MediatorViewModel<UniversalListView>(parent);

            viewModel.ApplyOptions(new Options
            {
                Model = requestParameters,
                Descriptions = new object[] { new ModelPropertyAttribute() },
                Parameters = new ParseParameters() { Flags = ParseOptionsFlags.WriteOnly }
            });

            viewPresenter = new DialogWindowPresenter(viewModel);
            viewPresenter.ShowDialog();

            return (bool)viewPresenter.DialogResult;
        }

        public class ModelFunctionAttributeParser : MediatorViewModelParser
        {
            protected int Depth;

            public ModelFunctionAttribute Description;
            public int MaxDepth;

            public ModelFunctionAttributeParser(ModelFunctionAttribute description, int maxDepth)
            {
                Description = description;
                MaxDepth = maxDepth;
            }

            public override int Parse(MediatorViewModel viewModel, object model, ParseParameters parameters)
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

        public class ModelPropertyAttributeParser : MediatorViewModelParser
        {
            protected int Depth;

            public ModelPropertyAttribute Description;
            public int MaxDepth;

            public ModelPropertyAttributeParser(ModelPropertyAttribute description, int maxDepth)
            {
                Description = description;
                MaxDepth = maxDepth;
            }

            public override int Parse(MediatorViewModel viewModel, object model, ParseParameters parameters)
            {
                if (Depth >= MaxDepth)
                {
                    return 0;
                }

                Depth++;

                var propterties = model?.GetType().GetProperties();

                if (propterties == null)
                {
                    return 0;
                }

                foreach (var property in propterties)
                {
                    if (property.GetCustomAttribute(typeof(ModelPropertyAttribute)) is ModelPropertyAttribute propertyAttribute)
                    {
                        if (typeof(ICollection).IsAssignableFrom(property.PropertyType)
                            || typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                        {
                            //Type elementType = property.PropertyType.GetElementType();

                            //if (elementType != null && elementType.IsPrimitive)
                            {
                                ParseOptions parseOptions = new ParseOptions();
                                parseOptions.Attribute = propertyAttribute;
                                parseOptions.Info = property;
                                parseOptions.Flags = parameters != null ? parameters.Flags : 0;
                                parseOptions.PropertyName = property.Name;
                                parseOptions.ColumnName = propertyAttribute.Group;

                                viewModel.AddProperty(model, parseOptions);
                            }

                            continue;
                        }
                        else if (property.PropertyType == typeof(string))
                        {

                        }
                        else if (property.PropertyType.IsClass)
                        {
                            Parse(viewModel, property.GetValue(model), parameters);
                            continue;
                        }

                        if (Description.Group == propertyAttribute.Group || Description.Group == null)
                        {
                            ParseOptions parseOptions = new ParseOptions();
                            parseOptions.Attribute = propertyAttribute;
                            parseOptions.Info = property;
                            parseOptions.Flags = parameters != null ? parameters.Flags : 0;
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

            public BitFieldCellElement(object model, PropertyInfo property, string propertyName, string column, bool isWritable)
                : base(model, propertyName, column, isWritable ? typeof(BitsFieldControlViewElement) : typeof(BitsFieldIndicatorViewElement))
            {
                var enums = property.PropertyType.GetFields(BindingFlags.Public | BindingFlags.Static);

                foreach (var enumElement in enums)
                {
                    Field.Add(new BitFieldProperty(model, property, enumElement.GetValue(null)));
                }
            }
        }

        public class CollectionCellElement : UserTemplateCellElement
        {
            public object Values { get; set; }

            public CollectionCellElement(object model, PropertyInfo property, string column)
                : base(model, property.Name, column, typeof(CollectionPresenterViewElement))
            {
                Values = property.GetValue(model);
            }
        }

        public class EnumCellElement : UserTemplateCellElement, IDisposable
        {
            protected PropertyInfo propertyInfo;

            public List<object> EnumValues { get; set; }

            public object SelectedValue
            {
                get => propertyInfo.GetValue(Model);
                set => propertyInfo.SetValue(Model, value);
            }

            public EnumCellElement(object model, PropertyInfo property, string column)
                : base(model, property.Name, column, typeof(EnumCellViewElement))
            {
                propertyInfo = property;
                EnumValues = Enum.GetValues(property.PropertyType).Cast<object>().ToList();

                if (model is UINotifyPropertyChanged notifier)
                {
                    notifier.PropertyChanged += PropertyChangedHandler;
                }

                //SelectedValue = default(property);
            }

            private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == propertyInfo.Name)
                {
                    OnPropertyChanged(nameof(SelectedValue));
                }
            }

            public override void Dispose()
            {
                base.Dispose();

                if (model is UINotifyPropertyChanged notifier)
                {
                    notifier.PropertyChanged -= PropertyChangedHandler;
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
                    propertyInfo.SetValue(model, Enum.ToObject(propertyInfo.PropertyType, 0));
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

            public override void Dispose()
            {
                base.Dispose();

                if (model is UINotifyPropertyChanged notification)
                {
                    notification.PropertyChanged -= OnPropertyChangedHandler;
                }
            }
        }

        public class ContextMenuElement
        {
            public string DisplayName { get; set; }
            public ICommand Command { get; set; }
            public object Parameter { get; set; }
        }

        public void ApplyOptions(object model, MediatorViewModelParser parser, ParseParameters parameters)
        {
            parser.Parse(this, model, parameters);
        }


        public void ApplyOptions(Options option)
        {
            ApplyOptions(new Options[] { option });
        }

        public void RemoveOptions(object model)
        {
            foreach (var row in Properties)
            {
                if (row is Element element)
                {

                }
            }
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
                                    parser.Parse(this, option.Model, option.Parameters);
                                    break;
                                }

                            case ModelFunctionAttribute attribute:
                                {
                                    ModelFunctionAttributeParser parser = new ModelFunctionAttributeParser(attribute, 2);
                                    parser.Parse(this, option.Model, option.Parameters);
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

            if (model == null)
            {
                return;
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

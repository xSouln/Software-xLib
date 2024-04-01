using System.Collections.Generic;
using System.Reflection;
using xLibV100.Controls;

namespace xLibV100.UI
{
    public class RecognizedModelProperty
    {
        protected object property;
        protected PropertyInfo info;
        protected ModelPropertyAttribute attribute;

        public object Property
        {
            get => property;
            set => property = value;
        }

        public PropertyInfo Info
        {
            get => info;
            set => info = value;
        }

        public ModelPropertyAttribute Attribute
        {
            get => attribute;
            set => attribute = value;
        }
    }

    public class RecognizedModelPropertyElement<TAttribute> : RecognizedModelProperty where TAttribute : ModelPropertyAttribute
    {
        public new TAttribute Attribute
        {
            get => attribute as TAttribute;
            set => attribute = value;
        }
    }

    public class RecognizedUIProperty
    {
        protected object property;
        protected PropertyInfo info;
        protected UIPropertyAttribute attribute;

        public object Property
        {
            get => property;
            set => property = value;
        }

        public PropertyInfo Info
        {
            get => info;
            set => info = value;
        }

        public UIPropertyAttribute Attribute
        {
            get => attribute;
            set => attribute = value;
        }
    }

    public class RecognizedUIPropertyElement<TAttribute> : RecognizedUIProperty where TAttribute : UIPropertyAttribute
    {
        public new TAttribute Attribute
        {
            get => attribute as TAttribute;
            set => attribute = value;
        }
    }

    public class RecognizedModel
    {
        protected object model;
        protected PropertyInfo info;
        protected SubmodelAttribute attribute;
        protected object properties;
        protected object models;

        public List<RecognizedModelProperty> Properties
        {
            get => properties as List<RecognizedModelProperty>;
            set => properties = value;
        }

        public List<RecognizedModel> Models
        {
            get => models as List<RecognizedModel>;
            set => models = value;
        }

        public object Model
        {
            get => model;
            set => model = value;
        }

        public PropertyInfo Info
        {
            get => info;
            set => info = value;
        }

        public SubmodelAttribute Attribute
        {
            get => attribute;
            set => attribute = value;
        }

        public RecognizedModel()
        {
            Properties = new List<RecognizedModelProperty>();
            Models = new List<RecognizedModel>();
        }
    }

    public class RecognizedModel<TModelAttribute, TPropertyAttribute> : RecognizedModel
        where TModelAttribute : SubmodelAttribute
        where TPropertyAttribute : ModelPropertyAttribute
    {
        public new List<RecognizedModel<TModelAttribute, TPropertyAttribute>> Models
        {
            get => models as List<RecognizedModel<TModelAttribute, TPropertyAttribute>>;
            set => models = value;
        }

        public new List<RecognizedModelPropertyElement<ModelPropertyAttribute>> Properties
        {
            get => properties as List<RecognizedModelPropertyElement<ModelPropertyAttribute>>;
            set => properties = value;
        }

        public RecognizedModel()
        {
            Properties = new List<RecognizedModelPropertyElement<ModelPropertyAttribute>>();
            Models = new List<RecognizedModel<TModelAttribute, TPropertyAttribute>>();
        }
    }

    public class RecognizedViewModel
    {
        protected object model;
        protected PropertyInfo info;
        protected SubViewModelAttribute attribute;
        protected object properties;
        protected object models;

        public List<RecognizedUIProperty> Properties
        {
            get => properties as List<RecognizedUIProperty>;
            set => properties = value;
        }

        public List<RecognizedViewModel> Models
        {
            get => models as List<RecognizedViewModel>;
            set => models = value;
        }

        public object Model
        {
            get => model;
            set => model = value;
        }

        public PropertyInfo Info
        {
            get => info;
            set => info = value;
        }

        public SubViewModelAttribute Attribute
        {
            get => attribute;
            set => attribute = value;
        }

        public RecognizedViewModel()
        {
            Properties = new List<RecognizedUIProperty>();
            Models = new List<RecognizedViewModel>();
        }
    }

    public class RecognizedViewModel<TModelAttribute, TPropertyAttribute> : RecognizedViewModel
        where TModelAttribute : SubViewModelAttribute
        where TPropertyAttribute : UIPropertyAttribute
    {
        public new List<RecognizedViewModel<TModelAttribute, TPropertyAttribute>> Models
        {
            get => models as List<RecognizedViewModel<TModelAttribute, TPropertyAttribute>>;
            set => models = value;
        }

        public new List<RecognizedUIPropertyElement<UIPropertyAttribute>> Properties
        {
            get => properties as List<RecognizedUIPropertyElement<UIPropertyAttribute>>;
            set => properties = value;
        }

        public RecognizedViewModel()
        {
            Properties = new List<RecognizedUIPropertyElement<UIPropertyAttribute>>();
            Models = new List<RecognizedViewModel<TModelAttribute, TPropertyAttribute>>();
        }
    }

    public static class Recognizer
    {
        public delegate void Constructor(RecognizedModel model, RecognizedModelProperty modelProperty,
            RecognizedViewModel viewModel, RecognizedUIProperty viewModelProperty);

        private static RecognizedModel<TModelAttribute, TPropertyAttribute> iRecognize<TModelAttribute, TPropertyAttribute>(object context, ref int deph)
            where TModelAttribute : SubmodelAttribute
            where TPropertyAttribute : ModelPropertyAttribute
        {
            if (deph == 0)
            {
                return null;
            }

            deph--;

            RecognizedModel<TModelAttribute, TPropertyAttribute> recognizeElement = new RecognizedModel<TModelAttribute, TPropertyAttribute>();
            recognizeElement.Model = context;

            var properties = context.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyAttribute = property.GetCustomAttribute(typeof(TPropertyAttribute)) as TPropertyAttribute;
                if (propertyAttribute != null)
                {
                    recognizeElement.Properties.Add(new RecognizedModelPropertyElement<ModelPropertyAttribute>
                    {
                        Attribute = propertyAttribute,
                        Info = property,
                        Property = property.GetValue(context)
                    });
                }

                var modelAttribute = property.GetCustomAttribute(typeof(TModelAttribute)) as TModelAttribute;
                if (modelAttribute != null)
                {
                    int totalDeph = deph;
                    RecognizedModel<TModelAttribute, TPropertyAttribute> recognizeElement1 = iRecognize<TModelAttribute, TPropertyAttribute>(property.GetValue(context), ref totalDeph);
                    if (recognizeElement1 != null)
                    {
                        recognizeElement1.Attribute = modelAttribute;
                        recognizeElement.Models.Add(recognizeElement1);
                    }
                }
            }

            return recognizeElement;
        }

        public static RecognizedModel<TModelAttribute, TPropertyAttribute> RecognizeModelElements<TModelAttribute, TPropertyAttribute>(object context, int deph)
            where TModelAttribute : SubmodelAttribute
            where TPropertyAttribute : ModelPropertyAttribute
        {
            int totalDeph = deph;
            return iRecognize<TModelAttribute, TPropertyAttribute>(context, ref totalDeph);
        }

        public static RecognizedModel RecognizeModelElements(object context, int deph)
        {
            int totalDeph = deph;
            return iRecognize<SubmodelAttribute, ModelPropertyAttribute>(context, ref totalDeph);
        }

        private static RecognizedViewModel<TModelAttribute, TPropertyAttribute> iRecognizeViewModelElements<TModelAttribute, TPropertyAttribute>(object context, ref int deph)
            where TModelAttribute : SubViewModelAttribute
            where TPropertyAttribute : UIPropertyAttribute
        {
            if (deph == 0)
            {
                return null;
            }

            deph--;

            RecognizedViewModel<TModelAttribute, TPropertyAttribute> recognizeElement = new RecognizedViewModel<TModelAttribute, TPropertyAttribute>();
            recognizeElement.Model = context;

            var properties = context.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyAttribute = property.GetCustomAttribute(typeof(TPropertyAttribute)) as TPropertyAttribute;
                if (propertyAttribute != null)
                {
                    recognizeElement.Properties.Add(new RecognizedUIPropertyElement<UIPropertyAttribute>
                    {
                        Attribute = propertyAttribute,
                        Info = property,
                        Property = property.GetValue(context)
                    });
                }

                var modelAttribute = property.GetCustomAttribute(typeof(TModelAttribute)) as TModelAttribute;
                if (modelAttribute != null)
                {
                    int totalDeph = deph;
                    RecognizedViewModel<TModelAttribute, TPropertyAttribute> recognizeElement1 = iRecognizeViewModelElements<TModelAttribute, TPropertyAttribute>(property.GetValue(context), ref totalDeph);
                    if (recognizeElement1 != null)
                    {
                        recognizeElement1.Attribute = modelAttribute;
                        recognizeElement.Models.Add(recognizeElement1);
                    }
                }
            }

            return recognizeElement;
        }

        public static RecognizedViewModel<TModelAttribute, TPropertyAttribute> RecognizeViewModelElements<TModelAttribute, TPropertyAttribute>(object context, int deph)
            where TModelAttribute : SubViewModelAttribute
            where TPropertyAttribute : UIPropertyAttribute
        {
            int totalDeph = deph;
            return iRecognizeViewModelElements<TModelAttribute, TPropertyAttribute>(context, ref totalDeph);
        }

        public static RecognizedViewModel RecognizeViewModelElements(object context, int deph)
        {
            int totalDeph = deph;
            return iRecognizeViewModelElements<SubViewModelAttribute, UIPropertyAttribute>(context, ref totalDeph);
        }

        public static void Construct(Constructor constructor, RecognizedModel model, RecognizedViewModel viewModel)
        {
            RecognizedViewModel viewModelHead = viewModel;
            while (viewModel != null)
            {

            }
        }
    }
}

﻿using System.Windows;
using System.Windows.Controls;

namespace xLibV100.UI.TemplateSelectors
{
    public class TemplateSelectorForSubscriptionOfPort : DataTemplateSelector
    {
        public DataTemplate DataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return DataTemplate;
        }
    }
}

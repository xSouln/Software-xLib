using System;

namespace xLibV100.Controls
{
    /// <summary>
    /// атрибут показывающй что данное свойство может содержать вложенные модели
    /// </summary>
    public class SubmodelAttribute : Attribute
    {
        public string Key;
    }
}

using System;

namespace xLibV100.Controls
{
    [Flags]
    public enum ModelPropertyFlags
    {
        None = 0,

        ReadOnly = 1 << 0,

    }

    /// <summary>
    /// атрибут которым отмечаются свойсвта объекта которые будут обработаны при сборке view model и вывода этих свойств в view
    /// </summary>
    public class ModelPropertyAttribute : Attribute
    {
        /// <summary>
        /// ключь значение
        /// </summary>
        public string Key = null;

        /// <summary>
        /// переопределяет имя
        /// </summary>
        public string Name = null;

        public string Element = null;

        /// <summary>
        /// определение подгрупп свойств
        /// </summary>
        public string Group = null;

        /// <summary>
        /// дополнительные флаги
        /// </summary>
        public ModelPropertyFlags Flags;

        public bool ReadOnly => (Flags & ModelPropertyFlags.ReadOnly) > 0;
    }
}

using System;

namespace xLibV100.Transactions
{
    /// <summary>
    /// атрибут маркирующий свойство как транзакцию для ее создания через рефлексию на этапе инициализации
    /// </summary>
    public class TxTransactionAttribute : Attribute
    {
        public string Key;
        public object Action;
    }
}

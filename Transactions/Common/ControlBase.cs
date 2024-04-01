using System.Reflection;
using System;
using xLibV100.Transceiver;

namespace xLibV100.Transactions.Common
{
    /// <summary>
    /// предоставляет логику инициализации определенных запросов посредством
    /// атрибутов RxTransaction и TxTransaction
    /// </summary>
    public class ControlBase : TransactionsBase, IControl
    {
        public ControlBase(object model, uint uid) : base(model)
        {
            UID = uid;

            var fields = GetType().GetFields();

            foreach (var field in fields)
            {
                object customAttribute = field.GetCustomAttribute(typeof(RxTransactionAttribute));

                if (customAttribute is RxTransactionAttribute)
                {
                    var attribute = (RxTransactionAttribute)customAttribute;
                    ReceiverBase transaction;

                    switch (attribute.Key)
                    {
                        default:
                            transaction = Activator.CreateInstance(field.FieldType, this, attribute.Action) as ReceiverBase;
                            break;
                    }

                    field.SetValue(this, transaction);
                    List.Add(transaction);
                }

                customAttribute = field.GetCustomAttribute(typeof(TxTransactionAttribute));

                if (customAttribute is TxTransactionAttribute)
                {
                    var attribute = (TxTransactionAttribute)customAttribute;

                    field.SetValue(this, Activator.CreateInstance(field.FieldType, this, attribute.Action));
                }
            }
        }

        /// <summary>
        /// идентификация пакета данных на принадлежность к определенным/отправленным транзакциям
        /// </summary>
        /// <param name="manager">предоставляет объект менеджера приема данных</param>
        /// <param name="content">предостовляет пакет принятых данные</param>
        /// <returns></returns>
        public unsafe bool Identification(RxPacketManager manager, xContent content)
        {
            if (manager.Packet != null && manager.Packet->Header.UID != UID)
            {
                return false;
            }

            if (Handle.Receive(manager, content) != ReceiverResult.NotFound)
            {
                return true;
            }

            foreach (ReceiverBase response in List)
            {
                if (response.Receive(manager, content) != ReceiverResult.NotFound)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

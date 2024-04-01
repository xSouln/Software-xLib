using System.Collections.Generic;
using xLibV100.Controls;
using xLibV100.Transceiver;

namespace xLibV100.Transactions.Common
{
    /// <summary>
    /// класс предостовляющий базовое представление свойств транзакции
    /// </summary>
    public class TransactionsBase : ModelBase
    {
        protected uint deviceId = 0;
        protected uint uid = 0;

        public ResponseHandle Handle = new ResponseHandle();
        public List<ReceiverBase> List = new List<ReceiverBase>();

        public TransactionsBase(object model)
        {
            parent = model;
        }

        /// <summary>
        /// Id устройства к которому отпровляется запрос
        /// </summary>
        public uint DeviceId
        {
            get => deviceId;
            set
            {
                if (deviceId != value)
                {
                    deviceId = value;
                    OnPropertyChanged(nameof(DeviceId), value);
                }
            }
        }

        /// <summary>
        /// определяет uid сервиса предостовляющий функции управления
        /// </summary>
        public uint UID
        {
            get => uid;
            set
            {
                if (uid != value)
                {
                    uid = value;
                    OnPropertyChanged(nameof(UID), uid);
                }
            }
        }
    }
}

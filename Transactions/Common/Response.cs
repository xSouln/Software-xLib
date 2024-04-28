using xLibV100.Transceiver;

namespace xLibV100.Transactions.Common
{
    /// <summary>
    /// предостовляет шаблон ответа на запрос
    /// </summary>
    /// <typeparam name="TValue">тип предостовляющий набор параметров входящих в ответа</typeparam>
    public class Response<TValue> : IResponseAdapter where TValue : unmanaged
    {
        public TValue[] Values = null;

        public unsafe object Recieve(RxPacketManager manager, xContent content)
        {
            TValue* values = (TValue*)content.Data;

            int count = content.DataSize / sizeof(TValue);
            Values = new TValue[count];

            for (int i = 0; i < count; i++)
            {
                Values[i] = values[i];
            }

            return this;
        }
    }

    public class ResponseResult : IResponseAdapter
    {
        public ActionResult Result;

        public object Recieve(RxPacketManager manager, xContent content)
        {
            content.Get(out Result);

            return this;
        }
    }
}

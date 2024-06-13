using System.Collections.Generic;
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
            List<TValue> elements = new List<TValue>();

            while (content.Get(out TValue element) == 0)
            {
                elements.Add(element);
            }

            Values = elements.ToArray();

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

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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

        public TValue this[int index]
        {
            get
            {
                try
                {
                    return Values[index];
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public TValue Value
        {
            get
            {
                if (Values == null || !(Values.Length > 0))
                {
                    throw new Exception("value missing");
                }

                return Values[0];
            }
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

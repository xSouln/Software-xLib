using System.Collections.Generic;
using xLibV100.Common;
using xLibV100.Transceiver;

namespace xLibV100.Transactions.Common
{
    /// <summary>
    /// базовый шаблон запроса
    /// </summary>
    /// <typeparam name="TContent">тип предостовляющий набор параметров запроса</typeparam>
    public class Request<TContent> : IRequestAdapter where TContent : unmanaged
    {
        public TContent Content;

        public Request(TContent content)
        {
            Content = content;
        }

        public int Add(List<byte> buffer)
        {
            return xMemory.Add(buffer, Content);
        }

        public unsafe int GetSize()
        {
            return sizeof(TContent);
        }
    }
}

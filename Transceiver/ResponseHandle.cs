using System.Collections.Generic;
using System.Threading;

namespace xLibV100.Transceiver
{
    public enum TxStatus
    {
        Free,
        Prepare,
        IsTransmit,
        Complite,
        TimeOut,
        TransmiteError,
        TransmiteActionError,
        Busy,
        Cancelled,
        AddError
    }

    public enum TxResponses
    {
        NoResponse,

        Accept,
        NotSuported
    }

    public class ResponseHandle
    {
        protected List<RequestBase> requests = new List<RequestBase>();
        protected AutoResetEvent rwSynchronizer = new AutoResetEvent(true);
        protected Semaphore queueSize;
        protected Thread thread;

        public ResponseHandle(int line_size)
        {
            if (line_size < 1)
            {
                line_size = 10;
            }
            queueSize = new Semaphore(line_size, line_size);
        }

        public ResponseHandle()
        {
            queueSize = new Semaphore(10, 10);
        }

        protected virtual void Update()
        {
            int i = 0;
            while (i < requests.Count)
            {
                switch (requests[i].Status)
                {
                    case TxStatus.Complite:
                        //requests[i].Accept();
                        requests.RemoveAt(i);
                        break;

                    case TxStatus.IsTransmit:
                        i++;
                        break;

                    case TxStatus.Prepare:
                        i++;
                        break;

                    default: requests.RemoveAt(i);
                        break;
                }
            }
        }

        public virtual bool Add(RequestBase request)
        {
            try
            {
                rwSynchronizer.WaitOne();

                Update();
                if (requests.Count >= 20)
                {
                    return false;
                }
                requests.Add(request);
            }
            finally
            {
                rwSynchronizer.Set();
            }

            return true;
        }

        public virtual void Remove(RequestBase request)
        {
            try
            {
                rwSynchronizer.WaitOne();

                for (int i = 0; i < this.requests.Count; i++)
                {
                    if (this.requests[i] == request)
                    {
                        requests.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            finally
            {
                rwSynchronizer.Set();
            }
        }

        public virtual ReceiverResult Receive(RxPacketManager manager, xContent content)
        {
            ReceiverResult result = ReceiverResult.NotFound;
            try
            {
                rwSynchronizer.WaitOne();

                Update();
                for (int i = 0; i < requests.Count; i++)
                {
                    result = requests[i].GetReceiver().Receive(manager, content);
                    if (result == ReceiverResult.Accept || result == ReceiverResult.NotSuported)
                    {
                        requests[i].Accept(result);
                        requests.RemoveAt(i);
                        break;
                    }
                }
            }
            finally
            {
                rwSynchronizer.Set();
            }
            return result;
        }
        /*
        public virtual bool Accept(TransmitterBase request)
        {
            bool result = false;
            try
            {
                read_write_synchronizer.WaitOne();

                for (int i = 0; i < requests.Count; i++)
                {
                    result = requests[i] == request;
                    if (result)
                    {
                        requests[i].Accept(result);
                        requests.RemoveAt(i);
                        break;
                    }
                }
            }
            finally
            {
                read_write_synchronizer.Set();
            }
            return result;
        }*/
    }
}

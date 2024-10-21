using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using xLibV100.Components;
using xLibV100.Ports;
using xLibV100.Transactions;
using xLibV100.Transceiver;
using xLibV100.UI;

namespace xLibV100.Controls
{
    public class TerminalTransactionRequest
    {
        public object Sender;

        public RequestBase Request;
        public string Description;
        public object Content;

        public int Timeout;
        public int TryNumber;
    }

    public class TerminalTransactionControl : IDisposable
    {
        protected CancellationTokenSource transactionRequestsHandlerTokenSource = new CancellationTokenSource();
        protected AutoResetEvent transactionSynchronize;
        protected List<TerminalTransactionRequest> transactionRequests = new List<TerminalTransactionRequest>();

        protected UINotifyPropertyChanged model;
        protected PortBase port;

        public TerminalTransactionControl(UINotifyPropertyChanged model, PortBase port)
        {
            this.port = port;
            this.model = model;

            model.ValuePropertyChanged += ValuePropertyChanged;

            Task.Run(TransactionRequestHandler, transactionRequestsHandlerTokenSource.Token);
        }

        private void ValuePropertyChanged(object sender, PropertyChangedEventHandlerArg<object> args)
        {
            port = args.State as PortBase; 
        }

        protected async void TransactionRequestHandler()
        {
            while (true)
            {
                transactionSynchronize.WaitOne();

                TerminalTransactionRequest element = null;

                if (transactionRequests.Count > 0)
                {
                    element = transactionRequests[0];
                }

                transactionSynchronize.Set();

                if (element != null)
                {
                    xTracer.Trace(await element.Request.TransmitAsync(port, 1, 2000), element.Description);

                    transactionSynchronize.WaitOne();
                    transactionRequests.Remove(element);
                    transactionSynchronize.Set();
                }

                await Task.Delay(1);
            }
        }

        public void Dispose()
        {
            model.ValuePropertyChanged -= ValuePropertyChanged;
        }
    }
}

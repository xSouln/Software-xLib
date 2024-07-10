using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using xLibV100.Components;
using xLibV100.Ports;
using xLibV100.Transactions;
using xLibV100.Transceiver;

namespace xLibV100.Controls
{
    public abstract class TerminalObject : ModelBase<TerminalBase>, ITerminalObject
    {
        protected Task updateStatesTask;
        protected CancellationTokenSource updateStatesTaskTokenSource;
        protected CancellationTokenSource transactionRequestsHandlerTokenSource = new CancellationTokenSource();
        protected AutoResetEvent transactionSynchronize;
        protected PortBase selectedPort;
        protected PortBase lastSelectedPort;
        protected uint updateStatePeriod = 800;
        protected uint id;

        public event PropertyChangedEventHandler<TerminalObject, PortBase> SelectedPortChanged;
        public event xPropertyChangedEventHandler<PortBase, ConnectionStateChangedEventHandlerArg> SelectedPortConnectionChanged;

        protected List<TerminalTransactionRequest> transactionRequests = new List<TerminalTransactionRequest>();
        public ObservableCollection<PortBase> Ports { get; protected set; } = new ObservableCollection<PortBase>();

        public ObservableCollection<object> Models = new ObservableCollection<object>();

        public TerminalBase Terminal { get; set; }

        public TerminalObject(TerminalBase model) : base(model)
        {
            Terminal = model;

            updateStatesTaskTokenSource = new CancellationTokenSource();
            transactionSynchronize = new AutoResetEvent(true);
            updateStatesTask = Task.Run(StateUpdateTask, updateStatesTaskTokenSource.Token);

            //Ports.CollectionChanged += PortsCollectionChanged;
            Task.Run(TransactionRequestHandler, transactionRequestsHandlerTokenSource.Token);
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
                    if (element.Transaction.Status != TxStatus.Cancelled)
                    {
                        xTracer.Trace(await element.Transaction.TransmitAsync(SelectedPort,
                            element.TryNumber > 0 ? element.TryNumber : 1,
                            element.Timeout > 0 ? element.Timeout : 2000), element.Description);
                    }

                    transactionSynchronize.WaitOne();
                    transactionRequests.Remove(element);
                    transactionSynchronize.Set();
                }

                await Task.Delay(1);
            }
        }

        public int AddTransactionToLine(TxTransactionBase transaction, string description)
        {
            transactionSynchronize.WaitOne();

            transactionRequests.Add(new TerminalTransactionRequest { Transaction = transaction, Description = description });

            transactionSynchronize.Set();

            return 0;
        }

        public int AddTransactionToLine(TxTransactionBase transaction, string description, int tryNumber, int timeout)
        {
            transactionSynchronize.WaitOne();

            transactionRequests.Add(new TerminalTransactionRequest
            {
                Transaction = transaction,
                Description = description,
                TryNumber = tryNumber,
                Timeout = timeout
            });

            transactionSynchronize.Set();

            return 0;
        }

        public uint Id
        {
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged(nameof(Id), id);
                }
            }
        }

        protected virtual Task StateUpdateHandler(object arg)
        {
            return Task.CompletedTask;
        }

        public int Subscribe(PortBase port)
        {
            if (port != null && Ports != null && !Ports.Contains(port))
            {
                var result = port.AddListener(this);
                if (result != null)
                {
                    Ports.Add(result);
                    return 0;
                }
            }
            return -1;
        }

        private void SelectedPortConnectionChangedHandler(PortBase port, ConnectionStateChangedEventHandlerArg arg)
        {
            SelectedPortConnectionChanged?.Invoke(port, arg);
        }

        public PortBase SelectedPort
        {
            get => selectedPort;
            set
            {
                if (selectedPort != value)
                {
                    lastSelectedPort = selectedPort;
                    selectedPort = value;

                    if (selectedPort != null)
                    {
                        selectedPort.ConnectionStateChanged += SelectedPortConnectionChangedHandler;
                    }

                    if (lastSelectedPort != null)
                    {
                        lastSelectedPort.ConnectionStateChanged -= SelectedPortConnectionChangedHandler;
                    }

                    SelectedPortChanged?.Invoke(this, selectedPort, lastSelectedPort);
                }
            }
        }

        public int UnSubscribe(PortBase port)
        {
            var result = port.RemoveListener(this);
            Ports?.Remove(result);
            return 0;
        }

        protected async virtual Task StateUpdateTask()
        {
            Stopwatch update_time = new Stopwatch();

            CancellationToken token = updateStatesTaskTokenSource.Token;

            try
            {
                while (true && !token.IsCancellationRequested)
                {
                    int time = 0;

                    if (SelectedPort != null)
                    {
                        update_time.Restart();

                        await StateUpdateHandler(this);

                        update_time.Stop();

                        time += (int)update_time.ElapsedMilliseconds;
                    }

                    int delay = (int)updateStatePeriod - time;

                    if (delay > 0)
                    {
                        await Task.Delay(delay, token);
                    }
                }
            }
            catch (Exception ex)
            {
                xTracer.Message(ex.ToString());
            }
        }

        public virtual bool ResponseIdentification(RxPacketManager manager, xContent content)
        {
            return false;
        }

        public override void Dispose()
        {
            base.Dispose();

            SelectedPortChanged = null;
            SelectedPortConnectionChanged = null;

            transactionRequestsHandlerTokenSource.Cancel();
            transactionRequestsHandlerTokenSource.Dispose();

            if (updateStatesTaskTokenSource != null)
            {
                updateStatesTaskTokenSource?.Cancel();
                while (updateStatesTask.Status == TaskStatus.Running)
                {
                    Thread.Sleep(1);
                }
                updateStatesTaskTokenSource?.Dispose();
                updateStatesTaskTokenSource = null;
            }

            foreach (var element in Models)
            {
                if (element is IDisposable model)
                {
                    model.Dispose();
                }
            }

            Models.Clear();
        }

        [ModelProperty]
        public uint UpdateStatePeriod
        {
            get => updateStatePeriod;
            set
            {
                if (value != updateStatePeriod)
                {
                    updateStatePeriod = value;
                    OnPropertyChanged(nameof(UpdateStatePeriod), updateStatePeriod);
                }
            }
        }
    }
}

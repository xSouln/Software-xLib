using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using xLibV100.Components;
using xLibV100.Ports;
using xLibV100.Transceiver;

namespace xLibV100.Controls
{
    public abstract class TerminalObject : ModelBase<TerminalBase>, ITerminalObject
    {
        protected Task updateStatesTask;
        protected CancellationTokenSource updateStatesTaskTokenSource;
        protected PortBase selectedPort;
        protected PortBase lastSelectedPort;
        protected int UpdateStatePeriod = 800;
        protected uint id;

        public event PropertyChangedEventHandler<TerminalObject, PortBase> SelectedPortChanged;
        public event xPropertyChangedEventHandler<PortBase, ConnectionStateChangedEventHandlerArg> SelectedPortConnectionChanged;

        public ObservableCollection<PortBase> Ports { get; protected set; } = new ObservableCollection<PortBase>();

        public ObservableCollection<object> Models = new ObservableCollection<object>();

        public TerminalBase Terminal { get; set; }

        public TerminalObject(TerminalBase model) : base(model)
        {
            Terminal = model;

            updateStatesTaskTokenSource = new CancellationTokenSource();
            updateStatesTask = Task.Run(StateUpdateTask, updateStatesTaskTokenSource.Token);

            //Ports.CollectionChanged += PortsCollectionChanged;
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

            try
            {
                while (true && updateStatesTaskTokenSource != null)
                {
                    int time = 0;

                    if (SelectedPort != null)
                    {
                        update_time.Restart();

                        await StateUpdateHandler(this);

                        update_time.Stop();

                        time += (int)update_time.ElapsedMilliseconds;
                    }

                    int delay = UpdateStatePeriod - time;

                    if (delay > 0)
                    {
                        await Task.Delay(delay, updateStatesTaskTokenSource.Token);
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
        }
    }
}

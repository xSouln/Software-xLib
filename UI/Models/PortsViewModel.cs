using xLibV100.UI;
using xLibV100.UI.Views;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using xLibV100.Controls;
using xLibV100.Ports;

namespace xLibV100.Common.UI.Models
{
    public class PortsViewModel : ViewModelBase<TerminalObject, PortsView>, IPortsViewModel
    {
        public TerminalObject Device { get; set; }
        public ObservableCollection<SubscriptionOfPortViewModel> SubscriptionsOfPorts { get; set; } = new ObservableCollection<SubscriptionOfPortViewModel>();
        protected SubscriptionOfPortViewModel lastSelectedPort;

        private TerminalBase Terminal { get; set; }

        public PortsViewModel(TerminalObject model) : base(model)
        {
            Name = "Available ports";

            Device = model;
            Model = model;

            Terminal = model.Terminal;

            Terminal.UI.ValuePropertyChanged += PropertyValueChangedHandler;

            model.SelectedPortChanged += SelectedPortChanged;
            model.Ports.CollectionChanged += PortsChanged;
            model.Terminal.AvailablePorts.CollectionChanged += AvailablePortsRemove;

            foreach (var element in Model.Ports)
            {
                var port = new SubscriptionOfPortViewModel(element);
                SubscriptionsOfPorts.Add(port);

                if (element == model.SelectedPort)
                {
                    port.TxIsSelected = true;
                    SelectedSubscribedPort = port;
                }
            }

            var frameworkElement = new FrameworkElementFactory(typeof(PortsView));
            frameworkElement.SetValue(FrameworkElement.DataContextProperty, this);

            Template = new DataTemplate { VisualTree = frameworkElement };

            View = new PortsView(this);
        }

        private void PropertyValueChangedHandler(object sender, PropertyChangedEventHandlerArg<object> args)
        {
            switch (args.Name)
            {
                case nameof(Terminal.UI.ConnectionsViewModel):
                    if (Terminal.UI.ConnectionsViewModel != null)
                    {
                        Terminal.UI.ConnectionsViewModel.ViewUpdateEvent += TerminalConnectionsViewModelUpdateEventHandler;
                        Terminal.UI.ConnectionsViewModel.UpdateEvent += TerminalConnectionsViewModelUpdateEventHandler;
                    }
                    break;
            }
        }

        private void TerminalConnectionsViewModelUpdateEventHandler(ViewModelBase viewModel)
        {
            OnUpdateEvent();
        }

        private void AvailablePortsRemove(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var element in SubscriptionsOfPorts)
                {
                    if (element.Port == (PortBase)e.OldItems[0])
                    {
                        SubscriptionsOfPorts.Remove(element);

                        if (SelectedSubscribedPort == element)
                        {
                            SelectedSubscribedPort = null;
                        }
                        return;
                    }
                }
            }
        }

        private void PortsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                SubscriptionsOfPorts.Add(new SubscriptionOfPortViewModel((PortBase)e.NewItems[0]));
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var element in SubscriptionsOfPorts)
                {
                    if (element.Port == (PortBase)e.OldItems[0])
                    {
                        SubscriptionsOfPorts.Remove(element);

                        if (SelectedSubscribedPort == element)
                        {
                            SelectedSubscribedPort = null;
                        }
                        return;
                    }
                }
            }
        }

        private void SelectedPortChanged(TerminalObject obj, PortBase state, PortBase lastState)
        {
            if (lastSelectedPort != null)
            {
                lastSelectedPort.TxIsSelected = false;
                lastSelectedPort = null;
            }

            if (state != null)
            {
                foreach (var element in SubscriptionsOfPorts)
                {
                    if (element.Port == state)
                    {
                        lastSelectedPort = element;
                        SelectedSubscribedPort = element;
                        SelectedSubscribedPort.TxIsSelected = true;
                        return;
                    }
                }
            }
        }

        public ViewModelBase SelectedPort { get; set; }

        public SubscriptionOfPortViewModel SelectedSubscribedPort { get; set; }

        public void Subscribe()
        {
            if (SelectedPort.Model is PortBase port)
            {
                Device?.Subscribe(port);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            Model.Ports.CollectionChanged -= PortsChanged;
            Model.Terminal.AvailablePorts.CollectionChanged -= AvailablePortsRemove;

            foreach (var element in SubscriptionsOfPorts)
            {
                element?.Dispose();
            }
        }

        public void Unsubscribe()
        {
            if (SelectedSubscribedPort != null)
            {
                Device?.UnSubscribe(SelectedSubscribedPort.Port);
                //SubscriptionsOfPorts?.Remove(SelectedPort);
            }
        }

        public void SelectTxLine()
        {
            Device.SelectedPort = SelectedSubscribedPort != null ? SelectedSubscribedPort.Port : null;
        }

        public void ResetTxLine()
        {
            Device.SelectedPort = null;
        }
    }
}

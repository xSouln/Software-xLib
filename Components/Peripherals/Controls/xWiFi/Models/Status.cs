using xLibV100.Peripherals.xWiFi.Models.Interfaces;
using xLibV100.Peripherals.xWiFi.Transactions;
using xLibV100.Peripherals.xWiFi.Types;
using xLibV100.Common;
using xLibV100.Controls;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xWiFi.Models
{
    public class Status : ModelBase<Control>, IStatus
    {
        public delegate void AlementChangedEventHandler(Status model, string name, object value);

        protected StatusT status = new StatusT();
        protected AddressT address = new AddressT();

        public event AlementChangedEventHandler AlementChanged;

        protected Control Control { get; set; }

        public Status(Control model) : base(model)
        {
            Name = nameof(Status);

            Control = model;

            Get.Status.ResponseReceiver += GetStatusEventReceive;
            Get.Address.ResponseReceiver += GetAddressEventReceive;

            Events.StatusChanged.Receiver += EventStatusChanged;

            //ViewModel = new StatusViewModel(this);
        }

        private void EventStatusChanged(RxPacketManager obj, Events.Event<StatusT> arg)
        {
            if (arg.Values != null)
            {
                foreach (var element in arg.Values)
                {
                    if (element.Number == 0)
                    {
                        Value = element.Element;
                        return;
                    }
                }
            }
        }

        private void GetAddressEventReceive(RxPacketManager obj, Get.Response<AddressT> arg)
        {
            if (arg.Values != null)
            {
                foreach (var element in arg.Values)
                {
                    if (element.Number == 0)
                    {
                        Address = element.Element;
                        return;
                    }
                }
            }
        }

        public AddressT Address
        {
            get => address;
            set
            {
                if (!xMemory.Compare(ref address, ref value))
                {
                    var previose = address;
                    address = value;

                    AlementChanged?.Invoke(this, nameof(value), value);

                    if (previose.Ip != value.Ip)
                    {
                        AlementChanged?.Invoke(this, nameof(value.Ip), value.Ip);
                    }

                    if (previose.Gateway != value.Gateway)
                    {
                        AlementChanged?.Invoke(this, nameof(value.Gateway), value.Gateway);
                    }

                    if (previose.Netmask != value.Netmask)
                    {
                        AlementChanged?.Invoke(this, nameof(value.Netmask), value.Netmask);
                    }
                }
            }
        }

        protected async void UpdateAddress()
        {
            await Control.GetAddress(Module.Number1);
        }

        protected async void UpdateConfig()
        {
            await Control.GetConfig(Module.Number1);
        }

        public StatusT Value
        {
            get => status;
            set
            {
                if (!xMemory.Compare(ref status, ref value))
                {
                    var previose = status;
                    status = value;

                    AlementChanged?.Invoke(this, nameof(value), value);

                    if (previose.IsEnable != value.IsEnable)
                    {
                        AlementChanged?.Invoke(this, nameof(value.IsEnable), value.IsEnable);
                    }

                    if (previose.IsStarted != value.IsStarted)
                    {
                        AlementChanged?.Invoke(this, nameof(value.IsStarted), value.IsStarted);
                    }

                    if (previose.IsInit != value.IsInit)
                    {
                        AlementChanged?.Invoke(this, nameof(value.IsInit), value.IsInit);

                        if (value.IsInit)
                        {
                            UpdateConfig();
                        }
                    }

                    if (previose.State != value.State)
                    {
                        AlementChanged?.Invoke(this, nameof(value.State), value.State);

                        if (value.State == States.Idle || value.State == States.Connected)
                        {
                            UpdateAddress();
                        }
                    }

                    if (previose.Error != value.Error)
                    {
                        AlementChanged?.Invoke(this, nameof(value.Error), value.Error);
                    }
                }
            }
        }

        private void GetStatusEventReceive(RxPacketManager obj, Get.Response<StatusT> arg)
        {
            if (arg.Values != null)
            {
                foreach (var element in arg.Values)
                {
                    if (element.Number == 0)
                    {
                        Value = element.Element;
                        return;
                    }
                }
            }
        }
    }
}

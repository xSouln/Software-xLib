using System.Collections.ObjectModel;
using xLibV100.Controls;
using xLibV100.Serializers;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals
{
    public class PeripheralControl : TerminalObject
    {
        protected bool gsmIsEnabled;
        protected bool ethernetIsEnabled;
        protected bool mqttIsEnabled;
        protected bool memoryControlIsEnabled;

        protected GsmControl.Gsm gsm;
        protected EthernetControl.Ethernet ethernet;
        protected MqttControl.Mqtt mqtt;
        protected MemoryControl.MemoryControl memoryControl;

        public ObservableCollection<PeripheralBase> Peripherals { get; protected set; } = new ObservableCollection<PeripheralBase>();

        public PeripheralControl(TerminalBase model) : base(model)
        {
            GsmIsEnabled = true;
            MemoryControlIsEnabled = true;

            Services.RegisterByType(this);

            PortSubscriptions.Open(this, "Components\\PeripheralsControl\\Saves");
        }

        public override void Dispose()
        {
            base.Dispose();

            PortSubscriptions.Save(this, "Components\\PeripheralsControl\\Saves");
        }

        public override unsafe bool ResponseIdentification(RxPacketManager manager, xContent content)
        {
            if (manager.Packet != null && manager.Packet->Header.DeviceId != Id)
            {
                return false;
            }

            foreach (var element in Peripherals)
            {
                if (element.ResponseIdentification(manager, content))
                {
                    return true;
                }
            }

            return false;
        }


        [ModelProperty]
        public bool GsmIsEnabled
        {
            get => gsmIsEnabled;
            set
            {
                if (gsmIsEnabled != value)
                {
                    gsmIsEnabled = value;
                    OnPropertyChanged(nameof(gsmIsEnabled), gsmIsEnabled);

                    if (gsmIsEnabled)
                    {
                        gsm = new GsmControl.Gsm(this);

                        Peripherals.Add(gsm);
                    }
                    else
                    {
                        Peripherals.Remove(gsm);

                        gsm.Dispose();
                        gsm = null;
                    }
                }
            }
        }


        [ModelProperty]
        public bool EthernetIsEnabled
        {
            get => ethernetIsEnabled;
            set
            {
                if (ethernetIsEnabled != value)
                {
                    ethernetIsEnabled = value;
                    OnPropertyChanged(nameof(EthernetIsEnabled), ethernetIsEnabled);

                    if (ethernetIsEnabled)
                    {
                        ethernet = new EthernetControl.Ethernet(this);

                        Peripherals.Add(ethernet);
                    }
                    else
                    {
                        Peripherals.Remove(ethernet);

                        ethernet.Dispose();
                        ethernet = null;
                    }
                }
            }
        }


        [ModelProperty]
        public bool MemoryControlIsEnabled
        {
            get => memoryControlIsEnabled;
            set
            {
                if (memoryControlIsEnabled != value)
                {
                    memoryControlIsEnabled = value;
                    OnPropertyChanged(nameof(MemoryControlIsEnabled), memoryControlIsEnabled);

                    if (memoryControlIsEnabled)
                    {
                        memoryControl = new MemoryControl.MemoryControl(this);
                        Peripherals.Add(memoryControl);
                    }
                    else
                    {
                        Peripherals.Remove(memoryControl);

                        memoryControl.Dispose();
                        memoryControl = null;
                    }
                }
            }
        }



        [ModelProperty]
        public bool MqttIsEnabled
        {
            get => mqttIsEnabled;
            set
            {
                if (mqttIsEnabled != value)
                {
                    mqttIsEnabled = value;
                    OnPropertyChanged(nameof(MqttIsEnabled), mqttIsEnabled);

                    if (mqttIsEnabled)
                    {
                        mqtt = new MqttControl.Mqtt(this);

                        Peripherals.Add(mqtt);
                    }
                    else
                    {
                        Peripherals.Remove(mqtt);

                        mqtt.Dispose();
                        mqtt = null;
                    }
                }
            }
        }
    }
}

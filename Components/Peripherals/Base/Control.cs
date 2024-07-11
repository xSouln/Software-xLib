using System.Collections.ObjectModel;
using xLibV100.Controls;

namespace xLibV100.Peripherals
{
    public class Control : TerminalObject
    {
        protected bool gsmIsEnabled;
        protected bool ethernetIsEnabled;
        protected bool mqttIsEnabled;

        protected GsmControl.Gsm gsm;
        protected EthernetControl.Ethernet ethernet;
        protected MqttControl.Mqtt mqtt;

        public ObservableCollection<PeripheralBase> Peripherals { get; protected set; } = new ObservableCollection<PeripheralBase>();

        public Control(TerminalBase model) : base(model)
        {

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

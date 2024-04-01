using xLibV100.Peripherals.xWiFi.Models.Interfaces;
using xLibV100.Peripherals.xWiFi.Transactions;
using xLibV100.Peripherals.xWiFi.Types;
using System.Text;
using xLibV100.Common;
using xLibV100.Controls;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.xWiFi.Models
{
    public class Config : ModelBase<Control>, IConfig
    {
        public delegate void ConfigChangedEventHandler(Config model, ref ConfigT config);
        public delegate void AlementChangedEventHandler(Config model, string name, object value);

        protected ConfigT config = new ConfigT();

        public event ConfigChangedEventHandler ConfigChangedEvent;
        public event AlementChangedEventHandler AlementChangedEvent;

        public Config(Control control) : base(control)
        {
            Name = nameof(Config);

            Get.Config.ResponseReceiver += GetConfigEventReceive;

            //ViewModel = new ConfigViewModel(this);

            ConfigT config = new ConfigT();
            config.Flags = 0x05;
        }

        private void GetConfigEventReceive(RxPacketManager obj, Get.Response<ConfigT> arg)
        {
            if (arg != null && arg.Values != null && arg.Values.Length > 0)
            {
                foreach (var element in arg.Values)
                {
                    if (element.Number == 0)
                    {
                        Value = element.Element;
                    }
                }
            }
        }

        public ConfigT Value
        {
            get => config;
            set
            {
                if (!xMemory.Compare(ref config, ref value))
                {
                    var previose = config;
                    config = value;

                    ConfigChangedEvent?.Invoke(this, ref value);

                    if (previose.Mode != value.Mode)
                    {
                        AlementChangedEvent?.Invoke(this, nameof(Value.Mode), value.Mode);
                    }

                    if (previose.SSID_BroadcastIsEnable != value.SSID_BroadcastIsEnable)
                    {
                        AlementChangedEvent?.Invoke(this, nameof(Value.SSID_BroadcastIsEnable), value.SSID_BroadcastIsEnable);
                    }

                    unsafe
                    {
                        if (!xMemory.Compare(previose.SSID, value.SSID, 32))
                        {
                            AlementChangedEvent?.Invoke(this,
                                nameof(Value.SSID),
                                Encoding.UTF8.GetString(value.SSID, 32));
                        }

                        if (!xMemory.Compare(previose.Password, value.Password, 64))
                        {
                            AlementChangedEvent?.Invoke(this,
                                nameof(Value.Password),
                                Encoding.UTF8.GetString(value.Password, 64));
                        }
                    }
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}

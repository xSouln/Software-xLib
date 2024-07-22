using xLibV100.Adaptation;

namespace xLibV100.Peripherals.GsmControl
{
    public class SetPropertiesArg : SynchronizedPropertyForSetting
    {
        public SetPropertiesArg(SynchronizedPropertySettingInfoT info, byte[] content) : base(info, content)
        {

        }
    }
}

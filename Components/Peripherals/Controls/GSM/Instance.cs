using System.Collections.ObjectModel;
using System.Threading.Tasks;
using xLibV100.Controls;
using xLibV100.Transactions.Common;

namespace xLibV100.Peripherals.GSM
{
    public partial class Instance : Peripherals.Instance
    {
        protected string imei = "";
        protected string apn = "";
        protected string apnLogic = "";
        protected string apnPassword = "";
        protected StatusMask statusMask;
        protected int signalQuality;
        protected InstanceTypes instanceType;

        public ObservableCollection<MqttInstance> MqttInstances { get; set; } = new ObservableCollection<MqttInstance>();

        public Instance(PeripheralBase model) : base(model)
        {

        }


        public Task<ActionResult> GetCredentialsAsync()
        {
            return Task.FromResult(ActionResult.NotSupported);
        }


        [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]    
        public string IMEI
        {
            get => imei;
            protected set
            {
                if (value != imei)
                {
                    imei = value;
                    OnPropertyChanged(nameof(IMEI), imei);
                }
            }
        }


        [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
        public string APN
        {
            get => apn;
            protected set
            {
                if (value != apn)
                {
                    apn = value;
                    OnPropertyChanged(nameof(APN), apn);
                }
            }
        }


        [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
        public string Login
        {
            get => apnLogic;
            protected set
            {
                if (value != apnLogic)
                {
                    apnLogic = value;
                    OnPropertyChanged(nameof(Login), apnLogic);
                }
            }
        }


        [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
        public string Password
        {
            get => apnPassword;
            protected set
            {
                if (value != apnPassword)
                {
                    apnPassword = value;
                    OnPropertyChanged(nameof(Password), apnPassword);
                }
            }
        }


        [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
        public StatusMask StatusMask
        {
            get => statusMask;
            protected set
            {
                if (value != statusMask)
                {
                    statusMask = value;
                    OnPropertyChanged(nameof(StatusMask), statusMask);
                }
            }
        }


        [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
        public InstanceTypes InstanceType
        {
            get => instanceType;
            protected set
            {
                if (value != instanceType)
                {
                    instanceType = value;
                    OnPropertyChanged(nameof(InstanceType), instanceType);
                }
            }
        }


        [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
        public int SignalQuality
        {
            get => signalQuality;
            protected set
            {
                if (value != signalQuality)
                {
                    signalQuality = value;
                    OnPropertyChanged(nameof(SignalQuality), signalQuality);
                }
            }
        }
    }
}

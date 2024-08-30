using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using xLibV100.Adaptation;
using xLibV100.Common;
using xLibV100.Controls;
using xLibV100.Transactions.Common;
using xLibV100.Transceiver;

namespace xLibV100.Peripherals.GsmControl
{
    public partial class Instance : Instance<Gsm>
    {
        protected string imei = "";
        protected string apn = "";
        protected string apnLogic = "";
        protected string apnPassword = "";
        protected string simCardPassword = "";
        protected int signalQuality;
        protected InstanceTypes instanceType;
        protected NetworkState networkState;
        protected StatusRegisterT statusRegister;

        protected Control Control;

        static Instance()
        {
            PropertyControl = new SynchronizedPropertyControl(typeof(Instance));
        }

        public Instance(Gsm model) : base(model)
        {
            Control = model.Parent;

            Parent.Transactions.GetProperties.ResponseReceiver += GetPropertiesResponseReceiver;

            byte[] result = xMemory.ConvertToArray(new StatusRegisterT[]
            {
                new StatusRegisterT { Value = 100 },
                new StatusRegisterT { Value = 200 }
            });

            if (result != null)
            {
                result[0] += 1;
                result[0] += 1;
            }

            var rr = xMemory.Convert(typeof(StatusRegisterT[]), result);

            if (rr != null)
            {

            }
        }

        private void GetPropertiesResponseReceiver(RxPacketManager obj, Peripherals.Transactions.ResponseGetProperties arg)
        {
            PropertyControl.Apply(arg.Properties);
        }

        [ModelFunction(Name = "Update APN")]
        public async virtual Task<ActionResult> UpdateAPNAsync()
        {
            var request = Parent.Transactions.GetProperties.Prepare(new Peripherals.Transactions.RequestGetProperties((ushort)PropertySelector.APN));
            Control.AddTransactionToLine(request, Name);
            await request.Await();

            if (request.ResponseResult != Transceiver.TxResponses.Accept)
            {
                return ActionResult.Error;
            }

            return ActionResult.Accept;
        }


        [ModelFunction(Name = "Update IMEI")]
        public async virtual Task<ActionResult> UpdateIMEIAsync()
        {
            var request = Parent.Transactions.GetProperties.Prepare(new Peripherals.Transactions.RequestGetProperties((ushort)PropertySelector.IMEI));
            Control.AddTransactionToLine(request, Name);
            await request.Await();

            if (request.ResponseResult != Transceiver.TxResponses.Accept)
            {
                return ActionResult.Error;
            }

            return ActionResult.Accept;
        }


        public async virtual Task<ActionResult> UpdatePropertiesAsync(params PropertySelector[] properties)
        {
            var request = Parent.Transactions.GetProperties.Prepare(new Peripherals.Transactions.RequestGetProperties(properties: properties));
            Control.AddTransactionToLine(request, Name);
            await request.Await();

            if (request.ResponseResult != Transceiver.TxResponses.Accept)
            {
                return ActionResult.Error;
            }

            return ActionResult.Accept;
        }

        public async virtual Task<ActionResult> SetPropertiesAsync(WritableProperty property, params WritableProperty[] properties)
        {
            var request = Parent.Transactions.SetProperties.Prepare(new Peripherals.Transactions.RequestSetProperties(properties));
            Control.AddTransactionToLine(request, Name);
            await request.Await();

            if (request.ResponseResult != Transceiver.TxResponses.Accept)
            {
                return ActionResult.Error;
            }

            return ActionResult.Accept;
        }

        public virtual Task<ActionResult> SetPropertiesAsync(params object[] models)
        {
            List<WritableProperty> request = new List<WritableProperty>();

            foreach (var model in models)
            {
                var properties = model.GetType().GetProperties();

                foreach (var property in properties)
                {
                    if (property.GetCustomAttribute(typeof(SynchronizedPropertyAttribute)) is SynchronizedPropertyAttribute attribute)
                    {
                        if (attribute.PropertySelector == null)
                        {
                            continue;
                        }

                        var content = xMemory.ConvertToArray(property.GetValue(model));

                        if (content == null)
                        {
                            continue;
                        }

                        request.Add(new WritableProperty(new WritablePropertyInfoT
                        {
                            Id = (ushort)attribute.PropertyId,
                            Type = attribute.PropertySelector.Type,
                            Size = (ushort)content.Length,
                        }, content));
                    }
                }
            }

            //await SetPropertiesAsync();

            return Task.FromResult(ActionResult.Accept);
        }

        public async Task<ActionResult> GetCredentialsAsync()
        {
            var request = Parent.Transactions.GetProperties.Prepare(new Peripherals.Transactions.RequestGetProperties(properties: new PropertySelector[]
            {
                PropertySelector.APN,
                PropertySelector.IMEI,
                PropertySelector.Login
            }));

            Control.AddTransactionToLine(request, Name);
            await request.Await();

            if (request.ResponseResult != Transceiver.TxResponses.Accept)
            {
                return ActionResult.Error;
            }

            return ActionResult.Accept;
        }


        [SynchronizedProperty(PropertyId = PropertySelector.Status, CopyingByMapping = true)]
        public StatusRegisterT StatusRegister
        {
            get => statusRegister;
            set
            {
                if (xMemory.Compare(statusRegister, value) != 0)
                {
                    statusRegister = value;
                    OnPropertyChanged(nameof(StatusRegister), statusRegister);
                }
            }
        }


        [SynchronizedProperty(PropertyId = PropertySelector.IMEI)]
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

        [SynchronizedProperty(PropertyId = PropertySelector.APN)]
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

        [SynchronizedProperty(PropertyId = PropertySelector.Login)]
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

        [SynchronizedProperty(PropertyId = PropertySelector.Password)]
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


        [SynchronizedProperty(PropertyId = PropertySelector.Password)]
        [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
        public string SimCardPassword
        {
            get => simCardPassword;
            protected set
            {
                if (value != simCardPassword)
                {
                    simCardPassword = value;
                    OnPropertyChanged(nameof(SimCardPassword), simCardPassword);
                }
            }
        }

        [SynchronizedProperty(PropertyId = PropertySelector.Status, Type = typeof(StatusRegisterT), CopyingByMapping = true)]
        [ModelProperty(Flags = ModelPropertyFlags.ReadOnly)]
        public NetworkState NetworkState
        {
            get => networkState;
            protected set
            {
                if (value != networkState)
                {
                    networkState = value;
                    OnPropertyChanged(nameof(NetworkState), networkState);
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

        [SynchronizedProperty(PropertyId = PropertySelector.SignalQuality)]
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

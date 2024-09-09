using xLibV100.Common;
using xLibV100.Transactions.Common;

namespace xLibV100.Adaptation
{
    public struct RequestedWritablePropertyT
    {
        private ushort value;

        public ushort Id
        {
            get => (ushort)BitsFieldHelper.GetValue(value, mask: 0x3ff, offset: 6);
            set => this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 0x3ff, offset: 6);
        }

        public bool ExtensionIsIncluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 0);
            set
            {
                if (value)
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 0);
                }
                else
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 0);
                }
            }
        }

        public bool ModeIsIncluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 1);
            set
            {
                if (value)
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 1);
                }
                else
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 1);
                }
            }
        }

        public bool TypeSizeVerificationIsIncluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 2);
            set
            {
                if (value)
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 2);
                }
                else
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 2);
                }
            }
        }

        public bool LimitsIsIncluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 3);
            set
            {
                if (value)
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 3);
                }
                else
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 3);
                }
            }
        }

        public bool AnswerIsOnlyToErrors
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 4);
            set
            {
                if (value)
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 4);
                }
                else
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 4);
                }
            }
        }

        public ushort Value => value;
    }

    public struct ReceivedWritablePropertyT
    {
        private ushort value;

        public ushort Id
        {
            get => (ushort)BitsFieldHelper.GetValue(value, mask: 0x3ff, offset: 6);
            set => this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 0x3ff, offset: 6);
        }

        public bool ErrorIsOccurred
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 0);
            set
            {
                if (value)
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 0);
                }
                else
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 0);
                }
            }
        }

        public bool ExtensionIsIncluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 1);
            set
            {
                if (value)
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 1);
                }
                else
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 1);
                }
            }
        }

        public bool ModeIsIncluded
        {
            get => BitsFieldHelper.GetState(value, mask: 1, offset: 2);
            set
            {
                if (value)
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 2);
                }
                else
                {
                    this.value = (ushort)BitsFieldHelper.SetValue(this.value, value, mask: 1, offset: 2);
                }
            }
        }

        public ushort Value => value;
    }

    public class RequestedWritableProperty
    {
        public RequestedWritablePropertyT Info;

        public byte[] Content { get; protected set; }

        public RequestedWritableProperty(RequestedWritablePropertyT info, byte[] content)
        {
            Info = info;
            Content = content;
        }
    }

    public class ReceivedWritableProperty
    {
        public ReceivedWritablePropertyT Info;

        public ushort Extension;
        public ActionResult Result;

        public ReceivedWritableProperty(ReceivedWritablePropertyT info,
            ushort extension = 0,
            ActionResult result = ActionResult.Accept)
        {
            Info = info;

            Extension = extension;
            Result = result;
        }
    }
}

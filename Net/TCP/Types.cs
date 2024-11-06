using System;

namespace xLibV100.Net
{
    public struct NetAddressT
    {
        private uint _value;

        public byte Octet4 => (byte)(_value >> 8 * 3);
        public byte Octet3 => (byte)(_value >> 8 * 2);
        public byte Octet2 => (byte)(_value >> 8 * 1);
        public byte Octet1 => (byte)_value;

        public uint Value
        {
            get => _value;
            set => _value = value;
        }

        public override string ToString()
        {
            return $"{Octet1}.{Octet2}.{Octet3}.{Octet4}";
        }

        public static NetAddressT Create(string value)
        {
            if (value == null || value.Length < 7)
            {
                throw new ArgumentException();
            }

            string[] octets = value.Split('.');
            if (octets.Length != 4)
            {
                throw new ArgumentException();
            }

            uint newValue = 0;
            try
            {
                var octet1 = byte.Parse(octets[0]);
                var octet2 = byte.Parse(octets[1]);
                var octet3 = byte.Parse(octets[2]);
                var octet4 = byte.Parse(octets[3]);

                newValue |= octet4;
                newValue <<= 8;

                newValue |= octet3;
                newValue <<= 8;

                newValue |= octet2;
                newValue <<= 8;

                newValue |= octet1;

                return new NetAddressT { Value = newValue };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

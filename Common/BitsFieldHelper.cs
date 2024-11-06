using System;
using System.Windows;

namespace xLibV100.Common
{
    public static class BitsFieldHelper
    {
        public static ulong SetValue<TSource, TValue>(TSource source, TValue value, ulong mask = 1, int offset = 0)
            where TSource : unmanaged
            where TValue : unmanaged
        {
            ulong _source = Convert.ToUInt64(source);
            ulong _value = Convert.ToUInt64(value);

            _source &= ~(mask << offset);
            _source |= (_value & mask) << offset;

            return _source;
        }

        public static TValue SetValue<TValue>(TValue source, TValue mask, bool state)
            where TValue : Enum
        {
            long _source = Convert.ToInt64(source);
            long _mask = Convert.ToInt64(mask);

            if (state)
            {
                _source |= _mask;
            }
            else
            {
                _source &= ~_mask;
            }

            return (TValue)Enum.ToObject(typeof(TValue), _source);
        }

        public static ulong GetValue<TSource>(TSource source, ulong mask = 1, int offset = 0)
            where TSource : unmanaged
        {
            ulong _source = Convert.ToUInt64(source);

            return (_source >> offset) & mask;
        }

        public static bool GetState<TSource>(TSource source, ulong mask = 1, int offset = 0)
           where TSource : unmanaged
        {
            ulong _source = Convert.ToUInt64(source);

            mask <<= offset;

            return (_source & mask) == mask;
        }

        public static bool GetState<TSource>(TSource source, TSource state)
            where TSource : unmanaged
        {
            ulong _source = Convert.ToUInt64(source);
            ulong _state = Convert.ToUInt64(state);

            return (_source & _state) == _state;
        }
    }
}

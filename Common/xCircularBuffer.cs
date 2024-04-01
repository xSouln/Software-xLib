using System.Collections.Generic;

namespace xLibV100.Common
{
    public class xCircularBuffer<TElement>
    {
        protected long sizeMask;
        protected long totalIndex;
        protected long handlerIndex;
        protected TElement[] buffer;

        public long SizeMask => sizeMask;
        public long TotalIndex => totalIndex;
        public long HandlerIndex => handlerIndex;
        public TElement[] Buffer => buffer;

        public xCircularBuffer(int sizeInBits)
        {
            long mask = 0xffffffff << sizeInBits;
            sizeMask = (mask ^ 0xffffffff) & 0xffffffff;

            buffer = new TElement[SizeMask + 1];
        }

        public int Add(TElement[] data)
        {
            foreach (var element in data)
            {
                buffer[totalIndex] = element;
                totalIndex++;
                totalIndex &= sizeMask;
            }
            return 0;
        }

        public int Read(TElement[] source, int maxSize)
        {
            int i = 0;
            while (i < maxSize && handlerIndex != totalIndex)
            {
                source[i] = buffer[handlerIndex];
                handlerIndex++;
                handlerIndex &= sizeMask;
            }

            return i;
        }

        public int Read(List<TElement> source, int tier, int maxSize)
        {
            int i = 0;
            while (i < maxSize && handlerIndex != totalIndex)
            {
                source.Add(buffer[handlerIndex]);
                handlerIndex++;
                handlerIndex &= sizeMask;

                if (source.Count > tier)
                {
                    source.RemoveAt(0);
                }
            }

            return i;
        }

        public int FilledSize => (int)((totalIndex - handlerIndex) & sizeMask);
    }
}

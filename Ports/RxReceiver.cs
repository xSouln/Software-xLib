namespace xLibV100.Ports
{
    public class RxReceiver
    {
        public enum Result
        {
            Reset,
            Storage,
        }

        public unsafe delegate void ReceivedPacketHandler(RxReceiver rx, ReceivedPacketHandlerArg arg);

        public event ReceivedPacketHandler PacketReceiver;

        public byte[] EndLine;
        public byte[] Data;
        public int ByteRecived;

        public Result Response = Result.Reset;

        public object Parent;

        public RxReceiver(int BufSize, byte[] EndLine)
        {
            this.EndLine = EndLine;
            Data = new byte[BufSize];
            ByteRecived = 0;

            if (this.EndLine == null)
            {
                this.EndLine = new byte[] { (byte)'\r' };
            }
        }
        private unsafe void BufLoaded()
        {
            ByteRecived = 0;
        }

        private unsafe void EndLineIdentify()
        {
            if (PacketReceiver != null)
            {
                fixed (byte* ptr = Data)
                {
                    PacketReceiver(this,
                        new ReceivedPacketHandlerArg
                        {
                            Data = Data,
                            DataSize = ByteRecived,
                            DataPtr = ptr,
                            PacketSize = ByteRecived - EndLine.Length
                        });
                }
            }
        }

        public void Add(byte[] data, int dataLength, int offset)
        {
            int totalSize = 0;

            while (totalSize < dataLength)
            {
                Data[ByteRecived] = data[totalSize + offset];
                ByteRecived++;

                if (ByteRecived >= EndLine.Length)
                {
                    int i = EndLine.Length;
                    int j = ByteRecived;
                    while (i > 0)
                    {
                        i--;
                        j--;
                        if (EndLine[i] != Data[j])
                        {
                            goto verify_end;
                        }
                    }

                    EndLineIdentify();
                }

            verify_end:
                if (ByteRecived >= Data.Length)
                {
                    BufLoaded();
                }

                totalSize++;
            }
        }

        public void Add(byte data)
        {
            Data[ByteRecived] = data;
            ByteRecived++;

            if (ByteRecived >= EndLine.Length)
            {
                int i = EndLine.Length;
                int j = ByteRecived;
                while (i > 0)
                {
                    i--;
                    j--;
                    if (EndLine[i] != Data[j])
                    {
                        goto verify_end;
                    }
                }
                EndLineIdentify();
            }

        verify_end:
            if (ByteRecived >= Data.Length)
            {
                BufLoaded();
            }
        }

        public void Add(byte[] data)
        {
            int total_size = 0;

            while (total_size < data.Length)
            {
                Data[ByteRecived] = data[total_size];
                ByteRecived++;

                if (ByteRecived >= EndLine.Length)
                {
                    int i = EndLine.Length;
                    int j = ByteRecived;
                    while (i > 0)
                    {
                        i--;
                        j--;
                        if (EndLine[i] != Data[j])
                        {
                            goto verify_end;
                        }
                    }
                    EndLineIdentify();
                }

            verify_end:
                if (ByteRecived >= Data.Length)
                {
                    BufLoaded();
                }

                total_size++;
            }
        }

        public void Clear()
        {
            Response = Result.Reset;
            ByteRecived = 0;
        }
    }
}

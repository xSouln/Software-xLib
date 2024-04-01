using System;
using System.Collections.Generic;

namespace xLibV100.Transceiver
{
    public struct PacketIdentificatorT
    {
        /// <summary>
        /// byte StartKey = '#'
        /// ushort Description = description of the purpose of the package - request, response, event, etc.
        /// byte EndKey = ':'
        /// </summary>
        public uint Value; // format: [#][Description][:]
    }

    public struct PacketHeaderT
    {
        /// <summary>
        /// byte StartKey = '#'
        /// ushort Description = description of the purpose of the package - request, response, event, etc.
        /// byte EndKey = ':'
        /// </summary>
        public uint Identificator; // format: [#][Description][:]

        /// <summary>
        /// unique key of the device, module. 0 - system commands
        /// </summary>
        public uint DeviceId;
        public uint UID;

        public static PacketHeaderT Init(uint identificator, PacketHeaderDescription value, uint deviceId, uint uid)
        {
            return new PacketHeaderT
            {
                Identificator = (identificator & (uint)PacketIdentificator.Mask) | ((uint)value << 8),
                DeviceId = deviceId,
                UID = uid
            };
        }

        public static PacketHeaderT Init(PacketHeaderDescription value, uint deviceId, uint uid)
        {
            return new PacketHeaderT
            {
                Identificator = (uint)PacketIdentificator.Default | ((uint)value << 8),
                DeviceId = deviceId,
                UID = uid
            };
        }

        public static PacketHeaderT Init(PacketHeaderDescription value)
        {
            return new PacketHeaderT
            {
                Identificator = (uint)PacketIdentificator.Default | ((uint)value << 8),
            };
        }

        public int Description
        {
            get => ((int)Identificator >> 8) & 0xffff;
            set
            {
                Identificator &= (uint)PacketIdentificator.Mask;
                Identificator |= ((uint)value << 8) & (uint)PacketIdentificator.Mask;
            }
        }
    }

    public struct PacketInfoT
    {
        /// <summary>
        /// generated key - when receiving a response, must match the request key
        /// </summary>
        public uint RequestId;

        /// <summary>
        /// action(command) key
        /// </summary>
        public ushort Action;

        /// <summary>
        /// size of nested data after packet info
        /// </summary>
        public ushort ContentSize;
    }

    /// <summary>
    /// the general message looks like a sequential sending of structures:
    /// PacketHeaderT
    /// PacketInfoT - PacketInfoT.ContentSize must be equal to the size of the attached content
    /// bytes[] Content
    /// end charecter - "/r"
    /// 
    /// array: [#][Description][:][DeviceKey][RequestId][ActionKey][ContentSize][uint8_t Content[ContentSize]][\r]
    /// </summary>
    public struct PacketT
    {
        public PacketHeaderT Header; //format: [#][Description][:][DeviceKey]
        public PacketInfoT Info; //format: [RequestId][Action][ContentSize]
        //byte Content[Info.ContentSize]
        //byte EndPacketSymbol default('\r')
    }

    public enum PacketIdentificator : uint
    {
        Mask = 0xFF0000FF, //
        Default = 0x3A000023U, //0x2300003A

        DescriptionMask = 0x00FFFF00,
    }

    public enum PacketHeaderDescription : ushort
    {
        Request = 0x5152, //"RQ" 0x5251
        Response = 0x5352, //"RS" 0x5253
        Event = 0x5645, //"EV" 0x4556
        Error = 0x5245 //"ER" 0x4552
    }

    public class PacketBase
    {
        public static unsafe void Add<TData>(List<byte> packet, TData data) where TData : unmanaged
        {
            if (packet != null)
            {
                byte* ptr = (byte*)&data;

                for (int i = 0; i < sizeof(TData); i++)
                {
                    packet.Add(ptr[i]);
                }
            }
        }

        public static unsafe void Add<TData>(List<byte> packet, TData[] data) where TData : unmanaged
        {
            if (packet != null)
            {
                foreach (TData element in data)
                {
                    byte* ptr = (byte*)&element;

                    for (int i = 0; i < sizeof(TData); i++)
                    {
                        packet.Add(ptr[i]);
                    }
                }
            }
        }

        public static void Aggregate<TRequest>(List<byte> packet, TRequest request) where TRequest : IRequestAdapter
        {
            request?.Add(packet);
        }

        public static unsafe void Add(List<byte> packet, void* ptr, int size)
        {
            if (packet != null)
            {
                byte* _ptr = (byte*)ptr;

                while (size > 0)
                {
                    packet.Add(*_ptr);
                    size--;
                }
            }
        }

        public static void Add(List<byte> packet, string data)
        {
            if (packet != null && data != null)
            {
                foreach (byte ch in data)
                {
                    packet.Add(ch);
                }
            }
        }

        public static void Add(List<byte> packet, byte[] data)
        {
            if (packet != null && data != null)
            {
                foreach (byte ch in data)
                {
                    packet.Add(ch);
                }
            }
        }
    }

    /*
    internal class Exapmles
    {
        const uint xDEVICE_UID = 0x5C786D5;
        const ushort ActionGetRootDevice = 103; // получение общей информации об хосте
        uint lastRequestId;

        interface IReceiver
        {
            unsafe int Receive(byte* data, int size);
        }

        struct ResponseGetRootDeviceT : IReceiver
        {
            public uint DeviceId;

            public ushort NumberOfServices;
            public ushort NumberOfDevices;

            public ushort DeviceType;
            public ushort DeviceExtansion;

            public uint Flags;

            public unsafe int Receive(byte* data, int size)
            {
                this = *(ResponseGetRootDeviceT*)data;

                return sizeof(ResponseGetRootDeviceT);
            }
        }

        private static int AddToPacket<T>(List<byte> packet, T data) where T : unmanaged
        {
            if (packet == null)
            {
                return 0;
            }

            unsafe
            {
                byte* ptr = (byte*)&data;

                for(int i = 0; i < sizeof(T); i++)
                {
                    packet.Add(ptr[i]);
                }

                return sizeof(T);
            }
        }

        private static int AddToPacket<T>(List<byte> packet, IList<T> data) where T : unmanaged
        {
            int size = 0;

            if (packet == null || data == null)
            {
                return 0;
            }

            foreach (T element in data)
            {
                size += AddToPacket(packet, element);
            }

            return size;
        }

        private static int AddToPacket(List<byte> packet, string data)
        {
            if (packet == null || data == null)
            {
                return 0;
            }

            foreach (var element in data)
            {
                packet.Add((byte)element);
            }

            return data.Length;
        }

        private unsafe int ReceiveResponse(byte[] data, IReceiver response)
        {
            // minimal size validation
            if (data.Length < sizeof(PacketT) + "/r".Length)
            {
                return 0;
            }

            fixed (byte* ptr = data)
            {
                PacketT* packet = (PacketT*)ptr;

                uint identificator = packet->Header.Identificator & (uint)PacketIdentificator.Mask;

                // packet validation
                if (identificator != (uint)PacketIdentificator.Default)
                {
                    return 0;
                }

                // response validation
                if (packet->Header.Description != (int)PacketHeaderDescription.Response)
                {
                    return 0;
                }

                // uid validation
                if (packet->Header.UID != xDEVICE_UID)
                {
                    return 0;
                }

                // unique id validation
                if (packet->Info.RequestId != lastRequestId)
                {
                    return 0;
                }

                int totalContentSize = data.Length - sizeof(PacketT) - "/r".Length;

                // content size validation
                if (packet->Info.ContentSize < totalContentSize)
                {
                    return 0;
                }

                // 
                return response.Receive(ptr + sizeof(PacketT), totalContentSize);
            }
        }

        private void RequestExapmle()
        {
            PacketHeaderT packetHeader = new PacketHeaderT();
            packetHeader.Identificator = (uint)PacketIdentificator.Default; // it is set for protocol recognition
            packetHeader.Description = (int)PacketHeaderDescription.Request; // description of what the request is
            packetHeader.DeviceId = 0; // the ID of the node in the network, for ethernet connections to work, is set to 0 
            packetHeader.UID = xDEVICE_UID;//The UID of the service that provides access to the interaction functions

            lastRequestId = (uint)new Random().Next();

            PacketInfoT packetInfo = new PacketInfoT();
            packetInfo.Action = ActionGetRootDevice;
            packetInfo.RequestId = lastRequestId;
            packetInfo.ContentSize = 0; // the request does not require the transmitted content

            ///passing structures:
            ///1 - packetHeader
            ///2 - packetInfoT
            ///3 - "/r"

            List<byte> packet = new List<byte>();

            AddToPacket(packet, packetHeader);
            AddToPacket(packet, packetInfo);
            //AddToPacket(packet, content); // add content if needed
            AddToPacket(packet, "/r");

            byte[] data = packet.ToArray(); // data to transfer

            ///the response received:
            ///1 - packetHeader
            ///2 - packetInfoT
            ///3 - ResponseGetRootDeviceT
            ///4 - "/r"
            ///
            byte[] responseData = new byte[data.Length]; // received data

            ResponseGetRootDeviceT responseContent = new ResponseGetRootDeviceT();
            ReceiveResponse(responseData, responseContent);
        }
    }*/
}

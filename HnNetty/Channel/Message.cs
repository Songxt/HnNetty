using HnNetty.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace HnNetty.Channel
{
    public class Message
    {
        public byte Head { get; set; } = 0x68;

        public byte End { get; set; } = 0x16;

        public int Len { get; set; }

        public byte Ctrl { get; set; } = 0x00;

        //public string Lat { get; set; }

        //public string Lon { get; set; }

        public byte[] Content { get; set; }

        //public int Count { get; set; }

        public byte[] ToBytes()
        {
            var list = new List<byte>();
            list.Add(Head);
            list.AddRange(BitConverter.GetBytes(5 + Content.Length).Take(2));

            list.Add(byte.Parse(Sn.Substring(0, 2), NumberStyles.HexNumber));
            list.Add(byte.Parse(Sn.Substring(2, 2), NumberStyles.HexNumber));
            list.Add(byte.Parse(Sn.Substring(4, 2), NumberStyles.HexNumber));
            list.Add(byte.Parse(Sn.Substring(6, 2), NumberStyles.HexNumber));

            list.Add(Ctrl);
            list.AddRange(Content);
            list.Add(CmdHelper.GetCRC(list.Skip(3).ToArray()));
            list.Add(End);
            return list.ToArray();
        }

        public string Packet { get; set; }

        public string Sn { get; set; }

        public Message Prase(byte[] bytes)
        {
            //头验证
            if (bytes[0] != 0x68)
            {
                return null;
            }

            //校验
            var crc = bytes[bytes.Length - 2];
            var crc2 = CmdHelper.GetCRC(bytes.Skip(3).Take(bytes.Length - 5).ToArray());

            if (crc != crc2)
            {
                return null;
            }

            Len = BitConverter.ToInt16(new byte[] { bytes[1], bytes[2] }, 0);
            Sn = BitConverter.ToString(bytes.Skip(3).Take(4).ToArray()).Replace("-", "");
            Packet = BitConverter.ToString(bytes).Replace("-", " ");
            Ctrl = bytes[7];
            //Count = bytes[8];
            Content = bytes.Skip(8).Take(Len - 5).ToArray();
            return this;
        }
    }
}
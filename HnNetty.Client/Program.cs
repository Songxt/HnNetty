using System;
using DotNetty.Buffers;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HnNetty.Client
{
    internal class Program
    {
        private static byte[] num = new byte[]
        {
            0x12,0x13,
            0x21,0x22,0x23,0x24,
            0x31,0x32,0x33,0x34,
            0x41,0x42,0x43,0x44,
            0x51,0x52,0x53,0x54,
            0x61,0x62,0x63,0x64
        };

        public static void Main() => RunClientAsync().Wait();

        private static async Task RunClientAsync()
        {
            var group = new MultithreadEventLoopGroup();

            string targetHost = null;

            try
            {
                for (int i = 0; i < 50000; i++)
                {
                    var bootstrap = new Bootstrap();
                    bootstrap
                        .Group(group)
                        .Channel<TcpSocketChannel>()
                        .Option(ChannelOption.TcpNodelay, true)
                        .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                        {
                            IChannelPipeline pipeline = channel.Pipeline;
                            pipeline.AddLast(new ClientHandler());
                        }));

                    IChannel bootstrapChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("192.168.2.100"), 1098));

                    var buff = new List<byte>();
                    buff.Add(0x68);
                    var len = 6 + 22 * 16;
                    buff.AddRange(BitConverter.GetBytes(len).Take(2).ToArray());

                    buff.Add(0x20);
                    buff.Add(0x20);
                    buff.AddRange(BitConverter.GetBytes(i).Take(2));
                    buff.Add(0x0C);
                    buff.Add(0x16);

                    for (int j = 0; j < num.Length; j++)
                    {
                        buff.Add(0xF1);
                        buff.Add(0x7B);
                        buff.Add(0x2C);
                        buff.Add(0x7B);

                        buff.Add(0x07);
                        buff.Add(0x00);

                        buff.Add(0x73);
                        buff.Add(0x00);


                        buff.Add(num[j]);

                        buff.Add(0x41);

                        var list = new List<byte>();
                        var date = DateTime.Now;
                        list.Add(GetBCD(date.Second));
                        list.Add(GetBCD(date.Minute));
                        list.Add(GetBCD(date.Hour));
                        list.Add(GetBCD(date.Day));
                        list.Add(GetBCD(date.Month));
                        list.Add(GetBCD(date.Year - 2000));

                        buff.AddRange(list);
                    }

                    buff.Add(GetCRC(buff.Skip(3).ToArray()));
                    buff.Add(0x16);

                    IByteBuffer initialMessage = Unpooled.Buffer(buff.Count);
                    initialMessage.WriteBytes(buff.ToArray());
                    // 将缓冲区数据流写入到管道中
                    await bootstrapChannel.WriteAndFlushAsync(initialMessage); // (3)
                    await bootstrapChannel.CloseAsync();
                }
                
              
            }
            finally
            {
                group.ShutdownGracefullyAsync().Wait(1000);
            }
        }

        public static byte GetCRC(byte[] bytes)
        {
            var value = 0;
            for (var i = 0; i < bytes.Length; i++)
                value += bytes[i];
            return BitConverter.GetBytes(value)[0];
        }

        /// <summary>
        ///     获取BCD码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte GetBCD(int value)
        {
            var i = value / 10;
            var j = value % 10;
            return (byte)((i << 4) + j);
        }
    }
}
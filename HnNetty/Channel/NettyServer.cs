using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;
using HnNetty.Tools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Dapper;
using Dapper.Contrib.Extensions;
using HnDb;
using HnDb.Data;
using HnDb.Enum;

namespace HnNetty.Channel
{
    public class NettyServer : SimpleChannelInboundHandler<Message>
    {
        public static volatile ConcurrentDictionary<string, Session> Clients = new ConcurrentDictionary<string, Session>();

        //public override bool IsSharable => true;//标注一个channel handler可以被多个channel安全地共享。
        //public string Sn { get; set; }

        protected override void ChannelRead0(IChannelHandlerContext ctx, Message msg)
        {
            var ip = (ctx.Channel.RemoteAddress as IPEndPoint).Address;
            Clients[ctx.Channel.Id.ToString()].Sn = msg.Sn;
            LogHelper.Info($"{ip} {msg.Sn} R=>{msg.Packet}");
            //foreach (var session in Clients)
            //{
            //    if (session.Value.Sn == "11 22")
            //    {
            //        session.Value.Channel.WriteAsync(new Message());
            //    }
            //}

            //var cmd = CmdFactory.GetClassInstance(msg.Ctrl);
            switch (msg.Ctrl)
            {
                case 0x0c:
                    var tircmd = new TireCmd();
                    tircmd.Execute(ctx.Channel, msg);
                    break;

                case 0x0d:
                    var timecmd = new TimeCmd();
                    timecmd.Execute(ctx.Channel, msg);
                    break;

                default:
                    var trancmd = new TranferCmd();
                    trancmd.Execute(ctx.Channel, msg);
                    break;
            }
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        //捕获 异常，并输出到控制台后断开链接，提示：客户端意外断开链接，也会触发
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            LogHelper.Error(exception);
            context.CloseAsync();

            UpdateStatus(context.Channel);
            Clients.TryRemove(context.Channel.Id.ToString(), out Session temp);
        }

        public override void HandlerAdded(IChannelHandlerContext context)
        {
            var ip = (context.Channel.RemoteAddress as IPEndPoint).Address;
            LogHelper.Info($"Client {ip} Connect");
            base.HandlerAdded(context);

            Clients.AddOrUpdate(context.Channel.Id.ToString(), new Session() { Channel = context.Channel }, (k, v) => v);
        }

        //客户端下线断线时
        public override void HandlerRemoved(IChannelHandlerContext context)
        {
            var ip = (context.Channel.RemoteAddress as IPEndPoint).Address;
            LogHelper.Info($"Client {ip} Disconnect");
            base.HandlerRemoved(context);

            UpdateStatus(context.Channel);
            Clients.TryRemove(context.Channel.Id.ToString(), out Session temp);
        }

        private void UpdateStatus(IChannel channel)
        {
            var client = Clients.FirstOrDefault(p => p.Key == channel.Id.ToString());
            if (client.Value.Sn != null)
            {
                var dtu = DbHelper.Connect().QueryFirstOrDefault<Dtu>("select * from dtu where sn=@sn", new { sn = client.Value.Sn });
                if (dtu != null)
                {
                    dtu.UpdateTime = DateTime.Now;
                    dtu.State = DtuState.Offline;
                    DbHelper.Connect().Update(dtu);
                }
            }
        }

        ////服务器监听到客户端活动时
        //public override void ChannelActive(IChannelHandlerContext context)
        //{
        //    LogHelper.Info($"Client {context.Channel.RemoteAddress} Online.");
        //    base.ChannelActive(context);
        //}

        ////服务器监听到客户端不活动时
        //public override void ChannelInactive(IChannelHandlerContext context)
        //{
        //    LogHelper.Info($"Client {((IPEndPoint)context.Channel.RemoteAddress).Address} Offline.");
        //    base.ChannelInactive(context);
        //}

        public override void UserEventTriggered(IChannelHandlerContext context, object evt)
        {
            if (evt is IdleStateEvent eventState)
            {
                if (eventState.State == IdleState.ReaderIdle)
                {
                    context.CloseAsync();
                    LogHelper.Info($"{context.Channel.RemoteAddress} OutOfTime");
                }
            }
            else
            {
                base.UserEventTriggered(context, evt);
            }
        }

        public static void KickAll()
        {
            foreach (var session in Clients)
            {
                session.Value.Channel.CloseAsync();
            }
        }
    }

    public class Session
    {
        public string Id { get; set; }

        public string Sn { get; set; }

        public IChannel Channel { get; set; }
    }

    #region 加密

    public class ServerEncoder : MessageToByteEncoder<Message>
    {
        protected override void Encode(IChannelHandlerContext context, Message message, IByteBuffer output)
        {
            //byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            //IByteBuffer initialMessage = Unpooled.Buffer(messageBytes.Length);
            //initialMessage.WriteBytes(messageBytes);
            byte[] messageBytes = message.ToBytes();

            LogHelper.Info(BitConverter.ToString(messageBytes).Replace("-", " "));

            output.WriteBytes(messageBytes);
        }
    }

    public class ServerDecoder : ByteToMessageDecoder
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            byte[] array = new byte[input.ReadableBytes];
            input.GetBytes(input.ReaderIndex, array, 0, input.ReadableBytes);
            input.Clear();

            var msg = new Message();
            output.Add(msg.Prase(array));
        }
    }

    #endregion 加密
}
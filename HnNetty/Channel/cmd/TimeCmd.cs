using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using DotNetty.Transport.Channels;
using HnNetty.Tools;

namespace HnNetty.Channel
{
    public class TimeCmd : CommandBase
    {
        public override int Key { get; } = 0x0d;

        public override void Execute(IChannel session, Message package)
        {
            Thread.Sleep(1 * 1000);
            var msg = new Message();
            msg.Ctrl = (byte) (Key + 0x80);
            msg.Sn = package.Sn;
            var list = new List<byte> { 0x01 };
            list.AddRange(CmdHelper.GetBCD_Date(DateTime.Now));
            msg.Content = list.ToArray();

            session.WriteAsync(msg);
        }
    }
}

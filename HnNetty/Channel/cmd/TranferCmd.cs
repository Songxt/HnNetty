using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Transport.Channels;

namespace HnNetty.Channel
{
    public class TranferCmd:CommandBase
    {
        public override int Key { get; } = 0xFF;

        public override void Execute(IChannel session, Message package)
        {
              NettyServer.KickAll();
        }
    }
}

using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using HnNetty.Channel;
using HnNetty.Tools;
using Microsoft.Extensions.Options;
using System;
using Topshelf;

namespace HnNetty
{
    public class HnService : ServiceControl
    {
        private readonly IOptions<AppOptions> _app;

        public HnService(IOptions<AppOptions> app)
        {
            _app = app;
        }

        public bool Start(HostControl hostControl)
        {
            RunServerAsync();
            return true;
        }

        public void RunServerAsync()
        {
            IEventLoopGroup eventLoop;
            eventLoop = new MultithreadEventLoopGroup();
            try
            {
                // 服务器引导程序
                var bootstrap = new ServerBootstrap();
                bootstrap.Group(eventLoop);
                bootstrap.Channel<TcpServerSocketChannel>();
                bootstrap.ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    //解码
                    pipeline.AddLast(new ServerDecoder());
                    //服务端为读IDLE
                    pipeline.AddLast(new IdleStateHandler(60 * 5, 60 * 5, 60 * 5));//第一个参数为读，第二个为写，第三个为读写全部
                    //解码
                    pipeline.AddLast(new ServerEncoder());
                    //添加handler
                    pipeline.AddLast(new NettyServer());
                }));
                foreach (var server in _app.Value.Servers)
                {
                    bootstrap.BindAsync(server.Port);
                    LogHelper.Info("服务器启动：" + server.Name + "端口" + server.Port);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public bool Stop(HostControl hostControl)
        {
            throw new NotImplementedException();
        }
    }
}
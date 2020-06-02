using CSRedis;
using HnNetty.Channel;
using log4net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using HnDb;
using Topshelf;

namespace HnNetty
{
    internal class Program
    {
        private static IConfiguration _appConfiguration;

        private static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                var hostBuilder = new HostBuilder().ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    var hostingEnvironment = hostContext.HostingEnvironment;
                    _appConfiguration = AppConfigurations.Get(hostingEnvironment.ContentRootPath,
                        hostingEnvironment.EnvironmentName);
                }).ConfigureServices((hostContext, services) =>
                {
                    //注入IConfiguration到DI容器
                    services.AddSingleton(_appConfiguration);
                    services.AddOptions();
                    services.Configure<AppOptions>(_appConfiguration.GetSection("App"));
                    //注入MyService到DI容器
                    //services.AddSingleton<IMyService, MyService>();
                    //}).ConfigureLogging((hostContext, logging) =>
                    //{
                    //});

                    CmdFactory.Init();

                    var connstr = _appConfiguration.GetConnectionString("Redis");
                    var csredis = new CSRedisClient(connstr + ",defaultDatabase=0,poolsize=50,ssl=false,writeBuffer=10240");
                    RedisHelper.Initialization(csredis);
                    DbHelper.Initialization(_appConfiguration.GetConnectionString("DefaultConnectionString"),DbHelper.DbType.NPGSQL);
                });

                var repository = LogManager.CreateRepository("loghelper");
                var config = new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory + "hnlog.config");
                log4net.Config.XmlConfigurator.Configure(repository, config);

                //初始化通用主机
                var host = hostBuilder.Build();
                var appOptions = host.Services.GetService<IOptions<AppOptions>>();

                //注册为服务
                x.Service<HnService>(sc =>
                {
                    sc.ConstructUsing(name => new HnService(appOptions));
                    sc.WhenStarted((s, hostControl) => s.Start(hostControl));
                    sc.WhenStopped((s, hostControl) => s.Stop(hostControl));
                });

                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.SetDescription(appOptions.Value.Description);
                x.SetDisplayName(appOptions.Value.Name);
                x.SetServiceName(appOptions.Value.Name);
            });
        }
    }
}
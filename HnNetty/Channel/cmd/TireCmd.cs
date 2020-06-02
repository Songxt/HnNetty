using Dapper;
using DotNetty.Transport.Channels;
using HnDb;
using HnDb.Data;
using HnDb.Enum;
using HnNetty.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dapper.Contrib.Extensions;

namespace HnNetty.Channel
{
    public class TireCmd : CommandBase
    {
        public override int Key { get; } = 0x0c;

        public override void Execute(IChannel session, Message package)
        {
            var dtu = DbHelper.Connect().QueryFirstOrDefault<Dtu>("select * from s_dtu where sn=@sn", new { sn = package.Sn });
            if (dtu == null)
            {
                dtu = new Dtu() {Sn = package.Sn};
                DbHelper.Connect().Insert<Dtu>(dtu);
            }
            else
            {
                dtu.UpdateTime = DateTime.Now;
                dtu.State = DtuState.Online;
                DbHelper.Connect().Update(dtu);
            }

            var sensors = new List<Sensor>();
            var count = package.Content[0];

            var msg = new Message();
            List<byte> list;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    var sensor = new Sensor();
                    sensor.Sn = BitConverter.ToString(package.Content.Skip(i * 16 + 1).Take(4).ToArray()).Replace("-", "");
                    sensor.Temperature = (sbyte)package.Content[i * 16 + 5];
                    sensor.Pressure = ((decimal)BitConverter.ToInt16(package.Content.Skip(i * 16 + 7).Take(2).ToArray(), 0)) / 100;
                    sensor.Num = package.Content[i * 16 + 9].ToString("X");
                    sensor.State = package.Content[i * 16 + 10];
                    sensor.UpdateTime = CmdHelper.GetBCD_Date(package.Content.Skip(i * 16 + 11).Take(6).ToArray());
                    sensor.Dtu = package.Sn;
                    sensors.Add(sensor);
                }
            }
            //解析异常 回复否认帧
            catch (Exception e)
            {
                LogHelper.Error(e);
                Thread.Sleep(1 * 1000);
                msg.Ctrl = 0x0c + 0x80;
                msg.Sn = package.Sn;
                list = new List<byte> { 0x02 };
                list.AddRange(CmdHelper.GetBCD_Date(DateTime.Now));
                msg.Content = list.ToArray();
                session.WriteAsync(msg);
                return;
            }

            foreach (var sensor in sensors)
            {
                RedisHelper.Set(package.Sn + "_" + sensor.Num, sensor);
                DbHelper.Connect().Insert(sensor);
            }

            Thread.Sleep(1 * 1000);
            msg.Ctrl = 0x0c + 0x80;
            msg.Sn = package.Sn;
            list = new List<byte> { 0x01 };
            list.AddRange(CmdHelper.GetBCD_Date(DateTime.Now));
            msg.Content = list.ToArray();

            session.WriteAsync(msg);
        }
    }
}
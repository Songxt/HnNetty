using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using HnDb.Data;

namespace HnDb.DAL
{
    public class SensorDAL
    {
        public static List<Sensor> Get(string sn, DateTime start, DateTime end)
        {
            string sql = "select * from public.sensor where dtu=@sn and updatetime between @start and @end order by updatetime desc";
            return DbHelper.Connect().Query<Sensor>(sql, new {sn, start, end}).ToList();
        }

        public static List<Sensor> Get(string sn, DateTime start, DateTime end,string num)
        {
            string sql = "select * from sensor where parent=@sn and num=@num and updatetime between @start and @end order by updatetime desc";
            return DbHelper.Connect().Query<Sensor>(sql, new { sn, start, end ,num}).ToList();
        }
    }
}

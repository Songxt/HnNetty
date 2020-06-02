using System.Collections.Generic;
using System.Linq;
using Dapper;
using HnDb.Data;

namespace HnDb.DAL
{
    public class DtuDAL
    {
        public static List<Dtu> GetList()
        {
            var sql = "select top 20 * from dtu  order by sn ";
            return DbHelper.Connect().Query<Dtu>(sql).ToList();
        }
    }
}

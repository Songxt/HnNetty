using System;
using Dapper.Contrib.Extensions;
using HnDb.Enum;

namespace HnDb.Data
{
    [Table("s_dtu")]
    public class Dtu
    {
        [ExplicitKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Sn { get; set; }

        public string Lat { get; set; }

        public string Lon { get; set; }

        public DateTime UpdateTime { get; set; } =DateTime.Now;

        public DtuState State { get; set; }

        public int Count { get; set; }

        public string Sensors { get; set; }
    }
}
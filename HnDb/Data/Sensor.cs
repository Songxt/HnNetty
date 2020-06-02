using Dapper.Contrib.Extensions;
using System;

namespace HnDb.Data
{
    [Table("s_sensor")]
    public class Sensor
    {
        [ExplicitKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Sn { get; set; }

        public string Num { get; set; }

        public string Dtu { get; set; }

        public int State { get; set; }

        public decimal Temperature { get; set; }

        public decimal Pressure { get; set; }

        public string Lat { get; set; }

        public string Lon { get; set; }

        public DateTime UpdateTime { get; set; }

        public DateTime RecordTime { get; set; } = DateTime.Now;
    }
}
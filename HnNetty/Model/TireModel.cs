using System;
using System.Collections.Generic;

namespace HnNetty.Model
{
    public class TireData
    {
        public string Sn { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime RecordTime { get; set; } = DateTime.Now;
        public List<TireSensor> Tire { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
    }

    public class TireSensor
    {
        public string Num { get; set; }
        public string Sn { get; set; }
        public string Temperature { get; set; }
        public string Pressure { get; set; }
        public string Status { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace HnNetty
{
    public class AppOptions
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public List<Server> Servers { get; set; }

        public class Server
        {
            public int Port { get; set; }

            public int Heart { get; set; }

            public string Ip { get; set; }

            public string Name { get; set; }
        }
    }
}
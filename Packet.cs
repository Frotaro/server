using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Packet
    {
        private string uuid;
        private string message;

        public string Uuid { get => uuid; set => uuid = value; }
        public string Message { get => message; set => message = value; }
    }
}

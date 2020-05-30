using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Server
{
    public class GameServer
    {
        private ServerContext Context;
        private const int ListeningPort = 55555;
        UdpClient listener;

        public GameServer()
        {
            Context = new ServerContext();
            listener = new UdpClient();
        }
    }
}

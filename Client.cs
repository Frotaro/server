using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Client
    {
        private int userId;
        private string pseudo;
        private string ipAddress;
        private int port;

        public Client(int pId, string pPseudo, string pAddress, int pPort)
        {
            this.userId = pId;
            this.pseudo = pPseudo;
            this.ipAddress = pAddress;
            this.port = pPort;
        }
    }
}

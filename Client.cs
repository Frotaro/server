using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Client
    {
        private string userId;
        private string uuid;
        private string pseudo;
        private string ipAddress;
        private int port;
        private int responseDelay;

        public Client(string pId, string pUuid, string pPseudo, string pAddress, int pPort)
        {
            this.userId = pId;
            this.uuid = pUuid;
            this.pseudo = pPseudo;
            this.ipAddress = pAddress;
            this.port = pPort;
            this.responseDelay = 0;
        }

        public string GetID() { return this.userId; }
        public string GetUUID() { return this.uuid; }
        public string GetPseudo() { return this.pseudo; }
        public string GetIP() { return this.ipAddress; }
        public int GetPort() { return this.port; }
        public int ResponseDelay { get => responseDelay; set => responseDelay = value; }
    }
}

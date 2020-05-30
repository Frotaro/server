using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class ServerContext
    {
        private List<Client> lstClients;
        private const int MaxClients = 10;
        private Database database;

        private List<string> lstLogs;

        public List<Client> LstClients { get => lstClients; set => lstClients = value; }

        public ServerContext(Database pDb)
        {
            this.database = pDb;
            this.lstClients = new List<Client>();
            this.lstLogs = new List<string>();
        }

        public void AddClient(Client pClient)
        {
            this.lstClients.Add(pClient);
        }

        public void AddClient(string pPseudo, string pUUID, IPEndPoint pEP)
        {
            List<string> userData = this.database.SelectData("SELECT * FROM client WHERE pseudo = '" + pPseudo + "'", 3)[1];

            Client c = new Client(userData[0], pUUID, userData[2], pEP.Address.ToString(), pEP.Port);
            this.lstClients.Add(c);

            if(c.GetIP() != pEP.Address.ToString())
            {
                Log(DateTime.Now.ToString() + " : " + pPseudo + " connected from " + pEP.Address.ToString() + " whereas it used to connect from " + userData[1]);
                this.database.ExecuteSql("UPDATE client SET ipv4Address = '" + pEP.Address.ToString() + "' WHERE id = '" + c.GetID() + "'");
            }
        }

        public void RemoveClient(Client pClient)
        {
            this.lstClients.Remove(pClient);
        }

        public void RemoveClient(string pAddress)
        {
            foreach (Client c in this.lstClients)
            {
                if (c.GetIP() == pAddress)
                {
                    this.lstClients.Remove(c);
                    return;
                }
            }
        }

        public void Register(string pPseudo, IPEndPoint pEP)
        {
            string id = this.database.GetNextID("client");
            string req = "INSERT INTO client VALUES ('" + id + "','" + pEP.Address.ToString() + "', '" + pPseudo + "')";
            this.database.ExecuteSql(req);
        }

        public string GetPseudo(string pIP)
        {
            foreach (Client c in this.lstClients)
                if (c.GetIP() == pIP)
                    return c.GetPseudo();

            return "";
        }

        public bool UserConnected(string pIP)
        {
            foreach(Client c in this.lstClients)
                if (c.GetIP() == pIP)
                    return true;

            return false;
        }

        public bool UserExists(string pPseudo)
        {
            Dictionary<int, List<string>> userData = this.database.SelectData("SELECT * FROM client WHERE pseudo = '" + pPseudo + "'", 3);
            if (userData.Count > 0)
                return true;

            return false;
        }

        public void Log(string pMessage)
        {
            string path = "E:/dev/Server/Server/log.txt";

            StreamReader reader = new StreamReader(path);

            List<string> lines = new List<string>();

            string ln;
            while ((ln = reader.ReadLine()) != null)
            {
                lines.Add(ln);
            }
            reader.Close();

            StreamWriter writer = new StreamWriter(path);

            try
            {
                writer.Write(pMessage);
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }

            writer.Close();
        }

        public void WriteLine(string pText)
        {
            this.lstLogs.Add(pText);

            DisplayTerminal();
            foreach(string str in this.lstLogs)
            {
                Console.WriteLine(str);
            }
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();

            throw new Exception("No network adapters with an IPv4 address in the system!");
        }


        public void DisplayTerminal()
        {
            Console.Clear();
            //Infos
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Server launched (" + ServerContext.GetLocalIPAddress() + ")");

            //Clients list
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Clients connected : (" + this.lstClients.Count + "/" + ServerContext.MaxClients + ")");
            int i = 1;
            foreach(Client c in this.lstClients)
            {
                Console.WriteLine(i.ToString() + " > " + c.GetPseudo() + "(" + c.GetIP() + ")");
                i++;
            }

            //Logs
            Console.SetCursorPosition(0, 26);
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine(new string(' ', 55));
            Console.SetCursorPosition(0, 26);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("LOG");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}

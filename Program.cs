using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(55, 35);
            Database db = new Database();
            GameServer gameServer = new GameServer(db);
            gameServer.Start();
        }
    }
}

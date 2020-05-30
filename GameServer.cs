using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class GameServer
    {
        private ServerContext Context;
        private const int ListeningPort = 55555;

        private const int MaxDelay = 10000;

        private DateTime StartTime;
        private UdpClient listener;

        private double IntervalHandShake;

        public GameServer(Database pDatabase)
        {
            Context = new ServerContext(pDatabase);
            listener = new UdpClient(ListeningPort);
            IntervalHandShake = 0;
            Context.DisplayTerminal();
        }
        
        public void Start()
        {
            bool running = true;
            StartTime = DateTime.Now;

            DateTime oldTimeRef;
            DateTime newTimeRef = DateTime.Now;

            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, ListeningPort);

            while (running)
            {
                if(listener.Available > 0)
                {
                    byte[] Buffer = null;

                    try
                    {
                        Buffer = listener.Receive(ref groupEP);

                        string data = Encoding.ASCII.GetString(Buffer, 0, Buffer.Length);
                        
                        Packet packet = JsonSerializer.Deserialize<Packet>(data);

                        //Set client response delay at 0
                        foreach (Client c in Context.LstClients)
                            if (c.GetUUID() == packet.Uuid)
                                c.ResponseDelay = 0;

                        int length = packet.Message.IndexOf(' ');
                        if (length < 0)
                            length = packet.Message.Length;

                        string keyword = packet.Message.Substring(0, length);
                        string pseudo;

                        switch (keyword)
                        {
                            case "register":
                                pseudo = packet.Message.Substring(length + 1);
                                if (!Context.UserConnected(groupEP.Address.ToString()))
                                {
                                    if (Context.UserExists(pseudo))
                                    {
                                        SendMessage("NO", "Unavailable username", groupEP);
                                        break;
                                    }

                                    Context.Register(pseudo, groupEP);
                                    SendMessage("YES", "Registered successfully !", groupEP);
                                    Context.WriteLine(pseudo + " just registered from " + groupEP.Address.ToString());
                                    break;
                                }

                                SendMessage("NO", "Already logged in !", groupEP);
                                break;
                            case "login":
                                pseudo = packet.Message.Substring(length + 1);
                                if (!Context.UserConnected(groupEP.Address.ToString()))
                                {
                                    if (Context.UserExists(pseudo))
                                    {
                                        Context.AddClient(pseudo, packet.Uuid, groupEP);
                                        SendMessage("YES", "Connected succesfully !", groupEP);
                                        Context.DisplayTerminal();
                                        Context.WriteLine(pseudo + " (" + groupEP.Address.ToString() + ") just logged in");
                                        break;
                                    }
                                    SendMessage("NO", "Wrong username !", groupEP);
                                    break;
                                }
                                SendMessage("NO", "Already logged in !", groupEP);
                                break;

                            case "exit":
                                string user = Context.GetPseudo(groupEP.Address.ToString());
                                Context.RemoveClient(groupEP.Address.ToString());
                                Context.WriteLine(user + " just left");
                                break;

                        }
                    }
                    catch(SocketException e)
                    {
                        Context.Log(e.ToString());
                    }
                    catch(Exception e)
                    {
                        Context.Log(e.ToString());
                    }
                }

                //Using for timers
                oldTimeRef = newTimeRef;
                newTimeRef = DateTime.Now;

                double Delta = newTimeRef.Subtract(oldTimeRef).TotalMilliseconds;

                IntervalHandShake += Delta;

                if(IntervalHandShake >= 500)
                {
                    IntervalHandShake = 0;
                    for (int i = Context.LstClients.Count - 1; i >= 0; i--)
                    {
                        Client c = Context.LstClients[i];
                        c.ResponseDelay += 500;
                        if(c.ResponseDelay >= GameServer.MaxDelay)
                        {
                            IPEndPoint cEP = new IPEndPoint(IPAddress.Parse(c.GetIP()), c.GetPort());
                            SendMessage("GETOUT", "Disconnected for inactivity", cEP);
                            Context.WriteLine(c.GetPseudo() + " disconnected for inactivity");
                            Context.RemoveClient(c);
                        }
                    }
                }
                
            }
        }

        public void SendMessage(string pAction, string pMessage, IPEndPoint pEP)
        {
            byte[] Buffer = Encoding.ASCII.GetBytes(pAction + ":" + pMessage);
            listener.Send(Buffer, Buffer.Length, pEP);
        }
    }
}

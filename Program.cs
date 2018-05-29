using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;

namespace Server
{

    class Program
    {
        static ClsGame Game = new ClsGame();
        static int key = 0;
        static void Main(string[] args)
        {
            int port = 8080;
            Socket ServerListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, port);
            ServerListener.Bind(ep);
            ServerListener.Listen(100);
            Console.WriteLine("Listening...");
            Socket ClientSocket = default(Socket);
            List<Socket> clients= new List<Socket>();
            int counter = 0;
            Program p = new Program();
            while (counter<2)
            {
                ClientSocket = ServerListener.Accept();
                clients.Add(ClientSocket);
                //Game.addPlayer();
                Console.WriteLine(counter + " Client connected");
                counter++;
                
            }
            for (int i=0; i < clients.Count; i++) Game.addPlayer();
            while (true)
            {
                Game.deal();
                Game.playing = Game.rank1;
                Game.Status = 1;
                foreach (Socket client in clients)
                {
                    Thread UserThread = new Thread(() => User(client, clients.IndexOf(client)));
                    UserThread.Start();
                }
                while (Game.Status != -1) {}
            }
            
        }
        static public void User(Socket client, int ID)
        {
            while (Game.Status != -1)
            {
                while (Game.Status == 1)
                {
                    key++;
                    while (key < Game.arrPlayers.Count) { }
                    
                    Game.Status = 0;
                    string msg = Game.arrPlayers[ID].getnumOfCard().ToString() + " ";
                    foreach (ClsCard i in Game.arrPlayers[ID].getarrCards())
                    {
                        msg = msg + i.value.ToString() + "," + i.character.ToString() + " ";
                    }
                    msg = msg + Game.JustPlayCard.Count.ToString() + " ";
                    foreach (ClsCard i in Game.JustPlayCard)
                    {
                        msg = msg + i.value.ToString() + "," + i.character.ToString() + " ";
                    }
                    if (ID == Game.playing)
                    {
                        bool k = true;
                        client.Send(Encoding.ASCII.GetBytes(msg + "1"), 0, (msg + "1").Length, SocketFlags.None);
                        byte[] rev = new byte[1024];
                        int size = client.Receive(rev);
                        string s = Encoding.ASCII.GetString(rev, 0, size);
                        string[] str = s.Split(' ');
                        if (str[0] == "1")
                        {
                            List<ClsCard> SelCards = new List<ClsCard>();
                            foreach (string word in str)
                            {
                                if (word.IndexOf(",") != -1)
                                    SelCards.Add(new ClsCard(int.Parse(word.Split(',')[0]), int.Parse(word.Split(',')[1])));
                            }
                            k = Game.play(Game.arrPlayers[ID], SelCards);
                        }
                        else Game.ignore(Game.arrPlayers[ID]);
                        if (k) Game.nextplayer();
                        if (Game.CountOf0 == Game.arrPlayers.Count - 1) Game.Status = -1;
                        else Game.Status = 1;
                        key = 0;
                    }
                    else client.Send(Encoding.ASCII.GetBytes(msg + "0"), 0, (msg + "0").Length, SocketFlags.None);
                }

            }
        }
    }
   
}

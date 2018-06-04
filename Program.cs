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
    class BoardGame
    {
        public ClsGame Game = new ClsGame();
        public int bet;
        private int key = 0;
        private List<Socket> ListClient = new List<Socket>();
        private int numofReady = 0;
        private bool[] kReady = new bool[4];
        public void sendStatus()
        {
            int n = ListClient.Count;
            for (int i = 0; i < n; i++)
            {
                string s = "Status " + n.ToString();
                for (int j = i; j < i + n; j++)
                {
                    if (kReady[j % n]) s = s + " 1";
                    else s = s + " 0";
                }
                ListClient[i].Send(Encoding.ASCII.GetBytes(s), 0, (s).Length, SocketFlags.None);
            }
        }
        public void addClient(Socket client)
        {
            ListClient.Add(client);
            //string msg = "ID " + ListClient.IndexOf(client);
            //client.Send(Encoding.ASCII.GetBytes(msg), 0, (msg).Length, SocketFlags.None);
        }
        public void process()
        {
            while (true)
            {
                numofReady = 0;
                kReady = new bool[4];
                int d = 0;                
                while (numofReady < ListClient.Count || ListClient.Count == 1) 
                {
                    while (true)
                    {
                        if (d >= ListClient.Count) break;
                        Thread.Sleep(1000);
                        Socket xclient = ListClient[d];
                        Thread thread = new Thread(() => Ready(xclient));
                        thread.Start();
                        d++;
                    }
                }
                Game.reset();
                for (int i = 0; i < ListClient.Count; i++)
                {
                    Game.addPlayer();
                    Game.arrPlayers[i].setRank(ListClient.Count);
                }
                Game.deal();
                Game.playing = Game.rank1;
                Game.Status = 1;
                foreach (Socket client in ListClient)
                {
                    Thread UserThread = new Thread(() => User(client, ListClient.IndexOf(client)));
                    UserThread.Start();
                }
                while (Game.Status != -1) { }
                foreach (Socket client in ListClient)
                {
                    string str = "Rank: " + Game.arrPlayers[ListClient.IndexOf(client)].getRank().ToString();
                    client.Send(Encoding.ASCII.GetBytes(str), 0, (str).Length, SocketFlags.None);
                }
                //Thread.Sleep(10000);
            }
        }
        public void Ready(Socket client)
        {
            byte[] rev = new byte[1024];
            int size = client.Receive(rev);
            string s = Encoding.ASCII.GetString(rev, 0, size);
            if (s == "1")
            {
                numofReady++;
                kReady[ListClient.IndexOf(client)] = true;
                sendStatus();
            }
        }
        public void User(Socket client, int ID)
        {
            while (Game.Status != -1)
            {
                //Console.WriteLine(Game.Status);
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
                    string sAct = "";
                    for (int i = ID; i < ID + Game.arrPlayers.Count; i++)
                    {
                        if (Game.arrPlayers[i % Game.arrPlayers.Count].getAct()) sAct = sAct + " 1";
                        else if (Game.arrPlayers[i % Game.arrPlayers.Count].getnumOfCard() == 0) sAct = sAct + " 1"; 
                        else sAct = sAct + " 0";
                    }
                    //Console.WriteLine((Game.playing - ID) % Game.arrPlayers.Count);
                    
                    if (ID == Game.playing)
                    {
                        bool k = true;
                        client.Send(Encoding.ASCII.GetBytes(msg + "0" + sAct), 0, (msg + "0" + sAct).Length, SocketFlags.None);
                        byte[] rev = new byte[1024];
                        int size = client.Receive(rev);
                        string s = Encoding.ASCII.GetString(rev, 0, size);
                        Console.WriteLine(s);
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
                    else client.Send(Encoding.ASCII.GetBytes((msg + ((Game.playing - ID + Game.arrPlayers.Count) % Game.arrPlayers.Count).ToString()) + sAct), 0, (msg + sAct).Length + 1, SocketFlags.None);
                    //Thread.Sleep(500);
                    
                }

            }
        }
    }
    class Program
    {
        static List<BoardGame> x = new List<BoardGame>();
        static int key = 0;
        static void Main(string[] args)
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            // Get the IP  
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();  
            int port = 8080;
            Socket ServerListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(myIP), port);
            Console.WriteLine(myIP + ":" + port);
            ServerListener.Bind(ep);
            ServerListener.Listen(100);
            Console.WriteLine("Listening...");
            Socket ClientSocket = default(Socket);
            //List<Socket> clients= new List<Socket>();
            int counter = 0;
            //BoardGame x = new BoardGame();
            //Program p = new Program();
            while (true) //chinh so nguoi choi
            {
                ClientSocket = ServerListener.Accept();
                Thread thread = new Thread(() => room(ClientSocket));
                thread.Start();
                //x.addClient(ClientSocket);
                //Game.addPlayer();
                Console.WriteLine(counter + " Client connected");
                counter++;   
            }
            //x.process();
        }
        public static void room(Socket client)
        {
            while (true)
            {
                byte[] rev = new byte[1024];
                int size = client.Receive(rev);
                string s = Encoding.ASCII.GetString(rev, 0, size);
                if (s.Split(' ')[0] == "1")
                {
                    x.Add(new BoardGame());
                    BoardGame y = x[x.Count - 1];
                    y.bet = int.Parse(s.Split(' ')[1]);
                    y.addClient(client);
                    y.sendStatus();
                    y.process();
                    return;
                }
                else
                {
                    foreach (BoardGame y in x)
                    {
                        if (y.bet == int.Parse(s.Split(' ')[1]))
                        {
                            client.Send(Encoding.ASCII.GetBytes("1"), 0, "1".Length, SocketFlags.None);
                            y.addClient(client);
                            y.sendStatus();
                            return;
                        }
                    }
                    client.Send(Encoding.ASCII.GetBytes("0"), 0, "0".Length, SocketFlags.None);
                }
            }
            
        }
    }
   
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ClsGame
    {
        public int Status = -1;
        private int numOfPlayers = 0;
        public List<ClsHandCard> arrPlayers = new List<ClsHandCard>();
        public int JustPlayer = -1;
        public int playing = 0;
        public List<ClsCard> JustPlayCard = new List<ClsCard>();
        public int rank1 = 0;
        public ClsRules rule;
        public int CountOf0 = 0;
        public void deal()
        {
            List<int> T= new List<int>();
            for (int i = 0; i < 52; i++) T.Add(i);
            Random rand = new Random();
            for (int i = 0; i < 13 * numOfPlayers; i++)
            {
                int j = rand.Next(T.Count);
                ClsCard Card = new ClsCard(T[j]/4,T[j]%4);
                arrPlayers[i % numOfPlayers].getarrCards().Add(Card);
                T.RemoveAt(j);
            }
            for (int i = 0; i < numOfPlayers; i++)
            {
                ClsRules.sort(arrPlayers[i].getarrCards());
                arrPlayers[i].setnumOfCard(13);
            }

        }
        public void nextplayer()
        {
            int i = (playing + 1) % numOfPlayers;
            while (!arrPlayers[i].getAct())
            {
                i = (i + 1) % numOfPlayers;
            }
            playing = i;
            if (JustPlayer == playing)
            {
                JustPlayer = -1;
                JustPlayCard = new List<ClsCard>();
                for (int j = 0; j < numOfPlayers; j++) arrPlayers[j].setAct(true);
            }
        }
        public bool play(ClsHandCard Player, List<ClsCard> SelectedCard)
        {
            if (ClsRules.IsWin(JustPlayCard, SelectedCard) || (JustPlayer == -1 && ClsRules.isTrue(SelectedCard)))
            {
                foreach (ClsCard i in SelectedCard)
                {
                    Player.getarrCards().RemoveAll(a => a.value == i.value && a.character == i.character);
                }
                Player.setnumOfCard(Player.getnumOfCard() - SelectedCard.Count);
                if (Player.getnumOfCard() == 0)
                {
                    CountOf0++;
                    Player.setRank(CountOf0);
                }
                if (Player.getnumOfCard() == 0 && CountOf0 == 1) rank1 = playing;
                JustPlayer = playing;
                JustPlayCard = SelectedCard;
                return true;
            }
            return false;
        }
        public void ignore(ClsHandCard Player)
        {
            Player.setAct(false);
        }
        public void addPlayer()
        {
            ClsHandCard a = new ClsHandCard();
            arrPlayers.Add(a);
            numOfPlayers++;

        }
        public void reset()
        {
            numOfPlayers = 0;
            CountOf0 = 0;
            arrPlayers = new List<ClsHandCard>();
            JustPlayCard = new List<ClsCard>();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ClsHandCard
    {
        private int numOfCard = 0;
        private bool Activated = true;
        private int Rank = 0;
        private List<ClsCard> arrCards = new List<ClsCard>();
        public bool getAct()
        {
            return Activated;
        }
        public void setAct(bool x)
        {
            Activated = x;
        }
        public int getnumOfCard()
        {
            return numOfCard;
        }
        public void setnumOfCard(int x)
        {
            numOfCard = x;
        }
        public List<ClsCard> getarrCards()
        {
            return arrCards;
        }
    }
}

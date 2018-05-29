using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ClsCard
    {
        public int value { get; set; }
        public int character { get; set; }
        public ClsCard(int x1, int x2) { value = x1; character = x2; }
        public int getvalue()
        {
            return value;
        }
        public int getcharacter()
        {
            return character;
        }
        public void setCard(ClsCard Card)
        {
            value = Card.value;
            character = Card.character;
        }
        public static bool operator <(ClsCard Card1, ClsCard Card2)
        {
            if (Card1.value < Card2.value || (Card1.value == Card2.value && Card1.character < Card2.character)) return true;
            return false;
        }
        public static bool operator >(ClsCard Card1, ClsCard Card2)
        {
            if (Card1.value > Card2.value || (Card1.value == Card2.value && Card1.character > Card2.character)) return true;
            return false;
        }
    }
}

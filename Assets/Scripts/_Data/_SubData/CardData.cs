using System.Collections;
using System.Collections.Generic;

namespace _Data._SubData
{
    public class CardData
    {
        public int index;
        public bool isChild;
        public string info;
        public List<CardChoice> choices;
    }

    public class CardChoice
    {
        public string info;
        public int effectTech;
        public int effectArmy;
        public int effectSupport;
        public int effectMoney;
        public int child;
    }
}

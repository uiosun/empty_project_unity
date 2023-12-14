using System;
using System.Collections.Generic;
using _Data._SubData;

namespace _Data
{
    [Serializable]
    public class DictData
    {
        public int ver;
        public int maxEpoch;
        public List<List<int>> data;
        public List<List<CardData>> Cards;
        public int techInit;
        public int techMax;
        public int armyInit;
        public int armyMax;
        public int supportInit;
        public int supportMax;
        public int moneyInit;
        public int moneyMax;
    }
}

[System.Serializable]
public class DataContainer
{
    public List<List<int>> data;
}
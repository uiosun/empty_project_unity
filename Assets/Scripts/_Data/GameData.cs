using System;
using System.Collections.Generic;
using _Data;
using UnityEngine;

namespace _Data
{
    public enum UserMapType
    {
        Home = 0,
        Adventure1 = 1,
    }

    [Serializable]
    public class GameData
    {
        public int ver;
        private int _tech;
        public int techMax;
        public bool dead;

        // 统计
        public Dictionary<string, int> statistic;
    
        public int Tech
        {
            get => _tech;
            set
            {
                _tech = value;
                if (_tech < 0)
                {
                    _tech = 0;
                }
                else if (_tech >= techMax)
                {
                    _tech = 0;
                }
            }
        }

        private ConfigData _config; // private will no save

        // 初始化
        public GameData Init(ConfigData config, bool isDebug)
        {
            _config = config;
            ver = _config.ver;
            _tech = _config.techInit;
            techMax = _config.techMax;
            dead = false;

            statistic = new Dictionary<string, int>
            {
                { "无", 0 },
                { "游戏次数", 0 },
                { "广告支持", 0 },
            };

            return this;
        }

        public void Load()
        {
            _config = GameManager.instance.config;
        }
    }
}
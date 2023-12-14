using System;
using System.Collections.Generic;
using _Data;
using _Data._SubData;
using Newtonsoft.Json;

namespace _Data
{
    [Serializable]
    public class ConfigData
    {
        public int ver;
        public int techInit;
        public int techMax;

        public ConfigData Init()
        {
            var jsonFile = ReadJsonFile();
            ver = jsonFile.ver;
        
            return this;
        }

        public DictData ReadJsonFile()
        {
            DictData configInJson = JsonConvert.DeserializeObject<DictData>(GameManager.instance.jsonFile.text);

            return configInJson;
        }
    }
}
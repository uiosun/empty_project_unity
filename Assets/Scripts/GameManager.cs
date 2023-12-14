using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using _Data;
using _Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [NonSerialized]public static GameManager instance;
    
    // 外部资源
    public TextAsset jsonFile;
    
    // 结构数据
    // [FormerlySerializedAs("dict")] public ConstDictData constDict;
    public GameData game;
    public ConfigData config;
    
    // 事件
    [NonSerialized] public UnityEvent turnEvent = new UnityEvent(); // 回合更新
    [NonSerialized] public UnityEvent fightRefreshEvent = new UnityEvent(); // 战斗时更新
    [NonSerialized] public UnityEvent changeMapEvent = new UnityEvent(); // 更换地图
    
    // 手选配置
    public bool gamePause;
    
    private float _configSavingTime = 0;
    private bool _inSave = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        InitConfig();
        InitGame();
        
        DontDestroyOnLoad(instance);
        
        Debug.Log("GameManager 创建完毕");
    }

    void Update()
    {
        if (_configSavingTime > 0)
        {
            _configSavingTime -= Time.deltaTime;
            if (_configSavingTime <= 0) SaveConfig(true);
        }
    }

    public void InitConfig()
    {
        config = new ConfigData().Init();
    }

    public void InitGame()
    {
        game = new GameData().Init(config, UIManager.instance.isDebug);
        gamePause = false;
    }

    public void LoadConfig()
    {
        BinaryFormatter formatter = new BinaryFormatter();
    
        string filePath = Application.persistentDataPath + "/config.data";
        Debug.Log($"file path: {filePath}");
        if (File.Exists(filePath))
        {
            FileStream file = File.Open(filePath, FileMode.Open);
            config = JsonConvert.DeserializeObject<ConfigData>((string)formatter.Deserialize(file));
            file.Close();
        }
    
        if (config == null)
        {
            InitConfig();
        }
    }
    
    public bool LoadGame()
    {
        BinaryFormatter formatter = new BinaryFormatter();
    
        string filePath = Application.persistentDataPath + "/game.data";
        if (File.Exists(filePath))
        {
            FileStream file = File.Open(filePath, FileMode.Open);
            game = JsonConvert.DeserializeObject<GameData>((string)formatter.Deserialize(file));
            file.Close();
            if (game == null)
            {
                UIManager.instance.AlertNew("存档爆炸", "存档结构无法解析，可能存档坏了，您有云存档吗？如果有的话请提供给Q群群主试试");
                return false;
            }
            
            game.Load();
            return true;
        }
        
        return false;
    }
    
    public void SaveConfig(bool isSave = false)
    {
        // 延迟且唯一操作
        if (!isSave)
        {
            _configSavingTime = 2;
            return;
        }
    
        Debug.Log("保存配置");
    
        if (_inSave) return;
        _inSave = true;
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/config.data");
        var jsonData = JsonConvert.SerializeObject(config, Formatting.Indented)
            .Replace("\n", "")
            .Replace("\r", "")
            .Replace(" ", "");
    
        formatter.Serialize(file, jsonData);
    
        file.Close();
        _inSave = false;
    }
    
    public void SaveGame()
    {
        if (_inSave) return;
        _inSave = true;
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/game.data");
        var jsonData = JsonConvert.SerializeObject(game, Formatting.Indented)
            .Replace("\n", "")
            .Replace("\r", "")
            .Replace(" ", "");
    
        formatter.Serialize(file, jsonData);
    
        file.Close();
        _inSave = false;
    }

    // 返回首页
    public void BackIndex(bool resetGame)
    {
        if (resetGame)
        {
            InitGame();
            SaveGame();
        }

        UIManager.instance.BackIndex();
        turnEvent.Invoke();
    }

    // 死亡提示
    public void DeadAlert(string tipTitle, string tipInfo)
    {
        gamePause = true;
        UIManager.instance.AlertNew(tipTitle, tipInfo, () =>
        {
            // 观看广告，结束后复活
            AdManager.instance.PlayRewardAd(() =>
            {
                game.Tech = config.techInit;
                turnEvent.Invoke();
                game.statistic["广告支持"]++;
            });
        }, () =>
        {
            BackIndex(true);
        });
    }
}

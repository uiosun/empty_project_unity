using System;
using System.Collections;
using System.Collections.Generic;
using _Common;
using F8Framework.F8ExcelTool;
using Index;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    // 外部属性
    public AlertPrefab alertPrefab;
    public IndexPanel indexPanel;
    public bool isDebug;
    [NonSerialized] public Canvas Canvas;
    
    private GameObject _remindListContent;
    private Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();
    
    private float _wantQuitTime = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        if (isDebug)
        {
            ReadExcel.Instance.LoadAllExcelData(); // 运行时加载所有配置
        }
        else
        {
            F8DataManager.Instance.LoadAll(); // 加载所有配置
        }
        
        Debug.Log(F8DataManager.Instance.GetSheet1ByID(1).name1);
        
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        Application.targetFrameRate = 45;

        Debug.Log("UIManager 创建完毕");
        DontDestroyOnLoad(instance);
    }

    private void Start()
    {
        indexPanel.gameObject.SetActive(true);
    }

    void Update()
    {
        if (_wantQuitTime > 0) _wantQuitTime -= Time.deltaTime;
        if (Input.GetKey(KeyCode.Escape))
        {
            if (_wantQuitTime > 0)
            {
                Application.Quit();
            }

            _wantQuitTime = 1;
        }
    }

    // 获得本场景下的最新画布
    public void RefreshNewScene(GameObject obj)
    {
        Canvas = obj.GetComponentsInParent<Canvas>()[0];
    }

    // 加载图片
    public Sprite GetSprite(string path, bool needCache = false)
    {
        if (_spriteCache.TryGetValue(path, out var sprite))
        {
            return sprite;
        }

        sprite = Resources.Load<Sprite>(path);
        if (needCache)
        {
            _spriteCache.Add(path, sprite);
        }

        return sprite;
    }

    public void AlertNew(string title, string info, Action callbackSure = null, Action callback = null)
    {
        var alert = Instantiate(alertPrefab, Canvas.transform);
        alert.SetInfo(title, info, callbackSure, callback);
    }

    public void BackIndex()
    {
        indexPanel.gameObject.SetActive(true);
    }
}
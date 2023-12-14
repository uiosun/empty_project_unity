using System;
using System.Collections;
using System.Collections.Generic;
using _Common;
using Index;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

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

        // todo 播放背景音乐？

        Debug.Log("SoundManager 创建完毕");
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

    //Drag a reference to the audio source which will play the sound effects.
    public AudioSource efxSource;

    //Drag a reference to the audio source which will play the music.
    public AudioSource musicSource;

    //The lowest a sound effect will be randomly pitched.
    public float lowPitchRange = .95f;

    //The highest a sound effect will be randomly pitched.
    public float highPitchRange = 1.05f;

    //Used to play single sound clips.
    public void PlaySingle(AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        efxSource.clip = clip;

        //Play the clip.
        efxSource.Play();
    }

    //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    public void RandomizeSfx(params AudioClip[] clips)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);

        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        efxSource.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        efxSource.clip = clips[randomIndex];

        //Play the clip.
        efxSource.Play();
    }
}
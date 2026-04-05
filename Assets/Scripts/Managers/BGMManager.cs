using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;

    private Dictionary<string, AudioClip> audioClipDictionary = new Dictionary<string, AudioClip>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeBGM();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (audioClipDictionary.TryGetValue("Garden_Intro", out AudioClip audioClip))
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeBGM()
    {
        foreach (AudioClip audioClip in audioClips)
        {
            if (!audioClipDictionary.ContainsKey(audioClip.name))
            {
                audioClipDictionary.Add(audioClip.name, audioClip);
            }
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.name)
        {
            case "IntroUI":
            case "Lobby":
            case "TeamLobby":
            case "Loading":
                PlayBGM("Garden_Intro");
                break;
            case "Map1":
                PlayBGM("Mid_Map1");
                break;
        }
    }

    public void PlayBGM(string bgmName)
    {
        if (audioClipDictionary.TryGetValue(bgmName, out AudioClip audioClip))
        {
            // 중복 방지
            if(audioSource.clip == audioClip && audioSource.isPlaying) return;
            
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
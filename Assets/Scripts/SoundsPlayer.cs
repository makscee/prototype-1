using System;
using UnityEngine;

public class SoundsPlayer : MonoBehaviour
{
    public AudioSource[] AudioSources = new AudioSource[4];

    public string[] Code = new string[4];

    PlayCode _playCode;
    public WaveModule[] WaveModules = new WaveModule[4];
    public GameObject[] ConfigRacks = new GameObject[4];

    void OnEnable()
    {
        GameManager.AfterServiceObjectsInitialized += CreateConfigRacks;
        GameManager.AfterServiceObjectsInitialized += Init;
    }

    void Init()
    {
        if (WaveModules[0] == null) return;
        
        for (var i = 0; i < 4; i++)
        {
            _playCode = new PlayCode(Code[i]);
            var newSource = gameObject.AddComponent<AudioSource>(); 
            newSource.playOnAwake = false;
            newSource.clip = WaveModules[i].GetClip();
            newSource.volume = WaveModules[i].AudioSource.volume;
            AudioSources[i] = newSource;
        }
    }

    void CreateConfigRacks()
    {
        for (var i = 0; i < 4; i++)
        {
            var configRack = Instantiate(Prefabs.Instance.ConfigRack, SharedObjects.Instance.UICanvas.transform);
            configRack.transform.position = transform.position + (Vector3)Utils.CoordsFromDir(i) * 2;
            var waveModule = configRack.transform.GetComponentInChildren<WaveModule>();
            WaveModules[i] = waveModule;
            ConfigRacks[i] = configRack;
            configRack.SetActive(false);
        }
    }

    public void ConfigRacksSetActive(bool value)
    {
        foreach (var configRack in ConfigRacks)
        {
            configRack.SetActive(value);
        }
    }
    void Update()
    {
        
    }

    public void Play(int dir)
    {
        AudioSources[dir].clip = WaveModules[dir].GetClip();
        AudioSources[dir].volume = WaveModules[dir].AudioSource.volume;
        AudioSources[dir].Play();
    }

    void OnDisable()
    {
        foreach (var audioSource in AudioSources)
        {
            Destroy(audioSource);
        }
    }
}
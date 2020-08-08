using System;
using UnityEngine;

public class SoundsPlayer : MonoBehaviour
{
    public AudioSource[] AudioSources = new AudioSource[4];
    public SoundConfig[] Configs = new SoundConfig[4];

    void Awake()
    {
        GameManager.InvokeAfterServiceObjectsInitialized(Init);
    }

    void Init()
    {
        for (var i = 0; i < 4; i++)
        {
            var newSource = gameObject.AddComponent<AudioSource>(); 
            newSource.playOnAwake = false;
            AudioSources[i] = newSource;
            Configs[i] = new SoundConfig();
        }
    }

    public void Play(int dir)
    {
        AudioSources[dir].clip = Configs[dir].Clip;
        AudioSources[dir].volume = Configs[dir].Volume;
        AudioSources[dir].Play();
    }

    void OnDisable()
    {
        // foreach (var audioSource in AudioSources)
        // {
        //     Destroy(audioSource);
        // }
    }
}
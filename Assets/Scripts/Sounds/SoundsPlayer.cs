using System;
using UnityEngine;

public class SoundsPlayer : MonoBehaviour
{
    public AudioSource[] AudioSources = new AudioSource[4];
    public SoundConfig[] Configs = new SoundConfig[4];
    public AudioClip Clip;

    void OnEnable()
    {
        Clip = Resources.Load<AudioClip>("Clips/clip1");
        for (var i = 0; i < 4; i++)
        {
            var newSource = gameObject.AddComponent<AudioSource>(); 
            newSource.playOnAwake = false;
            AudioSources[i] = newSource;
            Configs[i] = new SoundConfig(this);
        }
    }

    public void Play(int dir)
    {
        AudioSources[dir].clip = Configs[dir].Clip;
        AudioSources[dir].volume = Configs[dir].Volume;
        AudioSources[dir].Play();
    }
}
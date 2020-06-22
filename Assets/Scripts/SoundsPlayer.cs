using System;
using UnityEngine;

public class SoundsPlayer : MonoBehaviour
{
    public AudioSource[] AudioSources = new AudioSource[4];

    public string[] Code = new string[4];

    PlayCode _playCode;

    void OnEnable()
    {
        GameManager.AfterServiceObjectsInitialized += Init;
    }

    void Init()
    {
        for (var i = 0; i < 4; i++)
        {
            _playCode = new PlayCode(Code[i]);
            var newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            newSource.clip = _playCode.GetClip(PulseBlockCenter.Instance.Clip);
            AudioSources[i] = newSource;
        }
    }
    void Update()
    {
        
    }

    public void Play(int dir)
    {
        AudioSources[dir].Play();
    }
    
    
}
using System;
using UnityEngine;

public class SoundsPlayer : MonoBehaviour
{
    public AudioSource[] AudioSources = new AudioSource[4];

    static SoundsPlayer _instace;

    public AudioClip Clip;
    public string[] Code = new string[4];

    PlayCode _playCode;
    void OnEnable()
    {
        _instace = this;
        for (var i = 0; i < 4; i++)
        {
            _playCode = new PlayCode(Code[i]);
            var newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            newSource.clip = _playCode.GetClip(Clip);
            AudioSources[i] = newSource;
        }
    }
    void Update()
    {
        
    }

    public static void Play(int dir)
    {
        _instace.AudioSources[dir].Play();
    }
    
    
}
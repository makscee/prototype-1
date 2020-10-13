using System;
using UnityEngine;

public class SlicedClipPicker : MonoBehaviour
{
    public SlicedAudioClip slicedClip;
    [SerializeField] int rootId;
    AudioSource _audioSource;

    void Start()
    {
        if (slicedClip == null)
            slicedClip = GetComponent<WaveRenderer>().SlicedClip;

        _audioSource = gameObject.AddComponent<AudioSource>();
        rootId = GetComponentInParent<RootIdHolder>().id;
    }

    public void Play()
    {
        if (slicedClip == null) 
            throw new Exception("No audio clip in SlicedClipPicker");
        _audioSource.PlayOneShot(slicedClip.audioClip);
    }

    public void Apply()
    {
        if (slicedClip == null) 
            throw new Exception("No audio clip in SlicedClipPicker");
        Roots.Root[rootId].slicedClip = slicedClip;
        Roots.Root[rootId].onSlicedClipUpdate?.Invoke();
    }
}
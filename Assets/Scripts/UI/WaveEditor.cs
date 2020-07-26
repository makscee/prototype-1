using System;
using UnityEngine;
using UnityEngine.UI;

public class WaveEditor : MonoBehaviour
{
    AudioSource _audioSource;
    [SerializeField]AudioClip clip;
    WaveTextureProvider _textureProvider;
    RectTransform _rect;

    float _topPicker = 0.25f, _bottomPicker = 0.75f;
    RawImage _waveImage;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _waveImage = GetComponent<RawImage>();
        _audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        var rect = _rect.rect;
        int width = Mathf.RoundToInt(rect.width), height = Mathf.RoundToInt(rect.height);
        _textureProvider = new WaveTextureProvider(clip, width, height);
        _waveImage.texture = _textureProvider.GetTexture();
    }
    
    
}
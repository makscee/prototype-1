using System;
using TMPro;
using UnityEngine;

public class HelpEntry : MonoBehaviour
{
    [SerializeField] string text;
    [SerializeField] TextAsset clipTexture;
    
    TextMeshProUGUI _text;
    ClipTexture _clipTexture;
    

    void OnEnable()
    {
        AcquireComponents();
        Refresh();
    }

    void AcquireComponents()
    {
        _clipTexture = GetComponentInChildren<ClipTexture>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Refresh()
    {
        if (_clipTexture == null || _text == null)
        {
            AcquireComponents();
        }
        var ind = transform.GetSiblingIndex() + 1;
        _text.text = $"{ind}. {text}";
        _clipTexture.clipTexture = clipTexture;
    }

    void OnValidate()
    {
        Refresh();
    }
}
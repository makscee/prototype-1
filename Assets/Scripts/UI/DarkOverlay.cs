using System;
using UnityEngine;
using UnityEngine.UI;

public class DarkOverlay : MonoBehaviour
{
    const float FadeTime = 0.25f, Darkness = 0.5f;
    static DarkOverlay _instance;
    public static Action OnTap, OnNextTap;

    public float Alpha
    {
        get => _rawImage.color.a;
        set
        {
            var c = _rawImage.color;
            c.a = value;
            _rawImage.color = c;
        }
    }

    public static void Enable()
    {
        Animator.Interpolate(0f, Darkness, FadeTime).PassDelta(v => _instance.Alpha += v);
        _instance.gameObject.SetActive(true);
    }

    RawImage _rawImage;
    void Awake()
    {
        _rawImage = GetComponent<RawImage>();
        _instance = this;
        gameObject.SetActive(false);
    }

    public void Disable()
    {
        Animator.Interpolate(Darkness, 0f, FadeTime).PassDelta(v => Alpha += v).WhenDone(() => gameObject.SetActive(false));
        OnTap?.Invoke();
        OnNextTap?.Invoke();
        OnNextTap = null;
    }
}
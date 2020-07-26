using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DarkOverlay : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
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

    Camera _camera;
    void Start()
    {
        _camera = SharedObjects.Instance.Camera;
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
        if (CameraScript.WasZoomingLastFrame || _dragging) return;
        Animator.Interpolate(Darkness, 0f, FadeTime).PassDelta(v => Alpha += v).WhenDone(() => gameObject.SetActive(false));
        OnTap?.Invoke();
        OnNextTap?.Invoke();
        OnNextTap = null;
    }

    
    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount < 2)
        {
            _camera.transform.position -= _camera.ScreenToWorldPoint(eventData.delta) - _camera.ScreenToWorldPoint(Vector3.zero);
        }
    }

    bool _dragging;
    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
    }
}
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class RollingButton : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent action;
    public float showX, hideX;
    const float AnimationTime = 0.2f;
    RectTransform _rectTransform;
    bool _shown;

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _rectTransform.anchoredPosition = new Vector2(hideX, _rectTransform.anchoredPosition.y);
    }

    public void Show()
    {
        if (_shown) return;
        _shown = true;
        Animator.Interpolate(hideX, showX, AnimationTime).PassDelta(v =>
        {
            _rectTransform.anchoredPosition += new Vector2(v, 0f);
        }).Type(InterpolationType.OverflowReturn);
    }

    public void Hide()
    {
        if (!_shown) return;
        _shown = false;
        Animator.Interpolate(showX, hideX, AnimationTime).PassDelta(v =>
        {
            _rectTransform.anchoredPosition += new Vector2(v, 0f);
        }).Type(InterpolationType.InvSquare);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        action?.Invoke();
    }
}
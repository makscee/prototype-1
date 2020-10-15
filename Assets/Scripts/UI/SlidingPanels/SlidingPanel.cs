using System;
using UnityEngine;

[ExecuteInEditMode]
public class SlidingPanel : MonoBehaviour
{
    const float AnimationTime = 0.2f;
    public float targetWidth;
    public float openWidth;
    public float closedWidth;
    public float speed = 7;
    public SlidingPanelBookmark bookmark;
    RectTransform _rectTransform;
    public bool orientedLeft;
    public Action onOpen, onClose;

    public bool IsOpen { get; private set; }

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        bookmark.onDrag += v =>
        {
            if (orientedLeft) targetWidth = Utils.ScaledScreenCoords(Input.mousePosition, transform).x;
            else targetWidth = Utils.ScaledScreenCoords(new Vector2(Screen.width - Input.mousePosition.x, 0), transform).x;
        };
    }

    void Update()
    {
        var width = _rectTransform.rect.width;
        SetWidth(width + (targetWidth - width) * Time.deltaTime * speed);
    }

    void SetWidth(float width)
    {
        _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    public Interpolator<float> Open()
    {
        if (IsOpen) return null;
        onOpen?.Invoke();
        IsOpen = true;
        return Animator.Interpolate(_rectTransform.rect.width, openWidth, AnimationTime).Type(InterpolationType.OverflowReturn)
            .PassValue(
                v =>
                {
                    SetWidth(v);
                    targetWidth = v;
                });
    }

    public Interpolator<float> Close()
    {
        if (!IsOpen) return null;
        IsOpen = false;
        onClose?.Invoke();
        return Animator.Interpolate(_rectTransform.rect.width, closedWidth, AnimationTime).Type(InterpolationType.OverflowReturn)
            .PassValue(
                v =>
                {
                    SetWidth(v);
                    targetWidth = v;
                });
    }
}
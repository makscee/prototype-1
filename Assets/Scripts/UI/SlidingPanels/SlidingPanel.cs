using System;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SlidingPanel : MonoBehaviour
{
    public float targetWidth;
    public float openWidth;
    public float closedWidth;
    public float speed = 7;
    public SlidingPanelBookmark bookmark;
    RectTransform _rectTransform;
    public bool orientedLeft;

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

    public void Open()
    {
        gameObject.SetActive(true);
        targetWidth = openWidth;
        IsOpen = true;
    }

    public void Close()
    {
        targetWidth = closedWidth;
        IsOpen = false;
    }

    const float TargetReachedThreshold = 30f;
    public bool TargetReached => Mathf.Abs(_rectTransform.rect.width - targetWidth) < TargetReachedThreshold;
}
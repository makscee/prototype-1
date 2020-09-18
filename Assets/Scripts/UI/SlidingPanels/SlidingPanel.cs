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

    public bool IsOpen { get; private set; }

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        // bookmark.onDrag += v => targetWidth -= Utils.ScaledScreenCoords(v, transform).x;
        bookmark.onDrag += v =>
        {
            targetWidth = Screen.width - Input.mousePosition.x;
            // bookmark.transform.position += new Vector3(0f, v.y);
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
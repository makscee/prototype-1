using System;
using UnityEngine;

[ExecuteInEditMode]
public class SlidingPanelsFolder : MonoBehaviour
{
    public SlidingPanel primary;
    public SlidingPanel secondary;
    public float bookmarksOffsetByScreenDivision;
    public float bookmarkOffsetByPixels;
    SlidingPanelsGroup _parent;
    public Action onTargetReached;

    public bool IsOpen => primary.IsOpen || secondary.IsOpen;
    void Awake()
    {
        _parent = GetComponentInParent<SlidingPanelsGroup>();
        primary.bookmark.onClick += () => _parent.OpenOneCloseRest(this);
        primary.bookmark.onBeginDrag += () =>
        {
            if (!primary.IsOpen) _parent.OpenOneCloseRest(this);
        };

        var offsetPos = Screen.height / bookmarksOffsetByScreenDivision;
        primary.bookmark.transform.position = new Vector3(primary.bookmark.transform.position.x, offsetPos + bookmarkOffsetByPixels);
        secondary.bookmark.transform.position = new Vector3(secondary.bookmark.transform.position.x, offsetPos + bookmarkOffsetByPixels);

        primary.Close();
        secondary.Close();
    }

    void Update()
    {
        if (onTargetReached != null && primary.TargetReached && secondary.TargetReached)
        {
            onTargetReached();
            onTargetReached = null;
        }
    }

    public void Open()
    {
        primary.Open();
        secondary.Open();
    }

    public void Close()
    {
        primary.Close();
        secondary.Close();
    }
}
using System;
using UnityEngine;

[ExecuteInEditMode]
public class SlidingPanelsFolder : MonoBehaviour
{
    public SlidingPanel primary;
    public SlidingPanel secondary;
    public Action onOpen;
    public bool open;

    SlidingPanelsGroup _parent;
    
    public bool IsOpen => primary.IsOpen || secondary.IsOpen;
    void Start()
    {
        _parent = GetComponentInParent<SlidingPanelsGroup>();
        primary.bookmark.onClick += () => _parent.OpenOneCloseRest(this);
        primary.bookmark.onBeginDrag += () =>
        {
            if (!primary.IsOpen) _parent.OpenOneCloseRest(this);
        };

        GameManager.OnNextFrame += () =>
        {
            var ind = transform.GetSiblingIndex();
            var offset = SlidingPanelBookmark.GetHeightPositionByIndex(ind, transform);
            primary.bookmark.transform.position = new Vector3(primary.bookmark.transform.position.x, offset);
            secondary.bookmark.transform.position = new Vector3(secondary.bookmark.transform.position.x, offset);
        };

        if (open)
        {
            primary.Open();
            secondary.Open();
        }
        else
        {
            primary.Close();
            secondary.Close();
        }
    }

    public void Open()
    {
        primary.Open();
        secondary.Open();
        onOpen?.Invoke();
    }

    public void Close()
    {
        if (this == null || transform == null) return;
        transform.SetAsFirstSibling();
        primary.Close()?.OnDeltaSignChange(transform.SetAsLastSibling);
        secondary.Close();
    }
}
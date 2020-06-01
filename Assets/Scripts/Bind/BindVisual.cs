using System;
using UnityEngine;

public class BindVisual : MonoBehaviour
{
    public IBindable first, second;
    RectTransform _rectTransform;

    const float MinWidth = 0.7f, MaxWidth = 0.9f;
    float MaxLength;

    public Bind bind;

    public static void Create(Bind bind)
    {
        var b = Instantiate(Prefabs.Instance.BindVisual, SharedObjects.Instance.Canvas.transform).GetComponent<BindVisual>();
        b.first = bind.First;
        b.second = bind.Second;
        b.MaxLength = bind.BreakDistance > -1 ? bind.BreakDistance : 3;
        b.bind = bind;
        if (b.first is Block block)
        {
            ColorPalette.SubscribeToGameObject(block.inside.gameObject, b.gameObject);
        }
        else
        {
            ColorPalette.SubscribeGameObject(b.gameObject, 3);
        }
    }

    void OnEnable()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!BindMatrix.IsBound(first, second))
        {
            Destroy(gameObject);
            return;
        }
        
        bind.Update();
        
        var firstPosition = first.GetPosition();
        var dir = (second.GetPosition() - firstPosition) / 2;
        transform.position = firstPosition + dir;
        
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        var length = dir.magnitude * 2 - Block.BlockSide / 3;
        var width = Mathf.Lerp(MaxWidth, MinWidth, length / MaxLength);
        _rectTransform.sizeDelta = new Vector2(width, length);
    }
}
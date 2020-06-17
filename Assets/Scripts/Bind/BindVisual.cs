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
        ColorPalette palette;
        if (bind.First is Block t && t.PulseBlock != null)
            palette = t.PulseBlock.ColorPalette;
        else return;
        
        var b = Instantiate(Prefabs.Instance.BindVisual, SharedObjects.Instance.FrontCanvas.transform).GetComponent<BindVisual>();
        b.first = bind.First;
        b.second = bind.Second;
        b.MaxLength = bind.BreakDistance > -1 ? bind.BreakDistance : 3;
        b.bind = bind;
        
        if (b.first is Block block)
        {
            palette.SubscribeToGameObject(block.inside.gameObject, b.gameObject);
        }
        else
        {
            palette.SubscribeGameObject(b.gameObject, 3);
        }
    }

    void OnEnable()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    void OnDisable()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (BindMatrix.GetBind(first, second) != bind)
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
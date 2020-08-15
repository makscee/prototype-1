using System;
using UnityEngine;
using UnityEngine.UI;

public class BindVisual : MonoBehaviour
{
    public IBindable first, second;
    RectTransform _rectTransform;

    const float MinWidth = 0.5f, MaxWidth = 0.7f;
    float MaxLength;

    public Bind bind;

    Painter _painter;
    Image _image;
    void Awake()
    {
        _image = GetComponent<Image>();
        _painter = GetComponent<Painter>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public static bool Create(Bind bind, out BindVisual bindVisual)
    {
        bindVisual = null;
        if (!(bind.First is Block block))
            return false;
        
        bindVisual = Instantiate(Prefabs.Instance.bindVisual, SharedObjects.Instance.bindVisualsCanvas.transform).GetComponent<BindVisual>();
        bindVisual.first = bind.First;
        bindVisual.second = bind.Second;
        bindVisual.MaxLength = bind.BreakDistance > -1 ? bind.BreakDistance : 3;
        bindVisual.bind = bind;
        bindVisual._painter.subscribedTo = block.view.secondaryPainter;

        return true;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        bind.Update();
        
        var firstPosition = first.GetPosition();
        var dir = (second.GetPosition() - firstPosition) / 2;
        transform.position = firstPosition + dir;
        
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        var length = dir.magnitude * 2 - BlockOld.BlockSide / 3;
        var width = Mathf.Lerp(MaxWidth, MinWidth, length / MaxLength);
        _rectTransform.sizeDelta = new Vector2(width, length);
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class BindVisual : MonoBehaviour
{
    public IBindable first, second;
    RectTransform _rectTransform;

    const float MinWidth = 0.5f, MaxWidth = 0.7f;
    float MaxLength;

    public Bind bind;

    Painter _painter;
    bool isRectFirst;
    RectTransform firstRect;
    void Awake()
    {
        _painter = GetComponent<Painter>();
        _rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        if (bind.First is Block block)
        {
            isRectFirst = true;
            firstRect = block.view.VisualBase.Current.GetComponent<RectTransform>();
            _painter.subscribedTo = block.view.SecondaryPainter;
            block.view.VisualBase.onModelChange += model => _painter.subscribedTo = model.secondary;
        }  
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
        bindVisual._painter.subscribedTo = block.view.SecondaryPainter;

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


        var maxWidth = MaxWidth;
        var minWidth = MinWidth;
        if (isRectFirst)
        {
            var rect = firstRect.rect;
            maxWidth = Mathf.Min(rect.width, rect.height) / 2.5f;
            minWidth = maxWidth - 0.1f;
        }
        var length = dir.magnitude * 2 - 1f / 3;
        var width = Mathf.Lerp(maxWidth, minWidth, length / MaxLength);
        _rectTransform.sizeDelta = new Vector2(width, length);
    }
}
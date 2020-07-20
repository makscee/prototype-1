using System;
using UnityEngine;
using UnityEngine.UI;

public class BindVisual : MonoBehaviour
{
    public IBindable first, second;
    RectTransform _rectTransform;

    const float MinWidth = 0.7f, MaxWidth = 0.9f;
    float MaxLength;

    public Bind bind;

    Painter _painter;
    Image _image;
    void Awake()
    {
        _image = GetComponent<Image>();
        _painter = GetComponent<Painter>();
    }

    public static void Create(Bind bind)
    {
        Palette palette;
        if (bind.First is Block block && block.pulseBlock != null)
            palette = block.pulseBlock.palette;
        else return;
        
        var b = Instantiate(Prefabs.Instance.BindVisual, SharedObjects.Instance.FrontCanvas.transform).GetComponent<BindVisual>();
        b.first = bind.First;
        b.second = bind.Second;
        b.MaxLength = bind.BreakDistance > -1 ? bind.BreakDistance : 3;
        b.bind = bind;
        
        b._painter.subscribedTo = block.insidePainter;
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

        var imageActive = true;
        if (bind.First is BindableMonoBehavior mFirst) imageActive = mFirst.isActiveAndEnabled;
        if (bind.Second is BindableMonoBehavior mSecond) imageActive = imageActive && mSecond.isActiveAndEnabled;
        _image.enabled = imageActive;
        
        
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
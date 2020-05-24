using System;
using UnityEngine;

public class BindVisual : MonoBehaviour
{
    public IBindable first, second;
    RectTransform _rectTransform;

    const float MinWidth = 0, MaxWidth = 4;
    float MaxLength;

    public Bind bind;

    public static void Create(IBindable first, IBindable second, float breakDistance)
    {
        var b = Instantiate(Prefabs.Instance.BindVisual, SharedObjects.Instance.Canvas.transform).GetComponent<BindVisual>();
        b.first = first;
        b.second = second;
        b.MaxLength = breakDistance > -1 ? breakDistance : 300;
    }

    void OnEnable()
    {
        _rectTransform = GetComponent<RectTransform>();
        ColorPalette.SubscribeGameObject(gameObject, 3);
    }

    void Update()
    {
        if (!BindMatrix.IsBound(first, second))
        {
            Destroy(gameObject);
            return;
        }
        
        var firstPosition = first.GetPosition();
        var dir = (second.GetPosition() - firstPosition) / 2;
        transform.position = firstPosition + dir;
        
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        var length = dir.magnitude * 2;
        var width = Mathf.Lerp(MaxWidth, MinWidth, length / MaxLength);
        _rectTransform.sizeDelta = new Vector2(width, dir.magnitude * 2);
    }
}
using System;
using UnityEngine;

public class CentralRack : MonoBehaviour
{
    public Palette palette;
    public RectTransform rectTransform;

    void OnEnable()
    {
        
    }

    public void Show()
    {
        transform.position = Vector3.zero;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);
        gameObject.SetActive(true);

        var directions = new Vector3[]
        {
            new Vector3(1.5f, 0, 1f),
            new Vector3(0, 1.5f, 1f),
            new Vector3(-1.5f, 0, 1f),
            new Vector3(0, -1.5f, 1f),
        };
        Utils.Shuffle(ref directions);
        const float iterationTime = 0.1f;
        for (var i = 0; i < directions.Length; i++)
        {
            AnimateWrap(directions[i], iterationTime).Delay(iterationTime * i);
        }
    }
    public void Hide()
    {
        transform.position = Vector3.zero;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 3);
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 3);

        var directions = new Vector3[]
        {
            new Vector3(1.5f, 0),
            new Vector3(0, 1.5f),
            new Vector3(-1.5f, 0),
            new Vector3(0, -1.5f),
        };
        Utils.Shuffle(ref directions);
        const float iterationTime = 0.1f;
        for (var i = 0; i < directions.Length; i++)
        {
            AnimateWrap(directions[i], iterationTime).Delay(iterationTime * i);
        }

        Animator.Invoke(() => gameObject.SetActive(false)).In(iterationTime * directions.Length);
    }

    Interpolator<float> AnimateWrap(Vector3 dir, float iterationTime)
    {
        var grow = dir.z > 0f ? 1f : -1f;
        return Animator.Interpolate(0f, 1f, iterationTime).PassDelta(v =>
        {
            transform.position += dir * v / 2;
            if (dir.x != 0f)
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                    rectTransform.rect.width + Mathf.Abs(dir.x) * v * grow);
            else
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
                    rectTransform.rect.height + Mathf.Abs(dir.y) * v * grow);
        }).Type(InterpolationType.InvSquare);
    }
}
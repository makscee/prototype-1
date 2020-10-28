using UnityEngine;
using UnityEngine.UI;

public class FadeoutCircleEffect : MonoBehaviour
{
    float _time, _endSize;
    void Start()
    {
        var image = GetComponent<RawImage>();
        var rectTransform = GetComponent<RectTransform>();
        Animator.Interpolate(0f, 1f, _time).PassValue(v =>
        {
            image.color = image.color.LerpAlpha(1f, 0f, v);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, v * _endSize);
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, v * _endSize);
        }).WhenDone(() => Destroy(gameObject)).NullCheck(rectTransform.gameObject);
    }
    
    public static void Create(Transform under, float time, float endSize)
    {
        var go = Instantiate(Prefabs.Instance.FadeoutCircle, under.parent);
        go.transform.SetSiblingIndex(under.GetSiblingIndex());
        go.transform.position = under.position;
        var circle = go.GetComponent<FadeoutCircleEffect>();
        circle._time = time;
        circle._endSize = endSize;
    }
}
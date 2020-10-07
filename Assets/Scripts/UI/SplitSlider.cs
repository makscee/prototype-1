using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SplitSlider : MonoBehaviour, IDragHandler, IPointerClickHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] RawImage top, bottom;
    [SerializeField] RectTransform rect;
    [SerializeField] TextMeshProUGUI valueText;

    float _valueBefore;
    [field: Range(0f, 1f)] public float value;
    public float Min = 0f, Max = 1f;
    public float MultipliedValueFloat => Min + value * (Max - Min);
    public int MultipliedValueInt => (int) MultipliedValueFloat;

    public void InitValue(float v)
    {
        value = (v - Min) / (Max - Min);
    }
    void Update()
    {
        if (Math.Abs(top.rectTransform.rect.height - GetHeight(value, true)) > 0.001f ||
            Math.Abs(bottom.rectTransform.rect.height - GetHeight(value, false)) > 0.001f)
        {
            value = Mathf.Clamp(value, 0f, 1f);
            ValueUpdated(value);
            _valueBefore = value;
        }
    }

    float GetHeight(float v, bool top)
    {
        return (top ? 1 - v : v) * rect.rect.height;
    }
    void ValueUpdated(float v)
    {
        var heightTotal = rect.rect.height;
        var topHeight = GetHeight(v, true);
        var bottomHeight = GetHeight(v, false);
        top.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, topHeight);
        bottom.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bottomHeight);
        var textRectTransform = valueText.rectTransform;
        var textPos = textRectTransform.anchoredPosition;
        var textHeight = textRectTransform.rect.height;
        textPos.y = Mathf.Clamp(bottomHeight - textHeight, 0, heightTotal - textHeight * 2);
        textRectTransform.anchoredPosition = textPos;
        
        valueText.text = Mathf.RoundToInt(MultipliedValueInt).ToString();
        
        onValueChange?.Invoke(MultipliedValueFloat);
    }

    bool _dragging;
    public void OnDrag(PointerEventData eventData)
    {
        value += eventData.delta.y / rect.rect.height;
    }

    public Action<float> onValueChange;
    public Action onClick;
    public Action onDragEnd;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_dragging) return;
        onClick?.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
        onDragEnd?.Invoke();
    }
}
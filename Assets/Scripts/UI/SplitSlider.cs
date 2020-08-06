using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SplitSlider : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    [SerializeField] RawImage top, bottom;
    [SerializeField] RectTransform rect;
    [SerializeField] Text valueText;

    float _valueBefore;
    [field: Range(0f, 1f)] public float value;
    public float Min = 0f, Max = 1f;
    public float MultipliedValueFloat => Min + value * (Max - Min);
    public int MultipliedValueInt => (int) MultipliedValueFloat;

    void Update()
    {
        if (value != _valueBefore)
        {
            value = Mathf.Clamp(value, 0f, 1f);
            ValueUpdated(value);
            _valueBefore = value;
        }
    }
    
    void ValueUpdated(float v)
    {
        var heightTotal = rect.rect.height;
        var topHeight = (1 - v) * heightTotal;
        var bottomHeight = v * heightTotal;
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

    public void OnDrag(PointerEventData eventData)
    {
        value += eventData.delta.y / rect.rect.height;
    }

    public Action<float> onValueChange;
    public Action onClick;
    public Action onDragEnd;
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }
}
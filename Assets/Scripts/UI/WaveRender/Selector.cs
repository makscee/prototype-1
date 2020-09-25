using System;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    [SerializeField] RawImage top, bottom, left, right;
    RectTransform _rectTransform;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Show(_rectTransform.rect.width > 0); 
    }

    void Show(bool value)
    {
        top.enabled = value;
        bottom.enabled = value;
        left.enabled = value;
        right.enabled = value;
    }
}
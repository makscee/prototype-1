using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class WaveEditorAddRecordButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action OnDown, OnUp;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnUp?.Invoke();
    }
}
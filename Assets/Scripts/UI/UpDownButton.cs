using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UpDownButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
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

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
using System;
using UnityEngine;
using UnityEngine.UI;

public class BlockVisualModel : MonoBehaviour
{
    public Painter primary, secondary;
    string _text;
    [SerializeField]Text textComponent;
    public bool IsShown { get; private set; }

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            if (textComponent != null) textComponent.text = value;
        }
    }

    public void Hide()
    {
        IsShown = false;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        IsShown = true;
        gameObject.SetActive(true);
    }
}
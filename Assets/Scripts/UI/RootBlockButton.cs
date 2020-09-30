using System;
using UnityEngine;
using UnityEngine.UI;

public class RootBlockButton : MonoBehaviour
{
    public int rootId;
    [SerializeField] Palette palette;

    [SerializeField] GameObject addButton;
    [SerializeField] RawImage rawImage;
    [SerializeField] GameObject icon;

    void OnEnable()
    {
        if (Roots.Root[rootId].palette != null)
            Enable();
        else if (Roots.Count == rootId)
            EnableAdd();
        else
            Disable();
    }

    void Disable()
    {
        rawImage.enabled = false;
        icon.SetActive(false);
    }

    void Enable()
    {
        rawImage.enabled = true;
        icon.SetActive(true);
    }

    void EnableAdd()
    {
        Disable();
        addButton.SetActive(true);
    }
}
using System;
using UnityEngine;

public class RootBlockButton : MonoBehaviour
{
    public int rootId;
    [SerializeField] Palette palette;

    void OnEnable()
    {
        palette.copyOf = Roots.Palettes(rootId);
    }
}
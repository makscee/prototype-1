using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RootEditor : MonoBehaviour
{
    public Transform buttonBackgroundsParent;
    public GameObject buttonBackgroundPrefab;
    public int rootBlockId;
    public RootBlock RootBlock => Roots.Blocks[rootBlockId];
    Dictionary<string, Transform> _buttonBackgrounds = new Dictionary<string, Transform>();
    [SerializeField] Button[] defaultButtons;
    [SerializeField] Palette palette;

    RootEditorScreen _active;

    void Start()
    {
        if (_active == null)
        {
            foreach (var button in defaultButtons)
            {
                button.onClick.Invoke();
            }

            palette.copyOf = Roots.Palettes(rootBlockId);
        }
    }

    public void SelectScreen(RootEditorScreen screen)
    {
        if (_active != null) _active.Deselect();
        screen.Select();
        _active = screen;
    }

    public void DisplayButtonBackground(Transform targetTransform)
    {
        var id = targetTransform.tag;
        if (!_buttonBackgrounds.ContainsKey(id))
        {
            var newBg = Instantiate(buttonBackgroundPrefab, buttonBackgroundsParent);
            _buttonBackgrounds.Add(id, newBg.transform);
        }

        _buttonBackgrounds[id].position = targetTransform.position;
    }
    
}
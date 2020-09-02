using UnityEngine;

public class RootEditorsFolder : MonoBehaviour
{
    RootEditor _active;

    public void SelectEditor(RootEditor rootEditor)
    {
        if (_active != null) _active.gameObject.SetActive(false);
        rootEditor.gameObject.SetActive(true);
        _active = rootEditor;
    }
}
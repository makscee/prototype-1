using System;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPanelsGroup : MonoBehaviour
{
    SlidingPanelsFolder[] _folders;

    void Awake()
    {
        CollectChildren();
    }

    public void CollectChildren()
    {
        _folders = GetComponentsInChildren<SlidingPanelsFolder>();
    }

    bool _closeActionAdded;
    public void OpenOneCloseRest(SlidingPanelsFolder openFolder)
    {
        openFolder.Open();
        openFolder.transform.SetAsFirstSibling();
        foreach (var folder in _folders)
        {
            if (folder != openFolder && folder.IsOpen)
            {
                folder.Close();
            }
        }

        if (!_closeActionAdded)
        {
            SharedObjects.Instance.backgroundInputHandler.nextClickOverride += CloseAll;
            _closeActionAdded = true;
        }
        transform.SetAsLastSibling();
    }

    public void CloseAll()
    {
        _closeActionAdded = false;
        foreach (var folder in _folders)
        {
            folder.Close();
        }
    }
}
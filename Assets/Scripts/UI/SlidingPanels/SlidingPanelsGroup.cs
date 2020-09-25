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
        var actionAdded = false;
        foreach (var folder in _folders)
        {
            if (folder != openFolder && folder.IsOpen)
            {
                folder.Close();
                if (!actionAdded)
                {
                    folder.onTargetReached += () =>
                    {
                        openFolder.Open();
                        openFolder.transform.SetAsFirstSibling();
                    };
                    actionAdded = true;
                }
            }
        }

        if (!actionAdded)
        {
            openFolder.Open();
            openFolder.transform.SetAsFirstSibling();
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
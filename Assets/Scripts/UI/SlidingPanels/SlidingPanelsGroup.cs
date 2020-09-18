using System;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPanelsGroup : MonoBehaviour
{
    SlidingPanelsFolder[] _folders;

    void Awake()
    {
        _folders = GetComponentsInChildren<SlidingPanelsFolder>();
    }

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
        SharedObjects.Instance.backgroundInputHandler.nextClickOverride = CloseAll;
    }

    public void CloseAll()
    {
        foreach (var folder in _folders)
        {
            folder.Close();
        }
    }
}
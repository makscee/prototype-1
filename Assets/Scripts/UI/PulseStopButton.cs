using System;
using System.Collections;
using UnityEngine;

public class PulseStopButton : MonoBehaviour
{
    RollingButton _button;

    void Start()
    {
        _button = GetComponent<RollingButton>();
        StartCoroutine(nameof(ActivePulseCheck));
    }

    IEnumerator ActivePulseCheck()
    {
        while (true)
        {
            var shown = false;
            foreach (var root in Roots.Root.Values)
            {
                if (root.pulse.HasActivePulse)
                {
                    _button.Show();
                    shown = true;
                    break;
                }
            }
            if (!shown) _button.Hide();
            yield return new WaitForSeconds(1f);
        }
    }

    public void DoStop()
    {
        BlockLogic.KillAllPulses();
    }
}
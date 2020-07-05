using UnityEngine;

public class RecordAppendModule : MonoBehaviour
{
    public WaveModule waveModule;

    public void DoAppend()
    {
        PulseBlockCenter.Instance.Clip = ClipMaker.Add(PulseBlockCenter.Instance.Clip, waveModule.GetOutClip());
    }
}
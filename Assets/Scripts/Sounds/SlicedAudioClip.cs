using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlicedAudioClip
{
    public AudioClip audioClip;
    public List<int> slices;
    public int Samples => slices.Last() - slices[0];

    public DoubleArray<float> data;

    SlicedAudioClip() {}

    public static SlicedAudioClip CreateFromData(AudioClip clip, int[] slices)
    {
        var data = new float[clip.samples];
        clip.GetData(data, 0);
        var dataDoubleArray = new DoubleArray<float>();
        dataDoubleArray.first = data;
        return new SlicedAudioClip {audioClip = clip, slices = new List<int>(slices), data = dataDoubleArray};
    }
    public static SlicedAudioClip CreateFromAsset(SlicedAudioClipAsset asset)
    {
        return CreateFromData(asset.audioClip, asset.slices);
    }

    public static SlicedAudioClip CreateFromFile(int rootId)
    {
        if (!FileStorage.GetAudioClipFromFile(rootId, out var audioClip) || !FileStorage.GetAudioClipSlicesFromFile(rootId, out var slices))
            return null;
        return CreateFromData(audioClip, slices);
    }

    public void RemoveSlice(int ind)
    {
        if (ind < 0 || ind > slices.Count - 1) throw new Exception($"Slice index out of range: ind = {ind}");
        audioClip = ClipMaker.CutAway(audioClip, slices[ind], slices[ind + 1]);
        var length = slices[ind + 1] - slices[ind];
        for (var i = ind + 2; i < slices.Count; i++)
            slices[i] -= length;
        slices.RemoveAt(ind + 1);
        data.first = new float[audioClip.samples];
        audioClip.GetData(data.first, 0);
    }

    public void StartRecording()
    {
        slices.Add(slices.Last());
        Recorder.StartRecording();
    }

    public void UpdateRecording()
    {
        if (Recorder.Samples == 0) return;
        data.second = new float[Recorder.Samples];
        Recorder.Recording.GetData(data.second, 0);
        slices[slices.Count - 1] = data.Length;
    }

    public void EndRecording()
    {
        Recorder.EndRecording();
        if (Recorder.Recording == null)
        {
            data.Drop();
            slices.RemoveAt(slices.Count - 1);
            return;
        }
        UpdateRecording();
        data.Merge();
        audioClip = AudioClip.Create(audioClip.name, data.Length, 1, 44100, false);
        audioClip.SetData(data.first, 0);
    }
}
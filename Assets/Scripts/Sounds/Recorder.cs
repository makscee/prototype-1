using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Recorder
{
    public static AudioClip Recording { get; private set; }
    static float _startRecordingTime;
    const float MinVolume = 0.008f;
    public static int Samples => _recordingInProgress ? Mathf.RoundToInt(RecordingLength * Recording.frequency) : Recording.samples;

    public static float RecordingLength => Time.time - _startRecordingTime;

    public static void EndRecording()
    {
        Microphone.End("");
        var trimmedRecord = AudioClip.Create("record", (int)((Time.time - _startRecordingTime) * Recording.frequency), 1, Recording.frequency, false);
        var data = new float[(int)((Time.time - _startRecordingTime) * Recording.frequency)];
        Recording.GetData(data, 0);
        trimmedRecord.SetData(data, 0);
        Recording = TrimSilenceAndNormalize(trimmedRecord, MinVolume);
        _recordingInProgress = false;
    }
    
    public static AudioClip TrimSilenceAndNormalize(AudioClip clip, float min)
    {
        var data = new float[clip.samples];
        clip.GetData(data, 0);
        var samples = new List<float>(data);
        
        int i;
        for (i = 0; i < samples.Count; i++)
        {
            if (Mathf.Abs(samples[i]) > min)
            {
                break;
            }
        }

        samples.RemoveRange(0, i);
        if (samples.Count == 0) return null;
        
        for (i = samples.Count - 1; i > 0; i--)
            if (Mathf.Abs(samples[i]) > min)
                break;

        samples.RemoveRange(i, samples.Count - i);
        Normalize(samples);

        var result = AudioClip.Create(clip.name, samples.Count, clip.channels, clip.frequency, false);
        result.SetData(samples.ToArray(), 0);
        return result;
    }

    public static void Normalize(List<float> data)
    {
        var max = 0f;
        foreach (var val in data)
        {
            max = Mathf.Max(Mathf.Abs(val), max);
        }
        var scale = 1f / max;
        for (int i = 0; i < data.Count; i++)
        {
            data[i] *= scale;
        }
    }

    static bool _recordingInProgress;
    public static void StartRecording()
    {
        // Microphone.GetDeviceCaps("", out var minFreq, out var maxFreq);
        Recording = Microphone.Start("", false, 5, 44100);
        _startRecordingTime = Time.time;
        _recordingInProgress = true;
    }
}
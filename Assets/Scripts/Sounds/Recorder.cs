using System.Collections.Generic;
using UnityEngine;

public static class Recorder
{
    static AudioClip _record;
    static float _startRecordingTime;
    const float MinVolume = 0.008f;

    public static float RecordingLength => Time.time - _startRecordingTime;

    public static void EndRecording()
    {
        Microphone.End("");
        var trimmedRecord = AudioClip.Create("record", (int)((Time.time - _startRecordingTime) * _record.frequency), 1, _record.frequency, false);
        var data = new float[(int)((Time.time - _startRecordingTime) * _record.frequency)];
        _record.GetData(data, 0);
        trimmedRecord.SetData(data, 0);
        _record = TrimSilence(trimmedRecord, MinVolume);
    }

    public static AudioClip GetLastRecording()
    {
        return _record;
    }
    
    public static AudioClip TrimSilence(AudioClip clip, float min)
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
        {
            if (Mathf.Abs(samples[i]) > min)
            {
                break;
            }
        }

        samples.RemoveRange(i, samples.Count - i);

        var result = AudioClip.Create(clip.name, samples.Count, clip.channels, clip.frequency, false);

        result.SetData(samples.ToArray(), 0);

        return result;
    }

    public static void StartRecording()
    {
        // Microphone.GetDeviceCaps("", out var minFreq, out var maxFreq);
        _record = Microphone.Start("", false, 5, 44100);
        _startRecordingTime = Time.time;
    }
}
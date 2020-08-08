using UnityEngine;

public static class Recorder
{
    static AudioClip record;
    public static float startRecordingTime;

    public static float RecordingLength => Time.time - startRecordingTime;

    public static void EndRecording()
    {
        Microphone.End("");
        var trimmedRecord = AudioClip.Create("record", (int)((Time.time - startRecordingTime) * record.frequency), 1, record.frequency, false);
        var data = new float[(int)((Time.time - startRecordingTime) * record.frequency)];
        record.GetData(data, 0);
        trimmedRecord.SetData(data, 0);
        record = trimmedRecord;
    }

    public static AudioClip GetLastRecording()
    {
        return record;
    }

    public static void StartRecording()
    {
        // Microphone.GetDeviceCaps("", out var minFreq, out var maxFreq);
        record = Microphone.Start("", false, 5, 44100);
        startRecordingTime = Time.time;
    }
}
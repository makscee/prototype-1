using UnityEngine;

public class Recorder : MonoBehaviour
{
    AudioClip record;
    float startRecordingTime;
    public WaveModule waveModule;

    public void EndRecording()
    {
        Debug.Log("end recording");
        Microphone.End("");
        var trimmedRecord = AudioClip.Create("record", (int)((Time.time - startRecordingTime) * record.frequency), 1, record.frequency, false);
        var data = new float[(int)((Time.time - startRecordingTime) * record.frequency)];
        record.GetData(data, 0);
        trimmedRecord.SetData(data, 0);
        record = trimmedRecord;
        if (waveModule != null)
        {
            waveModule.ClearOwn();
            waveModule.SetDirty();
            waveModule.GenerateTexture();
        }
    }

    public AudioClip GetLastRecording()
    {
        return record;
    }

    public void StartRecording()
    {
        Debug.Log("start recording");
        // Microphone.GetDeviceCaps("", out var minFreq, out var maxFreq);
        record = Microphone.Start("", false, 5, 44100);
        startRecordingTime = Time.time;
    }
}
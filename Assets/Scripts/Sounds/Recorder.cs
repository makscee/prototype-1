using UnityEngine;
using UnityEngine.EventSystems;

public class Recorder : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]AudioClip record;
    float startRecordingTime;
    [SerializeField]AudioSource _audioSource;

    public AudioClip EndRecording()
    {
        Microphone.End("");
        var trimmedRecord = AudioClip.Create("record", (int)((Time.time - startRecordingTime) * record.frequency), 1, record.frequency, false);
        var data = new float[(int)((Time.time - startRecordingTime) * record.frequency)];
        record.GetData(data, 0);
        trimmedRecord.SetData(data, 0);
        record = trimmedRecord;
        return record;
    }

    public AudioClip GetLastRecording()
    {
        return record;
    }

    public void StartRecording()
    {
        // Microphone.GetDeviceCaps("", out var minFreq, out var maxFreq);
        record = Microphone.Start("", false, 5, 44100);
        startRecordingTime = Time.time;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (_audioSource == null) _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null) return;
            _audioSource.PlayOneShot(record);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            StartRecording();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            EndRecording();
        }
    }
}
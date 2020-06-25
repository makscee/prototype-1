using UnityEngine;

public static class ClipMaker
{
    public static AudioClip Make(AudioClip clip, int sampleStart, int sampleAmount, int sampleRate)
    {
        var channels = clip.channels;
        var newClip = AudioClip.Create(clip.name + "-sub", sampleAmount, channels, sampleRate, false);
        var data = new float[sampleAmount * channels];
        clip.GetData(data, sampleStart);
        newClip.SetData(data, 0);
        return newClip;
    }
}
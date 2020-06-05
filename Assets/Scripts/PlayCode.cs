using UnityEngine;

public class PlayCode
{
    readonly string _code;

    public PlayCode(string code)
    {
        _code = code;
    }

    public AudioClip GetClip(AudioClip clip)
    {
        InterpretCode(out var sampleStart, out var sampleAmount, out var sampleRate);

        var channels = clip.channels;
        var newClip = AudioClip.Create(clip.name + "-sub", sampleAmount, channels, sampleRate, false);
        /* Create a temporary buffer for the samples */
        var data = new float[sampleAmount * channels];
        /* Get the data from the original clip */
        clip.GetData(data, sampleStart);
        /* Transfer the data to the new clip */
        newClip.SetData(data, 0);
        return newClip;
    }

    void InterpretCode(out int sampleStart, out int sampleAmount, out int sampleRate)
    {
        sampleStart = 0;
        sampleAmount = 0;
        sampleRate = 0;
        
        var s = _code.Split(' ');
        if (s[0] == "PLAY")
        {
            sampleStart = int.Parse(s[1]);
            sampleAmount = int.Parse(s[2]);
            sampleRate = int.Parse(s[3]);
        }
    }
}
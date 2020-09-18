using System;
using UnityEngine;

public static class ClipMaker
{
    public static AudioClip Make(AudioClip clip, int sampleStart, int sampleEnd, int sampleRate, bool clickCutOut = true)
    {
        if (sampleStart >= sampleEnd) return null;
        var sampleAmount = 0;
        // Debug.Log($"start: sampleStart = {sampleStart} sampleEnd = {sampleEnd} total = {clip.samples}");
        float[] data;
        if (clickCutOut)
        {
            var fullData = new float[clip.samples];
            clip.GetData(fullData, 0);

            var leftBorderStart = sampleStart;
            var rightBorderStart = sampleEnd - 1;
            var leftBorder = leftBorderStart;
            var rightBorder = rightBorderStart;
            
            while (true)
            {
                if (leftBorder == 0) break;
                if (fullData[leftBorderStart] > 0 != fullData[leftBorder] > 0)
                {
                    leftBorder++;
                    break;
                }

                leftBorder--;
            }
            while (true)
            {
                if (rightBorder >= fullData.Length - 1) break;
                if (fullData[rightBorderStart] > 0 != fullData[rightBorder] > 0)
                {
                    rightBorder--;
                    break;
                }

                rightBorder++;
            }
            
            sampleStart = leftBorder;
            sampleAmount = rightBorder - leftBorder + 1;
            data = new float[sampleAmount];
            Array.Copy(fullData, sampleStart, data, 0, sampleAmount);
            // for (var i = leftBorder; i <= rightBorder; i++)
            // {
                // data[i - leftBorder] = fullData[i];
            // }

            // clip.GetData(data, sampleStart);
            // Debug.Log($"end: left = {leftBorder} right = {rightBorder}");
        }
        else
        {
            sampleAmount = sampleEnd - sampleStart;
            data = new float[sampleAmount];
            clip.GetData(data, sampleStart);
        }
        
        var newClip = AudioClip.Create(clip.name + "-sub", sampleAmount, 1, sampleRate, false);
        newClip.SetData(data, 0);
        // Debug.Log($"end: sampleStart = {sampleStart} sampleAmount = {sampleAmount}");
        return newClip;
    }

    public static AudioClip Add(AudioClip clipA, AudioClip clipB)
    {
        if (clipA == null && clipB == null) return Empty;
        if (clipA == null) return clipB;
        if (clipB == null) return clipA;
        var samples = clipA.samples + clipB.samples;
        var data = new float[samples];
        clipA.GetData(data, 0);
        var dataB = new float[clipB.samples];
        clipB.GetData(dataB, 0);
        var j = 0;
        for (var i = clipA.samples; i < samples; i++)
        {
            data[i] = dataB[j];
            j++;
        }

        var result = AudioClip.Create("A+B", samples, 1, clipA.frequency, false);
        result.SetData(data, 0);
        return result;
    }

    static AudioClip _empty;

    public static AudioClip Empty
    {
        get
        {
            if (_empty == null)
                _empty = AudioClip.Create("empty", 8, 1, 44100, false);
            return _empty;
        }
    }
}
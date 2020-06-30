using System;
using UnityEngine;

public static class ClipMaker
{
    public static AudioClip Make(AudioClip clip, int sampleStart, int sampleAmount, int sampleRate, bool clickCutOut = true)
    {
        //Debug.Log($"start: sampleStart = {sampleStart} sampleAmount = {sampleAmount}");
        float[] data;
        if (clickCutOut)
        {
            var fullData = new float[clip.samples];
            clip.GetData(fullData, 0);

            var leftBorderStart = sampleStart;
            var rightBorderStart = sampleStart + sampleAmount - 1;
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
                if (rightBorder == fullData.Length - 1) break;
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
            data = new float[sampleAmount];
            clip.GetData(data, sampleStart);
        }
        
        var newClip = AudioClip.Create(clip.name + "-sub", sampleAmount, 1, sampleRate, false);
        newClip.SetData(data, 0);
        // Debug.Log($"end: sampleStart = {sampleStart} sampleAmount = {sampleAmount}");
        return newClip;
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FileStorage
{
    static readonly string PathBase = Application.persistentDataPath + "/";
    public static string LoadGameFromFile(string fileName)
    {
        if (!File.Exists(Path(fileName))) return "{}";
        return File.ReadAllText(Path(fileName));
    }

    public static void SaveJsonToFile(string json, string fileName)
    {
        File.WriteAllText(Path(fileName), json);
    }

    public static void SaveAudioClipToFile(SlicedAudioClip clip, int rootId)
    {
        SaveAudioClipDataToFile(clip.data.first, rootId);
        SaveAudioClipSlicesToFile(clip, rootId);
    }

    static void SaveAudioClipSlicesToFile(SlicedAudioClip clip, int rootId)
    {
        var s = string.Join(" ", clip.slices);
        File.WriteAllText(GetClipSlicesPath(rootId), s);
    }

    static void SaveAudioClipDataToFile(IEnumerable<float> data, int rootId)
    {
        using (var file = File.Create(GetClipFilePath(rootId)))
        {
            using (var writer = new BinaryWriter(file))
            {
                foreach (var value in data)
                {
                    writer.Write(value);
                }
            }
        }
    }
    static string GetClipFilePath(int rootId)
    {
        return Path($"clip_{rootId}", "dat");
    }
    static string GetClipSlicesPath(int rootId)
    {
        return Path($"clip_slices{rootId}", "txt");
    }
    public static bool GetAudioClipFromFile(int rootId, out AudioClip result)
    {
        var path = GetClipFilePath(rootId);
        if (!File.Exists(path))
        {
            result = null;
            return false;
        }
        var bytes = File.ReadAllBytes(path);
        var data = new float[bytes.Length / 4];
        Buffer.BlockCopy(bytes, 0, data, 0, bytes.Length);
        result = AudioClip.Create("clip", data.Length, 1, 44100, false);
        result.SetData(data, 0);
        return true;
    }

    public static bool GetAudioClipSlicesFromFile(int rootId, out int[] result)
    {
        var path = Path(GetClipSlicesPath(rootId));
        if (!File.Exists(path))
        {
            result = null;
            return false;
        }
        var s = File.ReadAllText(path);
        var sArr = s.Split(new char[] {' '});
        result = new int[sArr.Length];
        for (var i = 0; i < sArr.Length; i++)
        {
            result[i] = int.Parse(sArr[i]);
        }

        return true;
    }

    static string Path(string fileName, string extension = "json")
    {
        return $"{PathBase}{fileName}.{extension}";
    }
}
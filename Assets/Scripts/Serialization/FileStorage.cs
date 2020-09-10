using System;
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

    public static void SaveAudioClipToFile(AudioClip clip, int rootId)
    {
        var data = new float[clip.samples];
        clip.GetData(data, 0);
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

    static string Path(string fileName, string extension = "json")
    {
        return $"{PathBase}{fileName}.{extension}";
    }
}
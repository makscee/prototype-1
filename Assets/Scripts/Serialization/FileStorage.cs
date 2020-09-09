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

    public static void SaveAudioClipToFile(AudioClip clip, string fileName)
    {
        var data = new float[clip.samples];
        clip.GetData(data, 0);
        using (var file = File.Create(Path(fileName, "dat")))
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

    public static AudioClip GetAudioClipFromFile(string fileName)
    {
        var bytes = File.ReadAllBytes(Path(fileName, "dat"));
        var data = new float[bytes.Length / 4];
        Buffer.BlockCopy(bytes, 0, data, 0, bytes.Length);
        var result = AudioClip.Create("clip", data.Length, 1, 44100, false);
        result.SetData(data, 0);
        return result;
    }

    static string Path(string fileName, string extension = "json")
    {
        return $"{PathBase}{fileName}.{extension}";
    }
}
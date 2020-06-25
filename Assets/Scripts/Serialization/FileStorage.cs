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

    static string Path(string fileName)
    {
        return $"{PathBase}{fileName}.json";
    }
}
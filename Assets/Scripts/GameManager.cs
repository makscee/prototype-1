﻿using System;
using UnityEngine;
 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    const string GameStateFileName = "game";
    [SerializeField] string jsonGameState;
    public static Action OnNextFrame;

    void OnEnable()
    {
        Instance = this; 
        OnNextFrame += LoadFromMemoryOrFile;
        PixelDriver.Add(PixelRoad.Checkerboard(Color.white, new Color(0.97f, 0.97f, 0.97f)).SetWeight(0.3f));
        PixelDriver.Add(PixelRoad.NodeBackground().SetWeight(0.1f));
    }
    void OnDisable()
    {
        SaveState();
        ClearField();
    }

    void Update() 
    {
        GlobalPulse.Update();
        Animator.Update();
        PixelDriver.Update();
        OnNextFrame?.Invoke();
        OnNextFrame = null;
    }

    public void SaveState()
    {
        jsonGameState = GameSerialized.Create().ToJson();
        Debug.Log($"Save state: {jsonGameState}");
    }

    public void SaveGameToFile()
    {
        SaveState();
        FileStorage.SaveJsonToFile(jsonGameState, GameStateFileName);
    }

    public void LoadFromMemoryOrFile()
    {
        if (!string.IsNullOrEmpty(jsonGameState)) 
            LoadSavedState();
        else LoadGameFromFile();
    }

    public void LoadGameFromFile()
    {
        jsonGameState = FileStorage.LoadGameFromFile(GameStateFileName);
        LoadSavedState();
    }

    public void LoadSavedState()
    {
        Debug.Log($"Loading json: {jsonGameState}");
        ClearField();
        GameSerialized.Create(jsonGameState).Deserialize();
    }

    public void ClearField()
    {
        foreach (var block in FieldMatrix.GetAllAsList())
            block.Destroy();
    }
}

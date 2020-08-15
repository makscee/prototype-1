﻿using System;
using UnityEngine;
 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    static readonly string GameStateFileName = "game";
    [SerializeField] string JsonGameState;
    public static Action OnNextFrame;
    public enum StartState
    {
        Game, WaveEditor
    }

    public StartState startState;

    void OnEnable()
    {
        Instance = this; 
        OnNextFrame += LoadGameFromFile;
        for (var i = 0; i < 30; i++)
        {
            for (var j = 0; j < 30; j++)
            {
                var c = (i + j) % 2 == 0 ? Color.white : new Color(0.97f, 0.97f, 0.97f);
                PixelFieldMatrix.Show(i - 15, j - 15, c);
            }
        }
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
        OnNextFrame?.Invoke();
        OnNextFrame = null;
    }

    public void SaveState()
    {
        JsonGameState = GameSerialized.Create().ToJson();
        Debug.Log($"Save state: {JsonGameState}");
    }

    public void SaveGameToFile()
    {
        SaveState();
        FileStorage.SaveJsonToFile(JsonGameState, GameStateFileName);
    }

    public void LoadGameFromFile()
    {
        JsonGameState = FileStorage.LoadGameFromFile(GameStateFileName);
        LoadSavedState();
    }

    public void LoadSavedState()
    {
        Debug.Log($"Loading json: {JsonGameState}");
        ClearField();
        GameSerialized.Create(JsonGameState).Deserialize();
    }

    public void ClearField()
    {
        foreach (var block in FieldMatrix.GetAllAsList())
            block.Destroy();
    }
}

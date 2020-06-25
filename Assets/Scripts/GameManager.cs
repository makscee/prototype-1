using System;
using UnityEngine;
 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    static readonly string GameStateFileName = "game";
    [SerializeField] string JsonGameState;
    public static Action AfterServiceObjectsInitialized;
    void OnEnable()
    {
        Instance = this;
        if (JsonGameState.Length > 0)
        {
            AfterServiceObjectsInitialized += LoadSavedState;
        }
        else
        {
            AfterServiceObjectsInitialized += LoadGameFromFile;
        }
    }
    void OnDisable()
    {
        SaveState();
        ClearField();
        ServiceObjectsInitialized = false; 
    }

    void Update()
    {
        GlobalPulse.Update();
        Animator.Update();
        CheckServiceObjectsInit();
    }

    public static bool ServiceObjectsInitialized;
    void CheckServiceObjectsInit()
    {
        if (ServiceObjectsInitialized) return;
        if (SharedObjects.Instance != null && Prefabs.Instance != null)
        {
            ServiceObjectsInitialized = true;
            AfterServiceObjectsInitialized();
        }
    }

    public void SaveState()
    {
        JsonGameState = GameSerialized.Create().ToJson();
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
        GameSerialized.Create(JsonGameState).Deserialize();
    }

    public void ClearField()
    {
        foreach (var block in FieldMatrix.GetAllAsList())
        {
            if (block.GetType() != typeof(Block)) continue;
            block.Destroy();
        }
    }
}

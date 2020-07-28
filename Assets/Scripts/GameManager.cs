using System;
using UnityEngine;
 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    static readonly string GameStateFileName = "game";
    [SerializeField] string JsonGameState;
    static Action _afterServiceObjectsInitialized;

    public static void InvokeAfterServiceObjectsInitialized(Action action)
    {
        if (ServiceObjectsInitialized) action();
        else _afterServiceObjectsInitialized += action;
    }

    void OnEnable()
    {
        Instance = this;
        if (JsonGameState.Length > 0)
        {
            InvokeAfterServiceObjectsInitialized(LoadSavedState);
        }
        else
        {
            InvokeAfterServiceObjectsInitialized(LoadGameFromFile);
        }
    }
    void OnDisable()
    {
        SaveState();
        BlockEditor.DeselectCurrent();
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
            _afterServiceObjectsInitialized?.Invoke();
            _afterServiceObjectsInitialized = null; 
        }
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

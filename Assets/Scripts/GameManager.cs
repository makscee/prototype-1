using System;
using UnityEngine;
 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] string JsonGameState;
    public static Action AfterServiceObjectsInitialized;
    void OnEnable()
    {
        Instance = this;
        if (JsonGameState.Length > 0)
        {
            AfterServiceObjectsInitialized += LoadSavedState;
        }
    }
    void OnDisable()
    {
        SaveState();
        ClearField();
        _serviceObjectsInited = false; 
    }

    void Update()
    {
        GlobalPulse.Update();
        CheckServiceObjectsInit();
    }

    bool _serviceObjectsInited;
    void CheckServiceObjectsInit()
    {
        if (_serviceObjectsInited) return;
        if (SharedObjects.Instance != null && Prefabs.Instance != null)
        {
            _serviceObjectsInited = true;
            AfterServiceObjectsInitialized();
        }
    }

    public void SaveState()
    {
        JsonGameState = GameSerialized.Create().ToJson();
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

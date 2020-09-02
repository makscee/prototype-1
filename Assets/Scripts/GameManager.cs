using System;
using UnityEngine;
 
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    static readonly string GameStateFileName = "game";
    [SerializeField] string JsonGameState;
    public static Action OnNextFrame;

    void OnEnable()
    {
        Instance = this; 
        OnNextFrame += LoadGameFromFile;
        PixelDriver.Add(PixelRoad.Checkerboard(Color.white, new Color(0.97f, 0.97f, 0.97f)).SetWeight(0.1f));
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

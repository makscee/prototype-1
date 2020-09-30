using System;
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

    public void LoadFromMemoryOrFile()
    {
        if (!string.IsNullOrEmpty(jsonGameState)) 
            LoadSavedState();
        else LoadGameFromFile();
    }

    public void SaveState()
    {
        jsonGameState = GameSerialized.Create().ToJson();
        Debug.Log($"Save state: {jsonGameState}");
    }

    public void LoadSavedState()
    {
        Debug.Log($"Loading json: {jsonGameState}");
        ClearField();
        if (jsonGameState == "{}")
        {
            var defaultJson = Resources.Load<TextAsset>("default_game").text;
            GameSerialized.Create(defaultJson).Deserialize();
        }
        else GameSerialized.Create(jsonGameState).Deserialize();
    }

    public void SaveGameToFile()
    {
        SaveState();
        FileStorage.SaveJsonToFile(jsonGameState, GameStateFileName);
        foreach (var root in Roots.Root.Values)
            FileStorage.SaveAudioClipToFile(root.slicedClip, root.block.rootId);
    }

    public void LoadGameFromFile()
    {
        jsonGameState = FileStorage.LoadGameFromFile(GameStateFileName);
        LoadSavedState();
        foreach (var root in Roots.Root.Values)
        {
            var clip = SlicedAudioClip.CreateFromFile(root.block.rootId);
            if (clip != null) root.slicedClip = clip;
        }
    }

    public void ClearField()
    {
        foreach (var block in FieldMatrix.GetAllAsList())
            block.Destroy();
    }
}

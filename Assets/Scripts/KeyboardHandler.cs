using UnityEngine;

public class KeyboardHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameManager.Instance.LoadSavedState();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            GameManager.Instance.ClearField();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameManager.Instance.SaveGameToFile();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameManager.Instance.LoadGameFromFile();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log(GameSerialized.Create().ToJson());
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            FileStorage.SaveAudioClipToFile(Roots.Blocks[0].soundsPlayer.Clip, "clip1");
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            var clip = FileStorage.GetAudioClipFromFile("clip1");
            Debug.Log($"{clip.samples}");
            Roots.Blocks[0].soundsPlayer.Clip = clip;
        }
    }
}
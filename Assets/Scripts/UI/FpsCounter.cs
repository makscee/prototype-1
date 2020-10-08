using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour
{
    [SerializeField] Text text;

    readonly int[] _pastValues = new int[25];
    int _curInd;
    void Update()
    {
        _pastValues[_curInd] = Mathf.RoundToInt(1 / Time.deltaTime);
        _curInd = (_curInd + 1) % _pastValues.Length;
        var value = _pastValues.Sum() / _pastValues.Length;
        text.text = Mathf.RoundToInt(value).ToString();
    }
}
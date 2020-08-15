using System;
using UnityEngine;

public class RollingButton : MonoBehaviour
{
    const float RollDuration = 0.3f, ShowForNSeconds = 3f;
    static readonly Vector3 RollOffset = Vector3.left;

    public Action OnClick;
    bool _interactable;
    bool _shown;

    Transform _parent;
    Vector3 _offset;

    float _hideTimer;

    void Update()
    {
        transform.position = _parent.position + _offset;
        _hideTimer -= Time.deltaTime;
        if (_hideTimer < 0f && _interactable) Hide();
    }
    
    public void Show()
    {
        if (_shown)
        {
            _hideTimer = ShowForNSeconds;
            return;
        }

        _shown = true;
        _hideTimer = ShowForNSeconds;
        gameObject.SetActive(true);
        Animator.Interpolate(0f, 1f, RollDuration)
            .PassDelta(v => _offset += RollOffset * v).WhenDone(() =>
            {
                _interactable = true;
            });
    }

    void Hide()
    {
        _interactable = false;
        Animator.Interpolate(1f, 0f, RollDuration)
            .PassDelta(v => _offset += RollOffset * v).WhenDone(() =>
            {
                gameObject.SetActive(false);
                _shown = false;
            });
    }

    public void Click()
    {
        if (!_interactable) return;
        OnClick?.Invoke();
        Hide();
    }

    public static RollingButton Create(Transform parent)
    {
        var button = Instantiate(Prefabs.Instance.rollingButton, SharedObjects.Instance.MidCanvas.transform)
            .GetComponent<RollingButton>();
        button._parent = parent;
        button.gameObject.SetActive(false);
        return button;
    }
}
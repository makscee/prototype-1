using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WaveEditor : RootEditorScreen, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    public RawImage Top, Mid, Bot;
    public RectTransform TopRect, MidRect, BotRect;
    public WaveEditorAddRecordButton AddRecordButton;

    Texture2D _currentTexture;
    [SerializeField]RectTransform rect;
    [SerializeField]int width, height;
    float _maxHeight;

    float _topPicker = 0.4f, _bottomPicker = 0.7f;
    const float MinThreshold = 0.002f;

    public float TopPicker
    {
        get => _topPicker;
        set => _topPicker = Mathf.Min(Mathf.Clamp(value, 0f, 1f), _bottomPicker - MinThreshold);
    }

    public float BottomPicker
    {
        get => _bottomPicker;
        set => _bottomPicker = Mathf.Max(Mathf.Clamp(value, 0f, 1f), _topPicker + MinThreshold);
    }

    AudioClip _lastClip;
    public AudioClip Clip => RootBlock.soundsPlayer.Clip;

    public void RefreshTexture()
    {
        _currentTexture = WaveTextureProvider.TextureFrom(Clip, width, height);
        Top.texture = _currentTexture;
        Mid.texture = _currentTexture;
        Bot.texture = _currentTexture;
        
        _topPicker = (float) RootBlock.soundsPlayer.Configs[currentDirection].SelectFrom / Clip.samples;
        BottomPicker = (float) RootBlock.soundsPlayer.Configs[currentDirection].SelectTo / Clip.samples;
    }

    public override void Select()
    {
        gameObject.SetActive(true);
    }

    public override void Deselect()
    {
        gameObject.SetActive(false);
    }

    void Awake()
    {
        var rect = this.rect.rect;
        width = Mathf.RoundToInt(rect.width);
        _maxHeight = transform.parent.GetComponent<RectTransform>().rect.height;
        height = Mathf.RoundToInt(_maxHeight);
        this.rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _maxHeight);
        
        AddRecordButton.OnDown = StartRecording;
        AddRecordButton.OnUp = EndRecording;

        _currentTexture = WaveTextureProvider.TextureFrom(Clip, width, height);
        _lastClip = Clip;
        Top.texture = _currentTexture;
        Mid.texture = _currentTexture;
        Bot.texture = _currentTexture;
    }
    
    bool _isRecording;
    void StartRecording()
    {
        _isRecording = true;
        Recorder.StartRecording();
    }

    void RecordingUpdate()
    {
        if (!_isRecording) return;

        height = Mathf.RoundToInt(_maxHeight * (Clip.length / (Clip.length + Recorder.RecordingLength)));
    }

    void EndRecording()
    {
        _isRecording = false;
        height = Mathf.RoundToInt(_maxHeight);
        Recorder.EndRecording();
        if (Recorder.GetLastRecording() == null) return;
        
        RootBlock.soundsPlayer.Clip = ClipMaker.Add(Clip, Recorder.GetLastRecording());

        RefreshTexture();
        var top = 1f - (float) Recorder.GetLastRecording().samples / Clip.samples;
        _topPicker = top;
        BottomPicker = 1f;
    }

    void Update()
    {
        if (!_isRecording && Clip != _lastClip)
        {
            _lastClip = Clip;
            RefreshTexture();
        }
        RecordingUpdate();
        
        var topDist = height * TopPicker;
        var botDist = height * BottomPicker;
        
        TopRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, topDist);
        MidRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, topDist, botDist - topDist);
        BotRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, botDist, height - botDist);

        var topRect = Top.uvRect;
        topRect.height = TopPicker;
        topRect.y = -TopPicker;
        Top.uvRect = topRect;
        
        var midRect = Mid.uvRect;
        midRect.height = BottomPicker - TopPicker;
        midRect.y = 1 - BottomPicker;
        Mid.uvRect = midRect;
        
        var botRect = Bot.uvRect;
        botRect.height = 1 - BottomPicker;
        Bot.uvRect = botRect;
        
        Apply();
    }

    bool _dragBeganOnTop, _dragHorizontal;
    RectTransform _curDragged;
    int _cutFrom, _cutTo;
    float _dragStartTime;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragStartTime = Time.time;
        _dragHorizontal = Mathf.Abs(eventData.delta.x) > Mathf.Abs(eventData.delta.y);
        var scaledPosition = SharedObjects.Instance.configCanvas.ScaledScreenPos(eventData.position);
        if (_dragHorizontal)
        {
            var topPos = (1 - TopPicker) * height;
            var bottomPos = (1 - BottomPicker) * height;
            if (scaledPosition.y > topPos)
            {
                _curDragged = Top.GetComponentInParent<RectTransform>();
                _cutFrom = 0;
                _cutTo = Mathf.RoundToInt(TopPicker * Clip.samples);
            }
            else if (scaledPosition.y < topPos && scaledPosition.y > bottomPos)
            {
                _curDragged = Mid.GetComponentInParent<RectTransform>();
                _cutFrom = Mathf.RoundToInt(TopPicker * Clip.samples);
                _cutTo = Mathf.RoundToInt(BottomPicker * Clip.samples);
            }
            else
            {
                _curDragged = Bot.GetComponentInParent<RectTransform>();
                _cutFrom = Mathf.RoundToInt(BottomPicker * Clip.samples);
                _cutTo = Clip.samples;
            }
        }
        else
        {
            var selectMiddle = 1 - (BottomPicker + TopPicker) / 2;
            var midPos = selectMiddle * height;
            _dragBeganOnTop = scaledPosition.y > midPos;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        var scaledDelta = SharedObjects.Instance.configCanvas.ScaledScreenPos(eventData.delta);
        if (_dragHorizontal)
        {
            _curDragged.anchoredPosition += new Vector2(scaledDelta.x, 0);
        }
        else
        {
            var selectMiddle = (BottomPicker + TopPicker) / 2;
            var midPos = selectMiddle * height;
            var delta = scaledDelta.y / height;
            if (_dragBeganOnTop)
                TopPicker -= delta;
            else
                BottomPicker -= delta;
            Apply();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragHorizontal)
        {
            var curDraggedX = _curDragged.anchoredPosition.x;
            if (Mathf.Abs(curDraggedX) < Screen.width / 4)
            {
                Animator.Interpolate(curDraggedX, 0, 0.1f).Type(InterpolationType.InvSquare)
                    .PassDelta(v => _curDragged.anchoredPosition += new Vector2(v, 0));
            }
            else
            {
                var speed = (eventData.position.x - Screen.width / 2) / (Time.time - _dragStartTime);
                const float animationTime = 0.2f;
                Animator.Interpolate(curDraggedX,  curDraggedX + speed * animationTime, animationTime).Type(InterpolationType.Linear)
                    .PassDelta(v => _curDragged.anchoredPosition += new Vector2(v, 0)).WhenDone(() =>
                    {
                        CutClip(_cutFrom, _cutTo);
                        _curDragged.anchoredPosition = new Vector3(0, _curDragged.position.y);
                    });
            }
        }
        Apply();
    }

    void CutClip(int from, int to)
    {
        RootBlock.soundsPlayer.Clip = ClipMaker.Add(
            ClipMaker.Make(Clip, 0, from, 44100),
            ClipMaker.Make(Clip, to, Clip.samples - 1, 44100)
        );
        foreach (var soundConfig in RootBlock.soundsPlayer.Configs)
        {
            soundConfig.CutoutAdjust(from, to);
        }
        RefreshTexture();
    }

    RootBlock RootBlock => rootEditor.RootBlock;
    [SerializeField]int currentDirection;

    public void SelectDirection(int num)
    {
        currentDirection = num;
        _topPicker = (float)RootBlock.soundsPlayer.Configs[num].SelectFrom / Clip.samples;
        _bottomPicker = (float)RootBlock.soundsPlayer.Configs[num].SelectTo / Clip.samples;
    }

    void Apply()
    {
        RootBlock.soundsPlayer.Configs[currentDirection].SelectFrom = Mathf.RoundToInt(_topPicker * Clip.samples);
        RootBlock.soundsPlayer.Configs[currentDirection].SelectTo = Mathf.RoundToInt(_bottomPicker * Clip.samples);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Play();
    }

    void Play()
    {
        RootBlock.soundsPlayer.Play(currentDirection);
    }
}
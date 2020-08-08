using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WaveEditor : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    public static WaveEditor Instance;
    
    public RawImage Top, Mid, Bot;
    public RectTransform TopRect, MidRect, BotRect;
    public SplitSlider Volume, Rate;
    public WaveEditorAddRecordButton AddRecordButton;

    [SerializeField]AudioClip clip;
    [SerializeField] Painter[] pbBackgrounds;
    [SerializeField] PulseBlock[] pulseBlocks;
    Texture2D _currentTexture;
    RectTransform _rect;
    Palette _palette;
    CanvasScaler _canvasScaler;
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

    public AudioClip Clip
    {
        get => clip;
        set
        {
            clip = value;
            _currentTexture = WaveTextureProvider.TextureFrom(Clip, width, height);
            Top.texture = _currentTexture;
            Mid.texture = _currentTexture;
            Bot.texture = _currentTexture;
        }
    }

    void Awake()
    {
        _canvasScaler = GetComponentInParent<CanvasScaler>();
        _palette = GetComponent<Palette>();
        _rect = GetComponent<RectTransform>();
        transform.parent.gameObject.GetComponent<Canvas>().enabled = true;
        Volume.onClick = Play;
        Rate.onClick = Play;
        AddRecordButton.OnDown += StartRecording;
        AddRecordButton.OnUp += EndRecording;
    }

    void OnEnable()
    {
        Instance = this;
        var rect = _rect.rect;
        width = Mathf.RoundToInt(rect.width);
        _maxHeight = transform.parent.GetComponent<RectTransform>().rect.height;
        height = Mathf.RoundToInt(_maxHeight);
        _rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _maxHeight);

        _currentTexture = WaveTextureProvider.TextureFrom(Clip, width, height);
        Top.texture = _currentTexture;
        Mid.texture = _currentTexture;
        Bot.texture = _currentTexture;
        GameManager.InvokeAfterServiceObjectsInitialized(() =>
        {
            SelectPulseBlock(0); 
            SelectDirection(0);
        });
    }


    float _recordingLength;
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
        Clip = ClipMaker.Add(Clip, Recorder.GetLastRecording());
        
        var top = (float) currentPulseBlock.SoundsPlayer.Configs[currentDirection].SelectFrom / Clip.samples;
        var bot = (float) currentPulseBlock.SoundsPlayer.Configs[currentDirection].SelectTo / Clip.samples;
        _topPicker = top;
        BottomPicker = bot;
    }

    void Update()
    {
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
        if (_dragHorizontal)
        {
            var topPos = (1 - TopPicker) * height;
            var bottomPos = (1 - BottomPicker) * height;
            if (eventData.position.y > topPos)
            {
                _curDragged = Top.GetComponentInParent<RectTransform>();
                _cutFrom = 0;
                _cutTo = Mathf.RoundToInt(TopPicker * Clip.samples);
            }
            else if (eventData.position.y < topPos && eventData.position.y > bottomPos)
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
            _dragBeganOnTop = eventData.position.y > midPos;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragHorizontal)
        {
            _curDragged.anchoredPosition += new Vector2(eventData.delta.x * _canvasScaler.referenceResolution.x / Screen.width, 0);
        }
        else
        {
            var selectMiddle = (BottomPicker + TopPicker) / 2;
            var midPos = selectMiddle * height;
            var delta = eventData.delta.y / height;
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
                        Clip = ClipMaker.Add(
                            ClipMaker.Make(Clip, 0, _cutFrom, 44100),
                            ClipMaker.Make(Clip, _cutTo, Clip.samples - 1, 44100)
                        );
                        // currentPulseBlock.SoundsPlayer.Configs[currentDirection].CutoutAdjust(_cutFrom, _cutTo);
                        foreach (var pulseBlock in pulseBlocks)
                        {
                            foreach (var soundConfig in pulseBlock.SoundsPlayer.Configs)
                            {
                                soundConfig.CutoutAdjust(_cutFrom, _cutTo);
                            }
                        }

                        _curDragged.position = new Vector3(0, _curDragged.position.y);
                        var top = (float) currentPulseBlock.SoundsPlayer.Configs[currentDirection].SelectFrom / Clip.samples;
                        var bot = (float) currentPulseBlock.SoundsPlayer.Configs[currentDirection].SelectTo / Clip.samples;
                        _topPicker = top;
                        BottomPicker = bot;
                    });
            }
        }
        Apply();
    }

    [SerializeField]PulseBlock currentPulseBlock;
    [SerializeField]int currentDirection;

    public void SelectPulseBlock(int num)
    {
        currentPulseBlock = pulseBlocks[num];
        _palette.copyOf = currentPulseBlock.palette;
        for (var i = 0; i < 4; i++)
        {
            pbBackgrounds[i].MultiplyBy = i == num ? Vector3.one : Vector3.zero;
        }
        SelectDirection(currentDirection);
    }

    public void SelectDirection(int num)
    {
        currentDirection = num;
        _topPicker = (float)currentPulseBlock.SoundsPlayer.Configs[num].SelectFrom / Clip.samples;
        _bottomPicker = (float)currentPulseBlock.SoundsPlayer.Configs[num].SelectTo / Clip.samples;
        Volume.value = currentPulseBlock.SoundsPlayer.Configs[num].Volume;
        Rate.value = (currentPulseBlock.SoundsPlayer.Configs[num].Rate - Rate.Min) / (Rate.Max - Rate.Min);
    }

    void Apply()
    {
        currentPulseBlock.SoundsPlayer.Configs[currentDirection].SelectFrom = Mathf.RoundToInt(_topPicker * Clip.samples);
        currentPulseBlock.SoundsPlayer.Configs[currentDirection].SelectTo = Mathf.RoundToInt(_bottomPicker * Clip.samples);
        currentPulseBlock.SoundsPlayer.Configs[currentDirection].Volume = Volume.value;
        currentPulseBlock.SoundsPlayer.Configs[currentDirection].Rate = Rate.MultipliedValueInt;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Play();
    }

    void Play()
    {
        currentPulseBlock.SoundsPlayer.Play(currentDirection);
    }
}
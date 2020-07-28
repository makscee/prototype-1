using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WaveEditor : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    public static WaveEditor Instance;
    
    public RawImage Top, Mid, Bot;
    public RectTransform TopRect, MidRect, BotRect;
    
    AudioSource _audioSource;
    public AudioClip clip;
    [SerializeField] Painter[] arrowBackgrounds;
    [SerializeField] Painter[] pbBackgrounds;
    [SerializeField] PulseBlock[] pulseBlocks;
    WaveTextureProvider _textureProvider;
    RectTransform _rect;
    Palette _palette;
    int width, height;

    float _topPicker = 0.4f, _bottomPicker = 0.7f;

    public float TopPicker
    {
        get => _topPicker;
        set => _topPicker = Mathf.Clamp(value, 0f, 1f);
    }
    public float BottomPicker
    {
        get => _bottomPicker;
        set => _bottomPicker = Mathf.Clamp(value, 0f, 1f);
    }

    void Awake()
    {
        _palette = GetComponent<Palette>();
        _rect = GetComponent<RectTransform>();
        _audioSource = GetComponent<AudioSource>();
        transform.parent.gameObject.GetComponent<Canvas>().enabled = true;
    }

    void Start()
    {
        transform.parent.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        Instance = this;
        var rect = _rect.rect;
        width = Mathf.RoundToInt(rect.width);
        height = Mathf.RoundToInt(rect.height);
        _textureProvider = new WaveTextureProvider(clip, width, height);
        Top.texture = _textureProvider.GetTexture();
        Mid.texture = _textureProvider.GetTexture();
        Bot.texture = _textureProvider.GetTexture();
        GameManager.InvokeAfterServiceObjectsInitialized(() =>
        {
            SelectPulseBlock(0); 
            SelectDirection(0);
        });
    }

    void Update()
    {
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
    }

    public void OnDrag(PointerEventData eventData)
    {
        var selectMiddle = (BottomPicker + TopPicker) / 2;
        var midPos = selectMiddle * height;
        var delta = eventData.delta.y / height;
        if (_dragBeganOnTop)
            TopPicker -= delta;
        else BottomPicker -= delta;
        Apply();
    }

    bool _dragBeganOnTop, _dragging;
    public void OnBeginDrag(PointerEventData eventData)
    {
        var selectMiddle = 1 - (BottomPicker + TopPicker) / 2;
        var midPos = selectMiddle * height;
        var delta = eventData.delta.y / height;
        _dragBeganOnTop = eventData.position.y > midPos;
        _dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
        _currentPulseBlock.SoundsPlayer.Play(_currentDirection);
        Apply();
    }

    PulseBlock _currentPulseBlock;
    int _currentDirection;

    public void SelectPulseBlock(int num)
    {
        _currentPulseBlock = pulseBlocks[num];
        _palette.copyOf = _currentPulseBlock.palette;
        for (var i = 0; i < 4; i++)
        {
            pbBackgrounds[i].MultiplyBy = i == num ? Vector3.one : Vector3.zero;
        }
        SelectDirection(_currentDirection);
    }

    public void SelectDirection(int num)
    {
        _currentDirection = num;
        for (var i = 0; i < 4; i++)
        {
            arrowBackgrounds[i].MultiplyBy = i == num ? Vector3.one : Vector3.zero;
        }

        _topPicker = _currentPulseBlock.SoundsPlayer.Configs[num].SelectFrom;
        _bottomPicker = _currentPulseBlock.SoundsPlayer.Configs[num].SelectTo;
    }

    void Apply()
    {
        _currentPulseBlock.SoundsPlayer.Configs[_currentDirection].SelectFrom = _topPicker;
        _currentPulseBlock.SoundsPlayer.Configs[_currentDirection].SelectTo = _bottomPicker;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_dragging)
            _audioSource.PlayOneShot(_currentPulseBlock.SoundsPlayer.AudioSources[_currentDirection].clip);
    }
}
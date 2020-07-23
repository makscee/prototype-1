using System;
using UnityEngine;
using UnityEngine.UI;

public class BindVisual : MonoBehaviour
{
    public IBindable first, second;
    RectTransform _rectTransform;

    const float MinWidth = 0.7f, MaxWidth = 0.9f;
    float MaxLength;

    public Bind bind;

    Painter _painter;
    Image _image;
    GameObject _shadowParticles;
    void Awake()
    {
        _image = GetComponent<Image>();
        _painter = GetComponent<Painter>();
        _rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        InitShadowParticles();
    }

    void InitShadowParticles()
    {
        if (!(bind.First is Block block) || !(bind.Second is Block))
            return;
        _shadowParticles = Instantiate(Prefabs.Instance.BindShadowParticles);
        _shadowParticles.transform.position = new Vector3(block.X, block.Y);
        var dir = Utils.DirFromCoords(bind.Offset);
        _shadowParticles.transform.Rotate(Vector3.right, dir * 90);
    }

    public static void Create(Bind bind)
    {
        if (!(bind.First is Block block) || block.pulseBlock == null)
            return;
        
        var b = Instantiate(Prefabs.Instance.BindVisual, SharedObjects.Instance.FrontCanvas.transform).GetComponent<BindVisual>();
        b.first = bind.First;
        b.second = bind.Second;
        b.MaxLength = bind.BreakDistance > -1 ? bind.BreakDistance : 3;
        b.bind = bind;
        
        b._painter.subscribedTo = block.insidePainter;
    }

    void OnDisable()
    {
        Destroy(gameObject);
        Destroy(_shadowParticles);
    }

    void Update()
    {
        if (BindMatrix.GetBind(first, second) != bind)
        {
            Destroy(gameObject);
            return;
        }

        var imageActive = true;
        if (bind.First is BindableMonoBehavior mFirst) imageActive = mFirst.isActiveAndEnabled;
        if (bind.Second is BindableMonoBehavior mSecond) imageActive = imageActive && mSecond.isActiveAndEnabled;
        if (_image.enabled != imageActive)
        {
            _image.enabled = imageActive;
            if (_shadowParticles != null)
                _shadowParticles.SetActive(!imageActive);
        }
        
        bind.Update();
        
        var firstPosition = first.GetPosition();
        var dir = (second.GetPosition() - firstPosition) / 2;
        transform.position = firstPosition + dir;
        
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        var length = dir.magnitude * 2 - Block.BlockSide / 3;
        var width = Mathf.Lerp(MaxWidth, MinWidth, length / MaxLength);
        _rectTransform.sizeDelta = new Vector2(width, length);
    }
}
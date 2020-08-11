using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PulseBlock : BlockOld
{
    public SoundsPlayer SoundsPlayer;
    public Palette palette;
    public GameObject Background;
    Painter _backgroundPainter;
    public int Dir;
    RawImage _bgRawImg;
    public PulseBlock()
    {
        StepNumber = 0;
    }

    protected override void OnEnable()
    {
        UpdateCoordsFromTransformPosition();
        // FieldMatrix.Add(X, Y, this); 
        base.OnEnable();
    }

    protected override void Init()
    {
        _bgRawImg = Background.GetComponent<RawImage>();
        // GameManager.InvokeAfterServiceObjectsInitialized(PostEnableInit);
    }

    void PostEnableInit()
    {
        BindMatrix.AddBind(this, StaticAnchor.Create(GetPosition()), Vector2.zero, Bind.PulseBlockBindStrength);
        var v = new Vector2(X, Y) - new Vector2(PulseBlockCenter.Instance.X, PulseBlockCenter.Instance.Y);
        BindMatrix.AddBind(PulseBlockCenter.Instance, this, v, Bind.BlockBindStrength);
        PulseBlockCenter.Instance.PulseBlocks[Utils.DirFromCoords(X, Y)] = this;
        // var button = RollingButton.Create(transform);
        // button.OnClick = () =>
        // {
        //     ShowConfigRack();
        //     BlockEditor.DeselectCurrent();
        // };
        // onTap += button.Show;
    }

    float _bgDesiredAlpha = 0f, _bgAlphaChangeSpeed = 1.5f;
    protected override void Update()
    {
        base.Update();
        var c = _bgRawImg.color;
        if (c.a > _bgDesiredAlpha)
        {
            c.a = Math.Max(c.a - Time.fixedDeltaTime * _bgAlphaChangeSpeed, _bgDesiredAlpha);
            _bgRawImg.color = c;
        }
    }

    public override bool IsAnchor()
    {
        return true;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            insidePainter.NumInPalette = 3;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (cancelDrag)
        {
            base.OnEndDrag(eventData);
            return;
        }
        base.OnEndDrag(eventData);

        if (!masked) Pulse();
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            insidePainter.NumInPalette = 2;
        }
    }

    public override void CancelDrag()
    {
        base.CancelDrag();
        insidePainter.NumInPalette = 2;
    }

    protected override void OnMiddleClick()
    {
    }

    public void OnPulseDeadEnd(int dir)
    {
        SoundsPlayer.Play(dir);
        if (dir == Dir)
        {
            var c = _bgRawImg.color;
            c.a = 0.6f;
            _bgRawImg.color = c;
        }
    }

    public override void ReceivePulse(BlockOld from)
    {
        insidePainter.NumInPalette = 3;
        GlobalPulse.SubscribeToNext(PassPulse);
    }

    public override void PassPulse()
    {
        foreach (var bind in BindMatrix.GetAllAdjacentBinds(this))
        {
            if (bind.First == this && bind.Second is BlockOld block)
            {
                block.ReceivePulse(this);
            }
        }
        insidePainter.NumInPalette = 2;
    }

    void Pulse()
    {
        ReceivePulse(null);
    }

    public override void RefreshStepNumber()
    {
        
    }
}

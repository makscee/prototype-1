using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PulseBlock : Block
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

    void OnEnable()
    {
        UpdateCoordsFromTransformPosition();
        PulseBlock = this;
        FieldMatrix.Add(X, Y, this); 
        _bgRawImg = Background.GetComponent<RawImage>();
        GameManager.AfterServiceObjectsInitialized += PostEnableInit;
    }

    protected override void SetupPalette()
    {
        palette = new Palette(Utils.DirFromCoords(X, Y));
        _backgroundPainter = Background.GetComponent<Painter>();
        _backgroundPainter.palette = palette;
        painter.palette = palette;
        insidePainter.palette = palette;
        _backgroundPainter.NumInPalette = 0;
        painter.NumInPalette = 3;
        insidePainter.NumInPalette = 2;
    }

    void PostEnableInit()
    {
        BindMatrix.AddBind(this, StaticAnchor.Create(GetPosition()), Vector2.zero, Bind.PulseBlockBindStrength);
        var v = new Vector2(X, Y) - new Vector2(PulseBlockCenter.Instance.X, PulseBlockCenter.Instance.Y);
        BindMatrix.AddBind(PulseBlockCenter.Instance, this, v, Bind.BlockBindStrength);
        PulseBlockCenter.Instance.PulseBlocks[Utils.DirFromCoords(X, Y)] = this;
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

    public override bool IsAnchored()
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
        base.OnEndDrag(eventData);
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            insidePainter.NumInPalette = 2;
        }
    }

    protected override void OnLeftClick()
    {
        base.OnLeftClick();
        HideAllConfigRacks();
        var cam = SharedObjects.Instance.Camera.transform;
        Animator.Invoke(() => SoundsPlayer.ConfigRacksSetActive(true)).In(0.2f);
        Animator.Interpolate(cam.position, transform.position, 0.3f)
            .PassDelta(v => cam.position += (Vector3)v).Type(InterpolationType.InvSquare);
    }

    public static void HideAllConfigRacks()
    {
        foreach (var pulseBlock in PulseBlockCenter.Instance.PulseBlocks)
        {
            if (pulseBlock == null) break;
            pulseBlock.SoundsPlayer.ConfigRacksSetActive(false);
        }
    }

    protected override bool TryCreateBlock()
    {
        var result = base.TryCreateBlock();
        if (!result) Pulse();
        return result;
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

    public override void ReceivePulse(Block from)
    {
        insidePainter.NumInPalette = 3;
        GlobalPulse.SubscribeToNext(PassPulse);
    }

    public override void PassPulse()
    {
        foreach (var bind in BindMatrix.GetAllAdjacentBinds(this))
        {
            if (bind.First == this && bind.Second is Block block)
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

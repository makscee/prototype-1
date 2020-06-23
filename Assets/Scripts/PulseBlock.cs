using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PulseBlock : Block
{
    public SoundsPlayer SoundsPlayer;
    public ColorPalette ColorPalette;
    public GameObject Background;
    public int Dir;
    RawImage _bgRawImg;
    public PulseBlock()
    {
        StepNumber = 0;
        PulseBlock = this;
    }

    void OnEnable()
    {
        PulseBlock = this;
        UpdateCoordsFromTransformPosition();
        ColorPalette = new ColorPalette(Utils.DirFromCoords(X, Y));
        ColorPalette.SubscribeGameObject(Background, 0);
        ColorPalette.SubscribeGameObject(gameObject, 3);
        ColorPalette.SubscribeGameObject(inside, 2); 
        FieldMatrix.Add(X, Y, this);
        _bgRawImg = Background.GetComponent<RawImage>();
        GameManager.AfterServiceObjectsInitialized += PostEnableInit;
    }

    protected override void Start()
    {
    }

    void PostEnableInit()
    {
        BindMatrix.AddBind(this, StaticAnchor.Create(GetPosition()), Vector2.zero, Bind.PulseBlockBindStrength);
        var v = new Vector2(X, Y) - new Vector2(PulseBlockCenter.Instance.X, PulseBlockCenter.Instance.Y);
        BindMatrix.AddBind(PulseBlockCenter.Instance, this, v, Bind.BlockBindStrength);
    }

    float _bgDesiredAlpha = 0f, _bgAlphaChangeSpeed = 1.5f;
    protected override void Update()
    {
        base.Update();
        ColorPalette.Update();
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
            ColorPalette.SubscribeGameObject(inside, 3);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ColorPalette.SubscribeGameObject(inside, 2);
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
        ColorPalette.SubscribeGameObject(inside, 3);
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
        ColorPalette.SubscribeGameObject(inside, 2);
    }

    void Pulse()
    {
        ReceivePulse(null);
    }

    public override void RefreshStepNumber()
    {
        
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PulseBlockCenter : Block
{
    public static PulseBlockCenter Instance;
    public PulseBlock[] PulseBlocks = new PulseBlock[4];
    public Palette Palette;
    public AudioClip Clip;
    void OnEnable()
    {
        SetupPalette();
        UpdateCoordsFromTransformPosition();
        FieldMatrix.Add(X, Y, this);
        Instance = this;
        GameManager.InvokeAfterServiceObjectsInitialized(PostEnableInit);
    }

    protected override void SetupPalette()
    {
        Palette = new Palette(4);
        painter.palette = Palette;
        insidePainter.palette = Palette;
        painter.NumInPalette = 3;
        insidePainter.NumInPalette = 2;
    }

    void PostEnableInit()
    {
        BindMatrix.AddBind(this, StaticAnchor.Create(GetPosition()), Vector2.zero, Bind.PulseBlockBindStrength);
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
        base.OnEndDrag(eventData); // todo forbid block creation
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            insidePainter.NumInPalette = 2;
            PassPulse();
        }
    }

    protected override bool TryCreateBlock()
    {
        return false;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
    }

    public override void ReceivePulse(Block from)
    {
    }
    
    protected override void RefreshFieldCircle()
    {
    }

    protected override void ShowNewBlockPlaceholders(bool value)
    {
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

    public override void RefreshStepNumber()
    {
        
    }

    protected override void OnMiddleClick()
    {
    }
}
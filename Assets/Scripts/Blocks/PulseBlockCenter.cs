using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PulseBlockCenter : Block
{
    public static PulseBlockCenter Instance;
    public PulseBlock[] PulseBlocks = new PulseBlock[4];
    public Palette Palette;
    public AudioClip Clip;
    public GameObject CentralRack;
    CentralRack _centralRack;

    protected override void Init()
    {
        _centralRack = CentralRack.GetComponent<CentralRack>();
        UpdateCoordsFromTransformPosition();
        FieldMatrix.Add(X, Y, this);
        Instance = this;
        GameManager.InvokeAfterServiceObjectsInitialized(PostEnableInit);
        onTap = () =>
            {
                _centralRack.Show();
                DarkOverlay.Enable();
                DarkOverlay.OnNextTap += _centralRack.Hide;
            };
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
        if (cancelDrag)
        {
            base.OnEndDrag(eventData);
            return;
        }
        base.OnEndDrag(eventData);
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            insidePainter.NumInPalette = 2;
            PassPulse();
        }
    }

    public override void CancelDrag()
    {
        base.CancelDrag();
        insidePainter.NumInPalette = 2;
    }

    public override void ReceivePulse(Block from)
    {
    }
    
    protected override void RefreshFieldCircle()
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
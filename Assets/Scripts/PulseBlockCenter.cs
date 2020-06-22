using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PulseBlockCenter : Block
{
    public static PulseBlockCenter Instance;
    
    public AudioClip Clip;
    void OnEnable()
    {
        // ColorPalette.SubscribeGameObject(gameObject, 3);
        // ColorPalette.SubscribeGameObject(inside, 2);
        UpdateCoordsFromTransformPosition();
        FieldMatrix.Add(X, Y, this);
        Instance = this;
        GameManager.AfterServiceObjectsInitialized += PostEnableInit;
    }

    void PostEnableInit()
    {
        BindMatrix.AddBind(this, StaticAnchor.Create(GetPosition()), Vector2.zero, Bind.PulseBlockBindStrength);
    }
    
    protected override void Start()
    {
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
            // ColorPalette.SubscribeGameObject(inside, 3);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData); // todo forbid block creation
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // ColorPalette.SubscribeGameObject(inside, 2);
            PassPulse();
        }
    }

    protected override void TryCreateBlock()
    {
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
        // ColorPalette.SubscribeGameObject(inside, 2);
    }

    public override void RefreshStepNumber()
    {
        
    }

    protected override void OnMiddleClick()
    {
    }
}
using UnityEngine;
using UnityEngine.EventSystems;

public class PulseBlock : Block
{
    public const float PulseDelay = 60f / 65f / 4f; // pulse bpm

    void OnEnable()
    {
        ColorPalette.SubscribeGameObject(gameObject, 3);
        ColorPalette.SubscribeGameObject(inside, 2);
        FieldMatrix.Add(0, 0, this);
    }

    protected override void Start()
    {
        base.Start();
        var rect = GetComponent<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BlockSide);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BlockSide);
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
            ColorPalette.SubscribeGameObject(inside, 3);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ColorPalette.SubscribeGameObject(inside, 2);
            Pulse();
        }
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        
    }

    public override void ReceivePulse(Block from)
    {
        ColorPalette.SubscribeGameObject(inside, 3);
        StopCoroutine(nameof(PassCoroutine));
        StartCoroutine(nameof(PassCoroutine));
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
}

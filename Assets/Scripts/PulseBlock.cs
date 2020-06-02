using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PulseBlock : Block
{
    public const float PulseDelay = 60f / 65f / 4f; // pulse bpm

    public PulseBlock()
    {
        StepNumber = 0;
    }

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
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            RefreshStepNumbers();
        }
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

    void RefreshStepNumbers()
    {
        var list = new List<Block>();
        var q = new Queue<Block>();
        q.Enqueue(this);
        list.Add(this);
        Used = true;
        int num = 0;
        while (q.Count > 0)
        {
            var arr = q.ToArray();
            q.Clear();
            foreach (var block in arr)
            {
                block.DisplayText(num.ToString());
                foreach (var bind in BindMatrix.GetAllAdjacentBinds(block))
                {
                    if (bind.First == block && !bind.Second.Used && bind.Second is Block b)
                    {
                       list.Add(b);
                       q.Enqueue(b);
                    }
                }
            }
            num++;
        }

        foreach (var block in list)
        {
            block.Used = false;
        }
    }
}

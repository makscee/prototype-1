using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockView : MonoBehaviour
{
    public Block parent;

    public Painter primaryPainter => VisualBase.Current.primary;
    public Painter secondaryPainter => VisualBase.Current.secondary;
    Dictionary<Bind, BindVisual> _bindVisuals = new Dictionary<Bind, BindVisual>(8);
    public Action onRefresh;

    public BlockVisualBase VisualBase { get; private set; }

    void Update()
    {
        if (!_dirty) return;
        Refresh();
    }

    bool _dirty;
    public void SetDirty()
    {
        _dirty = true;
    }

    void Refresh()
    {
        onRefresh?.Invoke();
        _dirty = false;
    }

    public void SetInitialModel(BlockVisualBase.Model model)
    {
        VisualBase = BlockVisualBase.Create(parent, model);
    }
    
    public void OnBind(Bind bind)
    {
        if (bind.First == parent && BindVisual.Create(bind, out var bindVisual))
            _bindVisuals.Add(bind, bindVisual);
        SetDirty();
    }

    public void OnUnbind(Bind bind)
    {
        if (!_bindVisuals.ContainsKey(bind)) return;
        var bindVisual = _bindVisuals[bind];
        _bindVisuals.Remove(bind);
        if (bindVisual != null)
            bindVisual.Destroy();
        SetDirty();
    }

    void OnDestroy()
    {
        VisualBase.Destroy();
    }
}
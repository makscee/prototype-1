using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockView : MonoBehaviour
{
    public Block parent;

    public Painter primaryPainter, secondaryPainter;
    Dictionary<Bind, BindVisual> _bindVisuals = new Dictionary<Bind, BindVisual>(8);
    public Action onRefresh;

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
    
    public void OnBind(Bind bind)
    {
        if (bind.First == parent && BindVisual.Create(bind, out var bindVisual))
            _bindVisuals.Add(bind, bindVisual);
    }

    public void OnUnbind(Bind bind)
    {
        if (!_bindVisuals.ContainsKey(bind)) return;
        var bindVisual = _bindVisuals[bind];
        _bindVisuals.Remove(bind);
        if (bindVisual != null)
            bindVisual.Destroy();
    }
}
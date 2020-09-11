using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BindMatrix
{
    static readonly Dictionary<IBindable, Dictionary<IBindable, Bind>> Matrix = new Dictionary<IBindable, Dictionary<IBindable, Bind>>();

    public static void AddBind(IBindable first, IBindable second, Vector2 offset, int strength,
        float ropeLength = 0, float breakDistance = -1)
    {
        if (IsBound(first, second)) return;
        
        var b = new Bind(first, second, offset, strength, ropeLength, breakDistance);
        
        if (!Matrix.ContainsKey(first)) Matrix[first] = new Dictionary<IBindable, Bind>();
        if (!Matrix.ContainsKey(second)) Matrix[second] = new Dictionary<IBindable, Bind>();

        Matrix[first][second] = b;
        Matrix[second][first] = b;
        
        if (first is Block block1) Roots.Blocks[block1.rootId].pulseVersionDirty = true;
        else if (second is Block block2) Roots.Blocks[block2.rootId].pulseVersionDirty = true;

        if (first is IBindHandler firstH)
            firstH.OnBind(b);
        if (second is IBindHandler secondH)
            secondH.OnBind(b);
    }

    public static Bind GetBind(IBindable first, IBindable second)
    {
        return Matrix.ContainsKey(first) ? (Matrix[first].ContainsKey(second) ? Matrix[first][second] : null) : null;
    }

    public static void RemoveBind(IBindable first, IBindable second)
    {
        if (!Matrix.ContainsKey(first) || !Matrix[first].ContainsKey(second)) return;
        var bind = Matrix[first][second];
        Matrix[first]?.Remove(second);
        Matrix[second]?.Remove(first);
        
        if (first is Block block1) Roots.Blocks[block1.rootId].pulseVersionDirty = true;
        else if (second is Block block2) Roots.Blocks[block2.rootId].pulseVersionDirty = true;
        
        if (first is IBindHandler firstH)
            firstH.OnUnbind(bind);
        if (second is IBindHandler secondH)
            secondH.OnUnbind(bind);
    }

    public static void RemoveAllBinds(IBindable obj)
    {
        if (!Matrix.ContainsKey(obj)) return;

        var l = new List<Bind>(Matrix[obj].Values);
        foreach (var bind in l)
        {
            RemoveBind(bind.First, bind.Second);
        }
    }

    public static bool IsBound(IBindable first, IBindable second)
    {
        if (first == null || second == null)
        {
            Debug.Log("Binding a null");
            return false;
        }
        return Matrix.ContainsKey(first) && Matrix[first].ContainsKey(second);
    }

    public static IEnumerable<Bind> GetAllAdjacentBinds(IBindable obj)
    {
        if (!Matrix.ContainsKey(obj)) return new Bind[0];
        return Matrix[obj].Values;
    }

    public static int GetOutBindsCount(IBindable obj)
    {
        if (!Matrix.ContainsKey(obj)) return 0;
        var counter = 0;
        foreach (var bind in Matrix[obj].Values)
        {
            if (bind.First == obj) counter++;
        }
        return counter;
    }

    public static int GetBindsCount(IBindable obj)
    {
        if (!Matrix.ContainsKey(obj)) return 0;
        return Matrix[obj].Values.Count;
    }

    public static List<Bind> GetAllAsList()
    {
        return Matrix.Values.SelectMany(dict => dict.Values).Distinct().ToList();
    }

    public static void Clear()
    {
        
    }
}
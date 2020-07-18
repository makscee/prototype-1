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
        BindVisual.Create(b);
        
        if (!Matrix.ContainsKey(first)) Matrix[first] = new Dictionary<IBindable, Bind>();
        if (!Matrix.ContainsKey(second)) Matrix[second] = new Dictionary<IBindable, Bind>();

        Matrix[first][second] = b;
        Matrix[second][first] = b;

        if (first.IsAnchored() != second.IsAnchored())
            foreach (var obj in CollectAllBoundObjects(first))
                obj.SetAnchored(true);

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
        if (!first.IsAnchor()) RefreshAnchored(first);
        if (!second.IsAnchor()) RefreshAnchored(second);
        
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

    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    public static List<IBindable> CollectAllBoundObjects(IBindable obj)
    {
        var result = new List<IBindable>();
        var queue = new Queue<IBindable>();
        queue.Enqueue(obj);
        while (queue.Count > 0)
        {
            var next = queue.Dequeue();
            next.Used = true;
            result.Add(next);
            foreach (var bind in GetAllAdjacentBinds(next))
            {
                if (!bind.First.Used) queue.Enqueue(bind.First);
                if (!bind.Second.Used) queue.Enqueue(bind.Second);
            }
        }

        foreach (var o in result)
        {
            o.Used = false;
        }

        return result;
    }

    static void RefreshAnchored(IBindable obj)
    {
        var bound = CollectAllBoundObjects(obj);
        var hasAnchor = bound.Any(o => o.IsAnchor());
        foreach (var o in bound)
        {
            o.SetAnchored(hasAnchor);
        }
    }

    public static List<Bind> GetAllAsList()
    {
        return Matrix.Values.SelectMany(dict => dict.Values).Distinct().ToList();
    }

    public static void Clear()
    {
        
    }
}
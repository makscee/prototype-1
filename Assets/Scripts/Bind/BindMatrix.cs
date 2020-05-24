using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BindMatrix
{
    static readonly Dictionary<IBindable, Dictionary<IBindable, Bind>> Matrix = new Dictionary<IBindable, Dictionary<IBindable, Bind>>();

    public static void AddBind(IBindable first, IBindable second, Vector2 offset, int strength,
        float breakDistance = -1)
    {
        if (IsBound(first, second)) return;
        
        var b = new Bind(first, second, offset, strength, breakDistance);
        
        if (!Matrix.ContainsKey(first)) Matrix[first] = new Dictionary<IBindable, Bind>();
        if (!Matrix.ContainsKey(second)) Matrix[second] = new Dictionary<IBindable, Bind>();

        Matrix[first][second] = b;
        Matrix[second][first] = b;

        if (first.IsAnchored() != second.IsAnchored())
            foreach (var obj in CollectAllBoundObjects(first))
                obj.SetAnchored(true);

        if (first is IBindHandler firstH)
            firstH.OnBind(second);
        if (second is IBindHandler secondH)
            secondH.OnBind(first);
    }

    public static void RemoveBind(IBindable first, IBindable second)
    {
        Matrix[first]?.Remove(second);
        Matrix[second]?.Remove(first);
        RefreshAnchored(first);
        RefreshAnchored(second);
        if (first is IBindHandler firstH)
            firstH.OnUnbind(second); 
        if (second is IBindHandler secondH)
            secondH.OnUnbind(first);
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

    public static void Update()
    {
        var objects = new HashSet<Bind>();
        foreach (var d in Matrix.Values)
        {
            foreach (var bind in d.Values)
            {
                objects.Add(bind);
            }
        }

        foreach (var bind in objects)
        {
            bind.Update();
        }
    }

    public static List<Bind> GetAllBindsAsList()
    {
        return Matrix.Values.SelectMany(dict => dict.Values).ToList();
    }
}
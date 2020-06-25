using System.Collections.Generic;

public static class NewBlockPlaceholderPool
{
    static List<NewBlockPlaceholder> _pool = new List<NewBlockPlaceholder>();
    
    public static void ClearAll()
    {
        foreach (var obj in _pool)
        {
            obj.gameObject.SetActive(false);
        }
    }

    public static void CreateAround(Block parent)
    {
        for (var i = 0; i < 4; i++)
        {
            Utils.CoordsFromDir(i, out var x, out var y);
            x += parent.X;
            y += parent.Y;
            if (FieldMatrix.Get(x, y, out var b)) continue;
            NewBlockPlaceholder newBlock = null;
            foreach (var obj in _pool)
            {
                if (!obj.gameObject.activeSelf)
                {
                    obj.gameObject.SetActive(true);
                    newBlock = obj;
                    break;
                }
            }

            if (newBlock == null)
            {
                _pool.Add(NewBlockPlaceholder.Create(parent, x, y));
            }
            else
            {
                newBlock.Parent = parent;
                newBlock.Painter.palette = parent.pulseBlock.palette;
                
                newBlock.X = x;
                newBlock.Y = y;
                newBlock.UpdatePosition();
            }
        }
    }
}
public class Cell : BindableMonoBehavior, IBindHandler
{

    void OnEnable()
    {
        ColorPalette.SubscribeGameObject(gameObject, 2);
    }
    
    public TetrisField field;
    public int x, y;
    public void OnBind(IBindable obj)
    {
        if (obj is Block block)
        {
            field.AddBlock(x, y, block);
        }
    }

    public void OnUnbind(IBindable obj)
    {
        if (obj is Block)
        {
            field.RemoveBlock(x, y);
        }
    }


    public override bool IsAnchor()
    {
        return true;
    }

    public override bool IsAnchored()
    {
        return true;
    }
}
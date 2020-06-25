using UnityEngine;
using UnityEngine.EventSystems;

public class NewBlockPlaceholder : MonoBehaviour, IPointerClickHandler
{
    public int X, Y;
    public Block Parent;
    public Painter Painter;

    void Awake()
    {
        Painter = GetComponent<Painter>();
    }

    public static NewBlockPlaceholder Create(Block parent, int x, int y)
    {
        var nbp = Instantiate(Prefabs.Instance.NewBlockPlaceholder, SharedObjects.Instance.FrontCanvas.transform).GetComponent<NewBlockPlaceholder>();
        nbp.Parent = parent;
        nbp.X = x;
        nbp.Y = y;
        nbp.Painter.palette = parent.PulseBlock.palette;
        nbp.Painter.NumInPalette = 1;
        return nbp;
    }
    void Start()
    {
        UpdatePosition();
    }

    void Update()
    {
        if (Parent == null)
        {
            gameObject.SetActive(false);
            return;
        }
        UpdatePosition();
    }

    public void UpdatePosition()
    {
        transform.position = Parent.transform.position + new Vector3(X - Parent.X, Y - Parent.Y);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        DoCreateBlock();
    }

    void DoCreateBlock()
    {
        if (!FieldMatrix.Get(X, Y, out var t))
        {
            var block = Block.Create(Parent, X, Y);
            NewBlockPlaceholderPool.ClearAll();
            NewBlockPlaceholderPool.CreateAround(Parent);
            NewBlockPlaceholderPool.CreateAround(block);
        }
    }
}
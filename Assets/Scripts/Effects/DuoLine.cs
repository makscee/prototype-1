using System;
using UnityEngine;

public class DuoLine : MonoBehaviour
{
    [SerializeField] Transform trans0, trans1;
    [SerializeField] bool world0, world1;
    [SerializeField] LineRenderer _lr;
    [SerializeField] Camera _cam;
    public Vector2 offset0, offset1;

    void Start()
    {
        _lr = GetComponent<LineRenderer>();
        _cam = SharedObjects.Instance.Camera;
        RefreshPositions();
    }

    void Update()
    {
        RefreshPositions();
    }

    void RefreshPositions()
    {
        GetPositions(out var pos0, out var pos1);
        _lr.SetPosition(0, pos0 + offset0);
        _lr.SetPosition(1, pos1 + offset1);
    }

    void GetPositions(out Vector2 first, out Vector2 second)
    {
        first = world0 ? trans0.position : _cam.ScreenToWorldPoint(trans0.position);
        second = world1 ? trans1.position : _cam.ScreenToWorldPoint(trans1.position);
    }

    public static DuoLine Create(Transform t0, Transform t1, bool w0, bool w1, int rootId)
    {
        var go = Instantiate(Prefabs.Instance.duoLine);
        var dl = go.GetComponent<DuoLine>();
        dl.trans0 = t0;
        dl.trans1 = t1;
        dl.world0 = w0;
        dl.world1 = w1;
        dl.GetComponent<Painter>().palette = Roots.Root[rootId].palette;
        return dl;
    }
}
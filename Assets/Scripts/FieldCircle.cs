using UnityEngine;

public class FieldCircle : MonoBehaviour
{
    public Transform Target;

    float _scaleBase;
    void Update()
    {
        if (Target == null)
        {
            Destroy(gameObject);
            return;
        }
        if (_scaleBase < 0.9f) _scaleBase += Time.deltaTime / 3;

        transform.position = Target.transform.position;
        var v = Mathf.Sin(Time.time) * 0.1f + _scaleBase;
        transform.localScale = new Vector3(v, v, v);
    }

    public static FieldCircle Create(Transform target)
    {
        var go = Instantiate(Prefabs.Instance.FieldCircle, SharedObjects.Instance.MidCanvas.transform);
        var fc = go.GetComponent<FieldCircle>();
        fc.Target = target;
        return fc;
    }
}
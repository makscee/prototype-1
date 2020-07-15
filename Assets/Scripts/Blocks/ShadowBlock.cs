using System;
using UnityEngine;

public class ShadowBlock : MonoBehaviour
{
    Block selfBlock;
    SpriteRenderer _sr;
    public static void Create(Block block)
    {
        var shadowBlock = Instantiate(Prefabs.Instance.ShadowBlock).GetComponent<ShadowBlock>();
        shadowBlock.selfBlock = block;
    }

    void OnEnable()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (selfBlock == null)
        {
            Destroy(gameObject);
            return;
        }
        transform.position = selfBlock.transform.position;
        _sr.enabled = selfBlock.Masked;
    }
    
    
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs : MonoBehaviour
{
    public GameObject Block;
    public GameObject BindVisual;
    public GameObject FieldCircle;
    public GameObject NewBlockPlaceholder;
    public GameObject ConfigRack;
    
    
    public static Prefabs Instance;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        Instance = this;
    }
}

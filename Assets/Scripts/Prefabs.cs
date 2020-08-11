﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prefabs : MonoBehaviour
{
    public GameObject NodeBlock;
    public GameObject RootBlock;
    public GameObject BindVisual;
    public GameObject FieldCircle;
    public GameObject NewBlockPlaceholder;
    public GameObject ConfigRack;
    public GameObject ShadowBlock;
    public GameObject Pixel;
    public GameObject RollingButton;
    public GameObject BindShadowParticles;
    
    
    public static Prefabs Instance;

    void OnEnable()
    {
        Instance = this;
    }
}

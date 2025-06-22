using System;
using Art.Shaders;
using UnityEngine;

public class M_VectorColour : M_ShaderValueController
{
    [SerializeField] private Color color;
    
    public Color Color
    {
        get => color;
        set
        {
            color = value;
            VisUpdateColour();
        }
    }

    protected override void Awake()
    {
        base.Awake();
        VisUpdateColour();
    }

    private void VisUpdateColour()
    {
        try
        {
            material.SetColor("_Color", color);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"{gameObject.name}.{e}");
        }
    }

    private void OnValidate()
    {
        VisUpdateColour();
    }
}

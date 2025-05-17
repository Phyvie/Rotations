using System;
using System.Linq;
using UnityEngine;

public class M_CircleSector : MonoBehaviour
{
    [SerializeField] private Material editSharedMaterial;
    [HideInInspector] private Material runtimeMaterialInstance; 
    public Material material => Application.isPlaying ? runtimeMaterialInstance : editSharedMaterial;

    [SerializeField] private float _beginAngle;
    [SerializeField] private float _endAngle;
    private int _fullRotations;
    private int fullRotations => (int) (Mathf.Abs(_endAngle - _beginAngle) / (2*Mathf.PI));

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        runtimeMaterialInstance = renderer.materials.FirstOrDefault(mat => mat == editSharedMaterial);
        runtimeMaterialInstance = renderer.material; 
        runtimeMaterialInstance.name = runtimeMaterialInstance.name + gameObject.name;
    }

    private void OnDestroy()
    {
        runtimeMaterialInstance = null; 
    }

    public float BeginAngle
    {
        get => _beginAngle;
        set
        {
            _beginAngle = value;
            UpdateShader(); 
        }
    }
    
    public float EndAngle
    {
        get => _endAngle;
        set
        {
            _endAngle = value;
            UpdateShader();
        }
    }

    void UpdateShader()
    {
        if (material == null) return;
        
        material.SetFloat("_BeginAngle", _beginAngle);
        material.SetFloat("_EndAngle", _endAngle);
        material.SetColor("_InsideAngleColour", 
            BeginAngle < EndAngle ? 
                material.GetColor("_PositiveAngleColour") :
                material.GetColor("_NegativeAngleColour"));
        material.SetFloat("FullRotations", fullRotations);
    }

    private void OnValidate()
    {
        BeginAngle = _beginAngle;
        EndAngle = _endAngle;
    }
}

using TMPro;
using UnityEngine;

/*
 * Proxy class for managing a 3D Coordinate Grid, consisting of multiple 2d-Grid which can be activated/deactivated etc.
 * TODO: More specific functionalities of the grid themselves (e.g. changing grid size), do not yet exist, but should be in an extra class, which this one accesses
 * TODO: this class should have colour control over its children (Axes & Labels)
 */
public class Vis_CoordinateGrid : MonoBehaviour
{
    [SerializeField] private Camera viewCamera; 
    
    public Camera ViewCamera
    {
        get => viewCamera;
        set => viewCamera = value;
    }
    
    [SerializeField] private TMP_Text xLabel;
    [SerializeField] private TMP_Text yLabel;
    [SerializeField] private TMP_Text zLabel;
    
    [SerializeField] private GameObject xzGrid;
    [SerializeField] private GameObject xyGrid;
    [SerializeField] private GameObject yzGrid;
    
    private bool _xzGridActive; 
    private bool _xyGridActive; 
    private bool _yzGridActive;

    public bool XZGridActive
    {
        get => _xzGridActive;
        set
        {
            _xzGridActive = value;
            xzGrid.SetActive(value);
        }
    }

    public bool XYGridActive
    {
        get => _xyGridActive;
        set
        {
            _xyGridActive = value;
            xyGrid.SetActive(value);
        }
    }

    public bool YZGridActive
    {
        get => _yzGridActive;
        set
        {
            _yzGridActive = value;
            yzGrid.SetActive(value);
        }
    }

    [SerializeField] private Vector3 labelOffset; 
    private Vector3[] _labelDefaultPositions = new Vector3[3]; 
    
    private void Awake()
    {
        XZGridActive = xzGrid.activeSelf; 
        XYGridActive = xyGrid.activeSelf; 
        YZGridActive = yzGrid.activeSelf;
        
        _labelDefaultPositions[0] = xLabel.transform.position; 
        _labelDefaultPositions[1] = yLabel.transform.position; 
        _labelDefaultPositions[2] = zLabel.transform.position;
    }

    private void Update()
    {
        UpdateLabelPositionAndRotation();
    }

    private void UpdateLabelPositionAndRotation()
    {
        if (!ViewCamera)
        {
            Debug.LogWarning($"{name}.{nameof(ViewCamera)} is null");
            return; 
        }
        
        xLabel.transform.rotation = ViewCamera.transform.rotation; 
        yLabel.transform.rotation = ViewCamera.transform.rotation; 
        zLabel.transform.rotation = ViewCamera.transform.rotation;
        
        Vector3 labelOffsetLocal = ViewCamera.transform.TransformVector(labelOffset);
        
        xLabel.transform.position = _labelDefaultPositions[0] + labelOffsetLocal; 
        yLabel.transform.position = _labelDefaultPositions[1] + labelOffsetLocal; 
        zLabel.transform.position = _labelDefaultPositions[2] + labelOffsetLocal;
    }
}

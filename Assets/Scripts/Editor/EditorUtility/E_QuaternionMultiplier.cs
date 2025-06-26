using System;
using RotParams;
using UnityEngine;

public class E_QuaternionMultiplier : MonoBehaviour
{
    [SerializeField] private RotParams_Quaternion quatA; 
    [SerializeField] private RotParams_Quaternion quatB;

    [SerializeField] private Vector3 directionA;
    [SerializeField] private Vector3 directionB;
    [SerializeField] private float slerpT;
    
    [ContextMenu("Multiply")]
    private void Multiply()
    {
        RotParams_Quaternion result = quatA * quatB;
        Debug.Log($"{quatA} * {quatB} = {result}");
        
        Quaternion A = new Quaternion(); 
        Quaternion B = new Quaternion();
        Quaternion ABSlerp = Quaternion.Slerp(A, B, 0.3f);
        Quaternion ABLerp = Quaternion.Lerp(A, B, 0.3f);
        Quaternion ABRotateTowards = Quaternion.RotateTowards(A, B, 0.3f);
    }

    private void OnValidate()
    {

    }
}

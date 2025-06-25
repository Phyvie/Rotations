using System;
using UnityEditor;
using UnityEngine;

namespace Visualisation
{
    public class Gizmo_Rotation : MonoBehaviour
    {
        public enum EArcType
        {
            Arc45, 
            Arc45Tapered, 
            Arc270, 
            Arc270Tapered
        }
        
        [SerializeField]
        Mesh prefab45Arc; 
        [SerializeField]
        Mesh prefab45ArcTapered;
        [SerializeField]
        Mesh prefab270Arc;
        [SerializeField]
        Mesh prefab270ArcTapered;
        
        [SerializeField] private Vector3 rotationAxis; 
        [SerializeField] private EArcType arcType;
        [SerializeField] private GameObject arc; 
        [SerializeField] private GameObject head;
        [SerializeField] private Vector3 headPos45; 
        [SerializeField] private Vector3 eulerRot45; 
        [SerializeField] private Vector3 headPos270;
        [SerializeField] private Vector3 eulerRot270;

        public Vector3 RotationAxis
        {
            get => rotationAxis;
            set
            {
                rotationAxis = value;
                transform.rotation = Quaternion.FromToRotation(Vector3.right, rotationAxis);
            }
        }

        public EArcType ArcType
        {
            get => arcType;
            set
            {
                arcType = value;
                switch (value)
                {
                    case EArcType.Arc45:
                        arc.GetComponent<MeshFilter>().mesh = prefab45Arc;
                        head.transform.localPosition = headPos45;
                        head.transform.localEulerAngles = eulerRot45;
                        break;
                    case EArcType.Arc45Tapered:
                        arc.GetComponent<MeshFilter>().mesh = prefab45ArcTapered;
                        head.transform.localPosition = headPos45;
                        head.transform.localEulerAngles = eulerRot45;
                        break;
                    case EArcType.Arc270:
                        arc.GetComponent<MeshFilter>().mesh = prefab270Arc;
                        head.transform.localPosition = headPos270;
                        head.transform.localEulerAngles = eulerRot270;
                        break;
                    case EArcType.Arc270Tapered:
                        arc.GetComponent<MeshFilter>().mesh = prefab270ArcTapered;
                        head.transform.localPosition = headPos270;
                        head.transform.localEulerAngles = eulerRot270;
                        break;
                    default:
                        arc.GetComponent<MeshFilter>().mesh = null;
                        break;
                }
            }
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
            {
                return; 
            }
            RotationAxis = rotationAxis;
        }
        #endif
    }
}
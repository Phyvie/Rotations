using System;
using RotParams;
using UnityEngine;
using UnityEngine.UIElements;

namespace RotContainers
{
    public class GeneralRotationContainer : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        private VisualElement uiRoot; 

        private RotParams.RotParams rotParams;
        [SerializeField] private TypedRotationContainer typedRotationContainer;

        [Serializable]
        public class UISlot
        {
            [SerializeField] public string ContainerName; 
            public VisualElement Container;

            public void Init(VisualElement visualElement)
            {
                Container = visualElement.Q(ContainerName);
            }
        }

        [SerializeField] private UISlot uiRotSlot;
        
        [SerializeField] private UISlot uiTypeSlot; //!ZyKa contextUI
        [SerializeField] private UISlot uiContextMenuSlot; //!ZyKa contextUI

        [Serializable]
        public class TypedRotationContainerPrefab
        {
            public GameObject visPrefab;
            public VisualTreeAsset uiPrefab; 
        }

        [SerializeField] private TypedRotationContainerPrefab eulerPrefab;
        [SerializeField] private TypedRotationContainerPrefab quaternionPrefab;
        [SerializeField] private TypedRotationContainerPrefab axisAnglePrefab;
        [SerializeField] private TypedRotationContainerPrefab matrixPrefab;

        private void Awake()
        {
            Init(); 
        }

        [ContextMenu("Init Rotation Container")]
        public void Init()
        {
            uiRoot = uiDocument.rootVisualElement;
            uiTypeSlot.Init(uiRoot);
            uiRotSlot.Init(uiRoot);
            uiContextMenuSlot.Init(uiRoot);
        }

        public void GenerateNewRotation<RotParamsType>() where RotParamsType : RotParams.RotParams, new()
        {
            rotParams = new RotParamsType();
            if (typedRotationContainer == null)
            {
                typedRotationContainer = gameObject.AddComponent<TypedRotationContainer>();
            }
            TypedRotationContainerPrefab prefab =
                typeof(RotParamsType) switch
                {
                    Type t when t == typeof(RotParams_EulerAngles) => eulerPrefab,
                    Type t when t == typeof(RotParams_Quaternion) => quaternionPrefab,
                    Type t when t == typeof(RotParams_AxisAngle) => axisAnglePrefab,
                    { } t when t == typeof(RotParams_Matrix) => matrixPrefab,
                    _ => null
                };
            
            if (prefab == null)
            {
                throw new NullReferenceException();
            }
            
            typedRotationContainer.SpawnTypedRotation(ref rotParams, prefab.visPrefab, this.transform, prefab.uiPrefab, uiRotSlot.Container); 
        }
        
        [ContextMenu("GenerateMatrix")]
        public void GenerateNewMatrix()
        {
            GenerateNewRotation<RotParams_Matrix>(); 
        }
        
        [ContextMenu("GenerateAxisAngle")]
        public void GenerateNewAxisAngle()
        {
            GenerateNewRotation<RotParams_AxisAngle>(); 
        }
        
        [ContextMenu("GenerateQuaternion")]
        public void GenerateNewQuaternion()
        {
            GenerateNewRotation<RotParams_Quaternion>(); 
        }
        
        [ContextMenu("GenerateEuler")]
        public void GenerateNewEulerAngles()
        {
            GenerateNewRotation<RotParams_EulerAngles>();
        }
    }
}

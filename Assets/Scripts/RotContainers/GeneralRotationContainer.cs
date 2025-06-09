using System;
using System.Collections.Generic;
using System.Reflection;
using RotParams;
using Unity.Properties;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RotContainers
{
    public class GeneralRotationContainer : MonoBehaviour
    {
        #region RuntimeVariables
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private VisualTreeAsset uiMenuAsset;

        private VisualElement _uiMenu; 
        
        private RotParams.RotParams _rotParams;
        [SerializeField] private TypedRotationContainer typedRotationContainer;
        #endregion

        #region UISlots
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
        [SerializeField] private UISlot uiMenuSlot; 
        #endregion UISlots
        
        #region RotationTypeData
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
        #endregion RotationTypeData
        
        #region UITypeSelectionControls
        [CreateProperty]
        public List<string> SelectableRotationTypes => new List<string>
        {
            "EulerAngles", 
            "Quaternion", 
            "AxisAngle", 
            "Matrix"
        };

        private int _selectedTypeIndex = 0;

        [CreateProperty]
        private int SelectedTypeIndex
        {
            get => _selectedTypeIndex;
            set
            {
                switch (value)
                {
                    case 0: 
                        GenerateNewEulerAngles();
                        break;
                    case 1:
                        GenerateNewQuaternion();
                        break;
                    case 2: 
                        GenerateNewAxisAngle();
                        break;
                    case 3:
                        GenerateNewMatrix();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _selectedTypeIndex = value;
            }
        }
        #endregion UITypeSelectionControls
        
        private void Awake()
        {
            Init(); 
        }

        [ContextMenu("Init Rotation Container")]
        public void Init()
        {
            uiRotSlot.Init(uiDocument.rootVisualElement);
            uiMenuSlot.Init(uiDocument.rootVisualElement);

            _uiMenu = uiMenuAsset.CloneTree(); 
            uiMenuSlot.Container.Add(_uiMenu);
            _uiMenu.dataSource = this; 
        }

        public void GenerateNewRotation<RotParamsType>() where RotParamsType : RotParams.RotParams, new()
        {
            _rotParams = new RotParamsType();
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
            
            typedRotationContainer.SpawnTypedRotation(ref _rotParams, prefab.visPrefab, this.transform, prefab.uiPrefab, uiRotSlot.Container); 
        }

        //accessor function for the templated version
        public void GenerateNewRotationGeneric(System.Type type)
        {
            if (!typeof(RotParams.RotParams).IsAssignableFrom(type))
            {
                Debug.LogError($"Type {type} is not a subclass of RotParams.RotParams.");
                return;
            }

            if (type.GetConstructor(Type.EmptyTypes) == null)
            {
                Debug.LogError($"Type {type} does not have a parameterless constructor.");
                return;
            }

            // Get a reference to the generic method definition
            var method = GetType()
                .GetMethod(nameof(GenerateNewRotation), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                ?.MakeGenericMethod(type);

            if (method == null)
            {
                Debug.LogError($"Failed to resolve generic method for type {type}.");
                return;
            }

            // Invoke the generic method
            method.Invoke(this, null);
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

        private void OnValidate()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode && !Application.isPlaying)
            {
                return; 
            }
            Init(); 
        }
    }
}

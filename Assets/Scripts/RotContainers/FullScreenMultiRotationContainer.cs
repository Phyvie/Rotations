using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace RotContainers
{
    public class FullScreenMultiRotationContainer : MonoBehaviour
    {
        [SerializeField] private UIDocument uiFullScreen;
        [SerializeField] private GameObject RotationContainerPrefab; 
        
        [SerializeField] 
        private List<CombinedRotationContainer> rotationContainer;

        private void Awake()
        {
            InitialiseRotationContainers();   
        }

        private void InitialiseRotationContainers()
        {
            for (int i = 0; i < rotationContainer.Count; i++)
            {
                VisualElement newVisualRoot = new VisualElement();
                newVisualRoot.name = "screenPartContainer" + i; 
                uiFullScreen.rootVisualElement.Add(newVisualRoot);
            }
        }

        private void InitialiseRotationContainer(CombinedRotationContainer rotationContainer)
        {
            
        }
    }
}
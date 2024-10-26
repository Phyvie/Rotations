using System.Collections.Generic;
using System.Reflection;
using RotationTypes;
using TMPro;
using UnityEngine;

namespace RotationVisualisation
{
    public class MB_Matrix : MonoBehaviour
    {
        private Matrix _refMatrix;
        [SerializeField] private MonoBehaviour MatrixHolder; 
        [SerializeField] private GameObject InputFieldsParent;
        [SerializeField] private TMP_InputField InputFieldPrefab;
        [SerializeField] private List<TMP_InputField> InputFields;
        
        public Matrix RefMatrix
        {
            get => _refMatrix;
            set => _refMatrix = value;
        }

        [ContextMenu("InitialiseInputFields")]
        public void InitialiseInputFields()
        {
            // Clear existing input fields if any
            foreach (Transform child in InputFieldsParent.transform)
            {
                DestroyImmediate(child.gameObject);
            }

            InputFields.Clear();

            // Calculate input field dimensions
            float parentWidth = InputFieldsParent.GetComponent<RectTransform>().rect.width;
            float parentHeight = InputFieldsParent.GetComponent<RectTransform>().rect.height;
            float inputFieldWidth = parentWidth / _refMatrix.Width;
            float inputFieldHeight = parentHeight / _refMatrix.Height;

            // Create input fields for each element in the matrix
            for (int row = 0; row < _refMatrix.Height; row++)
            {
                for (int column = 0; column < _refMatrix.Width; column++)
                {
                    // Instantiate a new input field
                    TMP_InputField inputField = Instantiate(InputFieldPrefab, InputFieldsParent.transform);
                    inputField.text = _refMatrix[row, column].ToString();
                    int r = row;
                    int c = column;

                    // Set up listener to update the matrix value when the input changes
                    inputField.onValueChanged.AddListener((value) =>
                    {
                        if (float.TryParse(value, out float result))
                        {
                            _refMatrix[r, c] = result;
                        }
                    });

                    // Set the size and position of the input field
                    RectTransform rectTransform = inputField.GetComponent<RectTransform>();
                    rectTransform.sizeDelta = new Vector2(inputFieldWidth, inputFieldHeight);
                    rectTransform.anchoredPosition = new Vector2(column * inputFieldWidth, -row * inputFieldHeight);

                    // Add the input field to the list
                    InputFields.Add(inputField);
                }
            }
        }

        private void OnDestroy()
        {
            // Remove listeners from input fields
            foreach (var inputField in InputFields)
            {
                inputField.onValueChanged.RemoveAllListeners();
            }
        }
        
        [ContextMenu("SearchForMatrix")]
        public void SearchForMatrix()
        {
            // Ensure the MatrixHolder is set
            if (MatrixHolder == null)
            {
                Debug.LogError("MatrixHolder is not set.");
                return;
            }

            PropertyInfo[] properties = MatrixHolder.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                if (property.PropertyType == typeof(Matrix))
                {
                    Matrix foundMatrix = (Matrix)property.GetValue(MatrixHolder);
                    if (foundMatrix != null)
                    {
                        RefMatrix = foundMatrix;
                        Debug.Log($"Matrix found in property: {property.Name}");
                        InitialiseInputFields(); // Initialize input fields with the found matrix
                        return;
                    }
                }
            }

            Debug.LogError("No Matrix found in the target behaviour.");
        }
    }
}

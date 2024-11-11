using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class NestedPropertyDrawer : PropertyDrawer
    {
        private bool initialized = false;
        static readonly Regex arrayRegex = new Regex(@"^data\[(\d+)\]$");
        
        protected object propertyAsObject = null;
        protected Type propertyType = null;
        
        protected List<object> objectHierarchy = new List<object>();
        protected List<FieldInfo> fieldInfoHierarchy = new List<FieldInfo>();

        protected object parentObject => objectHierarchy[~2]; 
        
        void AddToHierarchies(object obj, FieldInfo fieldInfo)
        {
	        objectHierarchy.Add(obj);
	        fieldInfoHierarchy.Add(fieldInfo);
        }

        void ClearHierarchies()
        {
	        objectHierarchy.Clear();
	        fieldInfoHierarchy.Clear();
        }

        protected virtual void InitializePropertyNesting(SerializedProperty prop)
        {
            if (initialized)
            {
	            return; 
            }
            
            ClearHierarchies();
            SerializedObject serializedObject = prop.serializedObject;
            propertyAsObject = serializedObject?.targetObject;
            propertyType = propertyAsObject?.GetType();
            
            if (string.IsNullOrEmpty(prop.propertyPath) || propertyAsObject is null)
            {
	            Debug.LogError("InitializeNesting failed");
                return; 
            }
            AddToHierarchies(propertyAsObject, null);

            string[] splitPath = prop.propertyPath.Split('.');
            
            Type cFieldType = null;
            FieldInfo cFieldInfo = null;
            object cObject = prop.serializedObject.targetObject; 
            
            for (int i = 0; i < splitPath.Length; i++)
            {
                string pathNode = splitPath[i];
                if (pathNode.Equals("Array"))
                {
	                i++;
	                pathNode = splitPath[i]; 
	                Match arrayMatch = arrayRegex.Match(pathNode);
	                if (!arrayMatch.Success)
	                {
		                throw new Exception(); 
	                }

	                int.TryParse(arrayMatch.Groups[1].Value, out int arrayIndex); 
	                GetFieldArrayObject(cObject, cFieldInfo, arrayIndex, out cObject);
                }
                else
                {
	                GetSubField(cObject, pathNode, out cObject, out cFieldInfo);
                }
                AddToHierarchies(cObject, cFieldInfo);
            }
            initialized = true;
		}

        protected void GetSubField(object source, string fieldName, out object obj, out FieldInfo fI)
        {
	        if (source == null) { throw new NullReferenceException(); }
	        Type type = source.GetType();
	        fI = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
	        if (fI == null) { throw new NullReferenceException(); }
	        obj = fI.GetValue(source); 
        }
        
        protected void GetFieldArrayObject(object source, FieldInfo fIn, int index, out object arrayElement)
        {
	        var enumerator = (source as IList)?.GetEnumerator();
	        if (enumerator == null) { throw new TypeAccessException(); }
			
	        while (index-- >= 0) enumerator.MoveNext();
	        arrayElement = enumerator.Current; 
        }

        protected object GetParentObject(SerializedProperty property)
        {
	        return objectHierarchy[~2];
        }
        
        protected SerializedProperty GetParentSerializedProperty(SerializedProperty prop)
        {
	        string[] splitPath = prop.propertyPath.Split('.');
	        if (splitPath.Length <= 1)
	        {
		        Debug.LogError("Can't access parentSerializedProperty, because there is no parent");
		        return null; 
	        }
	        string parentPath = prop.propertyPath.Substring(0, prop.propertyPath.LastIndexOf(".", StringComparison.Ordinal)); 
	        Match arrayMatch = arrayRegex.Match(splitPath[~2]);
	        if (arrayMatch.Success)
	        {
		        parentPath = parentPath.Substring(0, parentPath.LastIndexOf(".", StringComparison.Ordinal)); 
		        parentPath = parentPath.Substring(0, parentPath.LastIndexOf(".", StringComparison.Ordinal)); 
	        }
	        return prop.serializedObject.FindProperty(parentPath); 
        }

        protected int GetIndexOfProperty(SerializedProperty property)
        {
	        object targetObject = property.serializedObject.targetObject;
	        if (parentObject is IList objectList)
	        {
		        string path = property.propertyPath;
		        int indexOpen = path.LastIndexOf("[");
		        int indexClosed = path.LastIndexOf("]"); 
		        int.TryParse(path.Substring(indexOpen, indexClosed - indexOpen), out int arrayIndex);
		        return arrayIndex; 
	        }
	        else
	        {
		        return -1; 
	        }
        }
        
        protected T GetObject<T>(SerializedProperty property)
        {
	        object targetObject = property.serializedObject.targetObject;
	        if (parentObject is IList<T> objectList)
	        {
		        return ((T[])fieldInfo.GetValue(targetObject))[GetIndexOfProperty(property)]; 
	        }
	        else
	        {
				return (T)fieldInfo.GetValue(targetObject);   
	        }
        }

        protected void SetFieldValue(SerializedProperty property, object newValue)
        {
	        if (parentObject is IList objectArray)
	        {
		        objectArray[GetIndexOfProperty(property)] = newValue; 
	        }
	        fieldInfo.SetValue(parentObject, newValue);
        }

        protected void AttemptSetPropertyValue(object newValue, bool bSetFieldValueIfFailed)
        {
	        throw new NotImplementedException(); 
        }
    }
}
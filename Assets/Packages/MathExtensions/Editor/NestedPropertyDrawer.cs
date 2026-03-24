using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	/*  LaterZyKa Dependencies: Only AngleTypeInspector is based on this class */
    public class NestedPropertyDrawer : PropertyDrawer
    {
        static readonly Regex arrayRegex = new Regex(@"^data\[(\d+)\]$");
        
        protected object propertyAsObject = null;
        protected Type propertyType = null;
        
        protected List<object> objectHierarchy = new List<object>();
        protected List<FieldInfo> fieldInfoHierarchy = new List<FieldInfo>();

        protected object parentObject => objectHierarchy.Count >= 2 ? objectHierarchy[^2] : null;
        protected FieldInfo fieldInfo => fieldInfoHierarchy.Count >= 1 ? fieldInfoHierarchy[^1] : null;
        
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
            ClearHierarchies();
            SerializedObject serializedObject = prop.serializedObject;
            propertyAsObject = serializedObject?.targetObject;
            propertyType = propertyAsObject?.GetType();
            
            if (string.IsNullOrEmpty(prop.propertyPath) || propertyAsObject is null)
            {
	            Debug.LogError($"InitializeNesting failed for SerializedProperty: {prop.name}");
                return; 
            }
            AddToHierarchies(propertyAsObject, null);

            string[] splitPath = prop.propertyPath.Split('.');
            
            FieldInfo cFieldInfo = null;
            object cObject = prop.serializedObject.targetObject; 

            try
            {
	            for (int i = 0; i < splitPath.Length; i++)
	            {
		            string pathNode = splitPath[i];
		            if (pathNode.Equals("Array"))
		            {
			            i++;
			            pathNode = splitPath[i];
			            int arrayIndex = GetIndexFromPathNode(pathNode);
			            GetFieldArrayObject(cObject, cFieldInfo, arrayIndex, out cObject);
		            }
		            else
		            {
			            GetSubField(cObject, pathNode, out cObject, out cFieldInfo);
		            }

		            AddToHierarchies(cObject, cFieldInfo);
	            }
            }
            catch (Exception e)
            {
	            Debug.LogError($"InitializePropertyNesting failed at path '{prop.propertyPath}'. Failed to resolve node.\nException: {e.Message}");
	            throw;
            }
		}

        protected void GetSubField(object source, string fieldName, out object obj, out FieldInfo fI)
        {
	        if (source == null)
	        {
		        throw new NullReferenceException($"Cannot get subfield '{fieldName}' from a null source.");
	        }
	        Type type = source.GetType();
	        fI = null;
	        while (type != null && fI == null)
	        {
		        fI = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		        type = type.BaseType;
	        }
	        
	        if (fI == null)
	        {
		        throw new MissingFieldException(source.GetType().FullName, fieldName);
	        }
	        obj = fI.GetValue(source); 
        }
        
        protected void GetFieldArrayObject(object source, FieldInfo fIn, int index, out object arrayElement)
        {
	        if (source is not IList list)
	        {
		        string typeName = source?.GetType().FullName ?? "null";
		        throw new InvalidOperationException($"Source of type '{typeName}' is not an IList. Expected a list or array to access index {index}.");
	        }

	        if (index < 0 || index >= list.Count)
	        {
		        throw new IndexOutOfRangeException($"Index {index} is out of range for list of size {list.Count}.");
	        }

	        arrayElement = list[index];
        }

        protected object GetParentObject(SerializedProperty property)
        {
	        return objectHierarchy.Count >= 2 ? objectHierarchy[^2] : null;
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
	        Match arrayMatch = arrayRegex.Match(splitPath[^2]);
	        if (arrayMatch.Success)
	        {
		        parentPath = parentPath.Substring(0, parentPath.LastIndexOf(".", StringComparison.Ordinal)); 
		        parentPath = parentPath.Substring(0, parentPath.LastIndexOf(".", StringComparison.Ordinal)); 
	        }
	        return prop.serializedObject.FindProperty(parentPath); 
        }

        protected int GetIndexOfSerializedProperty(SerializedProperty property)
        {
	        if (parentObject is IList objectList)
	        {
		        string path = property.propertyPath;
		        string lastPathNode = path.Substring(path.LastIndexOf('.') + 1);
		        return GetIndexFromPathNode(lastPathNode); 
	        }
	        else
	        {
		        return -1; 
	        }
        }

        protected int GetIndexFromPathNode(string pathNode)
        {
	        Match arrayMatch = arrayRegex.Match(pathNode);
	        if (!arrayMatch.Success)
	        {
		        throw new FormatException($"Path node '{pathNode}' does not match expected array format 'data[index]'."); 
	        }

	        if (!int.TryParse(arrayMatch.Groups[1].Value, out int arrayIndex))
	        {
		        throw new FormatException($"Failed to parse index from path node '{pathNode}'.");
	        }
	        return arrayIndex; 
        }
        
        protected T GetObject<T>(SerializedProperty property)
        {
	        // object serializedObject = property.serializedObject.targetObject; //property.serializedObject.targetObject
	        if (parentObject is IList<T> objectList)
	        {
		        return ((T[])fieldInfo.GetValue(parentObject))[GetIndexOfSerializedProperty(property)]; 
	        }
	        else
	        {
		        object obj = fieldInfo.GetValue(parentObject); 
				return (T)obj;   
	        }
        }	

        protected void SetFieldValue(SerializedProperty property, object newValue)
        {
	        if (parentObject is IList objectArray)
	        {
		        objectArray[GetIndexOfSerializedProperty(property)] = newValue; 
	        }
	        else
	        {
		        fieldInfo.SetValue(parentObject, newValue);
	        }
        }

        protected bool SetPropertyValue(SerializedProperty property, object newValue)
        {
	        if (parentObject is IList objectArray)
	        {
		        throw new NotImplementedException(); 
		        // objectArray[GetIndexOfSerializedProperty(property)]
	        }
	        else
	        {
		        throw new NotImplementedException(); 
	        }
        }
    }
}
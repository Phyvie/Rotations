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
        protected object propertyObject = null;
        protected Type propertyType = null;
        protected FieldInfo propertyFieldInfo = null; 
        
        protected List<object> propertyObjectHierarchy = new List<object>();
        protected List<Type> propertyTypeHierarchy = new List<Type>();
        protected List<FieldInfo> propertyFieldInfoHierarchy = new List<FieldInfo>(); 

        private static readonly Regex matchArrayElement = new Regex(@"^data\[(\d+)\]$");

        protected virtual void InitializePropertyNesting(SerializedProperty prop)
        {
            if (initialized)
            {
               return; 
            }

            SerializedObject serializedObject = prop.serializedObject;
            string path = prop.propertyPath;

            propertyObject = serializedObject?.targetObject;
            propertyType = propertyObject?.GetType();
			propertyObjectHierarchy.Add(propertyObject);
            propertyTypeHierarchy.Add(propertyType);
			
            if (string.IsNullOrEmpty(path) || propertyObject is null)
            {
                return; 
            }

            string[] splitPath = path.Split('.');
            Type currentlyCheckedFieldType = null;

            for (int i = 0; i < splitPath.Length; i++)
            {
                string pathNode = splitPath[i];

				//both arrays and lists implement the IList interface
				if (currentlyCheckedFieldType != null && typeof(IList).IsAssignableFrom(currentlyCheckedFieldType))
				{
					//IList items are serialized like this: `Array.data[0]`
					Debug.AssertFormat(pathNode.Equals("Array", StringComparison.Ordinal), serializedObject.targetObject, "Expected path node 'Array', but found '{0}'", pathNode);

					//just skip the `Array` part of the path
					pathNode = splitPath[++i];

					//match the `data[0]` part of the path and extract the IList item index
					Match elementMatch = matchArrayElement.Match(pathNode);
					int index;
					if (elementMatch.Success && int.TryParse(elementMatch.Groups[1].Value, out index))
					{
						IList objectArray = (IList)propertyObject;
						bool validArrayEntry = objectArray != null && index < objectArray.Count;
						
						propertyObject = validArrayEntry ? objectArray[index] : null;
						propertyType = currentlyCheckedFieldType.IsArray
							? currentlyCheckedFieldType.GetElementType()          //only set for arrays
							: currentlyCheckedFieldType.GenericTypeArguments[0];  //type of `T` in List<T>
						propertyObjectHierarchy.Add(propertyObject);
						propertyTypeHierarchy.Add(propertyType);
					}
					else
					{
						Debug.LogErrorFormat(serializedObject.targetObject, "Unexpected path format for array item: '{0}'", pathNode);
					}
					//reset fieldType, so we don't end up in the IList branch again next iteration
					currentlyCheckedFieldType = null;
				}
				else
				{
					FieldInfo field;
					Type instanceType = propertyType;
					BindingFlags fieldBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
					do
					{
						field = instanceType.GetField(pathNode, fieldBindingFlags);

						//b/c a private, serialized field of a subclass isn't directly retrievable,
						fieldBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
						//if neccessary, work up the inheritance chain until we find it.
						instanceType = instanceType.BaseType;
					}
					while (field == null && instanceType != typeof(object));

					//store object info for next iteration or to return
					propertyObject = (field == null || propertyObject == null) ? null : field.GetValue(propertyObject);
					currentlyCheckedFieldType = field == null ? null : field.FieldType;
					propertyType = currentlyCheckedFieldType;
					propertyFieldInfo = fieldInfo; 
					
					propertyObjectHierarchy.Add(propertyObject);
					propertyTypeHierarchy.Add(propertyType);
					propertyFieldInfoHierarchy.Add(field); 
				}
			}
            initialized = true;
		}

        protected void PrintHierarchy()
        {
	        Debug.Log("propertyObjectHierarchy: ");
	        foreach(object obj in propertyObjectHierarchy)
	        {
		        Debug.Log(obj);
	        }

	        Debug.Log("propertyTypeHierarchy: ");
	        foreach (Type type in propertyTypeHierarchy)
	        {
		        Debug.Log(type.FullName);
	        }

	        Debug.Log("FieldInfoHierarchy: ");
	        foreach (FieldInfo fieldInfo in propertyFieldInfoHierarchy)
	        {
		        Debug.Log(fieldInfo);
	        }
        }
    }
}
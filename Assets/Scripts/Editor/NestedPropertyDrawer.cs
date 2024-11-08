using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class NestedPropertyDrawer<T> : PropertyDrawer
    {
        private bool initialized = false;
        protected object propertyAsObject = null;

        protected T PropertyAsT
        {
	        get => GetPropertyAsT<T>();
	        set => SetPropertyToT(value); 
        }
        
        protected object parentObject => propertyObjectHierarchy[^2]; 
        protected Type propertyType = null;
        protected FieldInfo propertyFieldInfo = null;
        protected int arrayIndex = -1; 
        
        protected List<object> propertyObjectHierarchy = new List<object>();
        protected List<Type> propertyTypeHierarchy = new List<Type>();
        protected List<FieldInfo> propertyFieldInfoHierarchy = new List<FieldInfo>();
        protected List<int> propertyArrayIndicesHierarchy = new List<int>(); 
        
        private static readonly Regex matchArrayElement = new Regex(@"^data\[(\d+)\]$");

        protected virtual void InitializePropertyNesting(SerializedProperty prop)
        {
            if (initialized)
            { 
	            return; 
            }

            SerializedObject serializedObject = prop.serializedObject;
            string path = prop.propertyPath;

            propertyAsObject = serializedObject?.targetObject;
            propertyType = propertyAsObject?.GetType();
			propertyObjectHierarchy.Add(propertyAsObject);
            propertyTypeHierarchy.Add(propertyType);
			
            if (string.IsNullOrEmpty(path) || propertyAsObject is null)
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
					Debug.AssertFormat(pathNode.Equals("Array", StringComparison.Ordinal), serializedObject!.targetObject, "Expected path node 'Array', but found '{0}'", pathNode);

					//just skip the `Array` part of the path
					pathNode = splitPath[++i];

					//match the `data[0]` part of the path and extract the IList item index
					Match elementMatch = matchArrayElement.Match(pathNode);
					if (elementMatch.Success && int.TryParse(elementMatch.Groups[1].Value, out arrayIndex))
					{
						IList objectArray = (IList)propertyAsObject;
						bool validArrayEntry = objectArray != null && arrayIndex < objectArray.Count;
						
						propertyAsObject = validArrayEntry ? objectArray[arrayIndex] : null;
						propertyType = currentlyCheckedFieldType.IsArray
							? currentlyCheckedFieldType.GetElementType()          //only set for arrays
							: currentlyCheckedFieldType.GenericTypeArguments[0];  //type of `T` in List<T>
						propertyObjectHierarchy.Add(propertyAsObject);
						propertyTypeHierarchy.Add(propertyType);
						propertyArrayIndicesHierarchy.Add(arrayIndex);
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
					propertyAsObject = (field == null || propertyAsObject == null) ? null : field.GetValue(propertyAsObject);
					currentlyCheckedFieldType = field == null ? null : field.FieldType;
					propertyType = currentlyCheckedFieldType;
					propertyFieldInfo = fieldInfo;
					arrayIndex = -1; 
					
					propertyObjectHierarchy.Add(propertyAsObject);
					propertyTypeHierarchy.Add(propertyType); 
					propertyArrayIndicesHierarchy.Add(-1);
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

        protected T GetPropertyAsT<T>()
        {
	        if (!initialized)
	        {
		        Debug.LogWarning($"Called GetPropertyAs<{typeof(T).FullName}> before initializing the nested Property"); 
		        return default; 
	        }
	        
	        T returnValue; 
	        if (parentObject is null)
	        {
		        Debug.LogError($"Can't GetPropertyAs<{typeof(T).FullName}>, because parentObject is null");
		        return default; 
	        }
	        if (parentObject is IList) //TODO: check whether this can replace an IsAssignableFrom
	        {
		        IList objectArray = (IList)parentObject;
		        bool validArrayEntry = arrayIndex < objectArray.Count;
						
		        if (!validArrayEntry)
		        {
			        Debug.LogWarning($"GetPropertyAs<{typeof(T).FullName}>: OutOfBoundsException; If this happened during array initialisation or size-change everything is fine, otherwise not");
			        return default; 
		        }
		        else
		        {
			        returnValue = (T)objectArray[arrayIndex];
		        }
	        }
	        else
	        {
		        returnValue = (T)propertyFieldInfo.GetValue(parentObject); //TODO! this actually doesn't work for lists //would be nice to have smth like FieldInfo.GetArrayValue
	        }
	        
	        return returnValue; 
        }

        protected void SetPropertyToT<T>(T newValue)
        {
	        if (parentObject is null)
	        {
		        Debug.LogError($"Can't SetPropertyTo<{typeof(T).FullName}>, because parentObject is null");
		        return; 
	        }
	        if (parentObject is IList && arrayIndex != -1)
	        {
		        IList objectArray = (IList)parentObject; 
		        bool validArrayEntry = arrayIndex < objectArray.Count;

		        if (!validArrayEntry)
		        {
			        Debug.LogWarning($"SetPropertyTo<{typeof(T).FullName}>: OutOfBoundsException; If this happened during array initialisation or size-change everything is fine, otherwise not"); 
		        }
		        else
		        {
			        objectArray[arrayIndex] = newValue;   
		        }
	        }
	        else
	        {
		        propertyFieldInfo.SetValue(parentObject, newValue); 
	        }
        }
    }
}
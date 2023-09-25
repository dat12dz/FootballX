using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

public class CheckNullPropertiesChecker : MonoBehaviour
{
    
    private void Start()
    {
        // Get all loaded assemblies
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Iterate through the assemblies and find classes with the CheckNullProperties attribute
        foreach (var assembly in assemblies)
        {
            Type[] types = assembly.GetTypes()
                .Where(type => type.GetCustomAttributes(typeof(CheckNullPropertiesAttribute), true).Length > 0)
                .ToArray();

            foreach (var type in types)
            {
                // Create an instance of the class
                MonoBehaviour component = gameObject.AddComponent(type) as MonoBehaviour;

                // Use reflection to call the CheckNullProperties method if it exists
                MethodInfo methodInfo = type.GetMethod("CheckNullProperties", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (methodInfo != null)
                {
                    methodInfo.Invoke(null, new object[] { component });
                }

                // Destroy the temporary instance
                Destroy(component);
            }
        }
    }
}

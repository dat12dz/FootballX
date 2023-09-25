using UnityEngine;
using System.Reflection;

// Custom attribute to check for null properties derived from Component
[System.AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
public class CheckNullPropertiesAttribute : System.Attribute
{
    public static void CheckNullProperties(Component component)
    {
        if (component == null)
        {
            Debug.LogError("Component is null.");
            return;
        }

        // Get all fields and properties of the component type
        FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        PropertyInfo[] properties = component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            if (field.FieldType.IsSubclassOf(typeof(Component)))
            {
                Component value = (Component)field.GetValue(component);
                if (value == null)
                {
                    Debug.LogError($"Field '{field.Name}' is null.");
                }
            }
        }

        foreach (var property in properties)
        {
            if (property.PropertyType.IsSubclassOf(typeof(Component)))
            {
                Component value = (Component)property.GetValue(component, null);
                if (value == null)
                {
                    Debug.LogError($"Property '{property.Name}' is null.");
                }
            }
        }
    }
}

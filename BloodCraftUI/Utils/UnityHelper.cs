using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace BloodCraftEZLife.Utils;

public static class UnityHelper
{


    public static void ListAllComponentsWithChildren(GameObject root)
    {
        // Get all components in this object and children (true = include inactive)
        Component[] allComponents = root.GetComponentsInChildren<Component>(true);

        foreach (Component comp in allComponents)
        {
            if (comp == null) continue; // skip missing scripts
            LogUtils.LogInfo($"[{comp.GetType().Name} on {comp.gameObject.name}]");
        }
    }

    public static void ListAllComponents(GameObject root)
    {
        // Get all components in this object and children (true = include inactive)
        Component[] allComponents = root.GetComponents<Component>();

        foreach (Component comp in allComponents)
        {
            if (comp == null) continue; // skip missing scripts
            Debug.Log($"[{comp.GetType().Name} on {comp.gameObject.name}]");
        }
    }

    public static GameObject FindInHierarchy(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        string[] segments = path.Split('|');
        if (segments.Length == 0)
            return null;

        // Start with the root object
        GameObject current = GameObject.Find(segments[0]);
        if (current == null)
            return null;

        // Navigate through the hierarchy path
        for (int i = 1; i < segments.Length; i++)
        {
            Transform child = current.transform.Find(segments[i]);
            if (child == null)
                return null;

            current = child.gameObject;
        }

        return current;
    }

    public static void LogShader(Material material)
    {
        var cnt = material.shader.GetPropertyCount();
        for (int i = 0; i < cnt; i++)
        {
            var name = material.shader.GetPropertyName(i);
            var desc = material.shader.GetPropertyDescription(i);
            var attr = material.shader.GetPropertyAttributes(i);
            var flags = material.shader.GetPropertyFlags(i);
            var type = material.shader.GetPropertyType(i);
            object def = null; //
            var range = type == ShaderPropertyType.Range
                ? material.shader.GetPropertyRangeLimits(i)
                : Vector2.zero;
            switch (type)
            {
                case ShaderPropertyType.Vector:
                    def = material.shader.GetPropertyDefaultVectorValue(i);
                    break;
                case ShaderPropertyType.Float:
                    def = material.shader.GetPropertyDefaultFloatValue(i);
                    break;
                case ShaderPropertyType.Int:
                    def = material.shader.GetPropertyDefaultIntValue(i);
                    break;
            }

            LogUtils.LogInfo(
                $"Property {i}: {name} - {desc} - {(attr != null ? string.Join(',', attr.Select(a => a)) : null)} - {flags} - {type} - {def} - {range.x}:{range.y}");
        }
    }

    /// <summary>
    /// Prints the hierarchy of parents up to the root.
    /// </summary>
    public static void PrintParents(Transform t)
    {
        int depth = 0;
        Transform current = t;

        while (current != null)
        {
            string prefix = new string('-', depth);
            Debug.Log(prefix + current.gameObject.name);

            current = current.parent;
            depth++;
        }
    }
    static public void PrintChilds(Transform t, int depth, bool withcomponent = false)
    {
        // Prefix with dashes based on depth
        string prefix = new string('-', depth);

        // Print GameObject name
        Debug.Log(prefix + t.gameObject.name+ GetLabelValue(t.gameObject));
        if (withcomponent)
            ListAllComponentsWithChildren(t.gameObject);

        for (int i = 0; i < t.childCount; i++)
        {
            Transform child = t.GetChild(i);
            PrintChilds(child, depth + 1);
        }


    }
    public static string GetLabelValue(GameObject obj)
    {
        var label = obj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (label != null)
            return " Text: "+label.text;
        return "";
    }

}
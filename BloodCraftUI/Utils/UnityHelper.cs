using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace BloodCraftEZLife.Utils;

public static class UnityHelper
{

    public static void HideObject(GameObject obj)
    {
        Image BgImg = obj.GetComponent<UnityEngine.UI.Image>();
        if (BgImg != null)
        {
            Color bgColor = BgImg.color;
            bgColor.a = 0.0f;
            BgImg.color = bgColor;
        }
    }
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

    public static void DeleteComponentNamed(GameObject root,string compname)
    {
        // Get all components in this object and children (true = include inactive)
        Component[] allComponents = root.GetComponentsInChildren<Component>(true);

        for (int x = 0; x < allComponents.Count()-1;x++)
        {
            Component comp = allComponents[x];
            if (comp == null) continue; // skip missing scripts
            if (comp.name == compname)
            {
                Object.Destroy(comp.gameObject);
                return;
            }
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

    public static void DumpUI(GameObject go, int depth = 0)
    {
        if (go == null) return;

        string prefix = new string('-', depth);

        // Print GameObject name
        Debug.Log($"{prefix} GameObject: {go.name}");

        // RectTransform info
        var rect = go.GetComponent<RectTransform>();
        if (rect != null)
        {
            Debug.Log($"{prefix} RectTransform: Pos={rect.anchoredPosition}, Size={rect.sizeDelta}, Pivot={rect.pivot}");
        }

        // Image (backgrounds, icons)
        var img = go.GetComponent<Image>();
        if (img != null)
        {
            Debug.Log($"{prefix} Image: Color={img.color}, Sprite={(img.sprite != null ? img.sprite.name : "None")}, Type={img.type}");
        }

        // Text (TMP)
        var tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            Debug.Log($"{prefix} TMP_Text: Text='{tmp.text}', Font={tmp.font?.name}, Size={tmp.fontSize}, Color={tmp.color}, Alignment={tmp.alignment}");
        }

        // Text (legacy UI)
        var text = go.GetComponent<Text>();
        if (text != null)
        {
            Debug.Log($"{prefix} Text: Text='{text.text}', Font={text.font?.name}, Size={text.fontSize}, Color={text.color}, Alignment={text.alignment}");
        }

        // Buttons / Toggles / Sliders
        var btn = go.GetComponent<Button>();
        if (btn != null)
        {
            Debug.Log($"{prefix} Button: Transition={btn.transition}, Colors={btn.colors.normalColor}/{btn.colors.highlightedColor}/{btn.colors.pressedColor}");
        }

        var toggle = go.GetComponent<Toggle>();
        if (toggle != null)
        {
            Debug.Log($"{prefix} Toggle: IsOn={toggle.isOn}, Graphic={toggle.graphic?.name}");
        }

        var slider = go.GetComponent<Slider>();
        if (slider != null)
        {
            Debug.Log($"{prefix} Slider: Min={slider.minValue}, Max={slider.maxValue}, Value={slider.value}");
        }

        // Layout components
        var layout = go.GetComponent<HorizontalOrVerticalLayoutGroup>();
        if (layout != null)
        {
            Debug.Log($"{prefix} LayoutGroup: Padding={layout.padding}, Spacing={layout.spacing}");
        }

        var fitter = go.GetComponent<ContentSizeFitter>();
        if (fitter != null)
        {
            Debug.Log($"{prefix} ContentSizeFitter: HorizontalFit={fitter.horizontalFit}, VerticalFit={fitter.verticalFit}");
        }

        // Recurse into children (safe for Il2Cpp)
        for (int i = 0; i < go.transform.childCount; i++)
        {
            Transform child = go.transform.GetChild(i);
            DumpUI(child.gameObject, depth + 1);
        }
    }
}
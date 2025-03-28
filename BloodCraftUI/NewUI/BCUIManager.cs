using BloodCraftUI.NewUI.UICore.UI.Panel;
using UnityEngine;
using UIBase = BloodCraftUI.NewUI.UICore.UniverseLib.UI.UIBase;
using UniversalUI = BloodCraftUI.NewUI.UICore.UniverseLib.UI.UniversalUI;

namespace BloodCraftUI.NewUI;

public static class BCUIManager
{
    public enum Panels
    {
        Base,
        BoxList,
        BoxContent
    }

    public static UIBase UiBase { get; private set; }
    public static GameObject UIRoot => UiBase?.RootObject;
    public static ContentPanel ContentPanel { get; private set; }
    public static bool IsInitialized { get; private set; }
    
    internal static void Initialize()
    {
        UniversalUI.Init();
    }

    public static void SetupAndShowUI()
    {
        if (IsInitialized) return;
        
        UiBase = UniversalUI.RegisterUI(PluginInfo.PLUGIN_GUID, UiUpdate);
        ContentPanel = new ContentPanel(UiBase);
        SetActive(true);
        IsInitialized = true;
    }

    public static void SetActive(bool active)
    {
        if (ContentPanel == null) return;
        ContentPanel.SetActive(active);
    }

    public static void Reset()
    {
        IsInitialized = false;

        ContentPanel.Reset();
        ContentPanel.Destroy();
        Object.Destroy(UIRoot);
    }

    private static void UiUpdate()
    {
        // Called once per frame when your UI is being displayed.
    }

    public static T GetPanel<T>()
        where T : class
    {
        return ContentPanel.GetPanel<T>();
    }
}
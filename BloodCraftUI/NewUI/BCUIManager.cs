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
        Progress,
        Actions,
    }
    
    public static bool IsInitialised { get; private set; }
    
    internal static void Initialize()
    {
        UniversalUI.Init();
    }

    public static UIBase UiBase { get; private set; }
    public static GameObject UIRoot => UiBase?.RootObject;
    public static ContentPanel ContentPanel { get; private set; }

    public static void OnInitialized()
    {
        if (IsInitialised) return;
        
        UiBase = UniversalUI.RegisterUI(PluginInfo.PLUGIN_GUID, UiUpdate);
        ContentPanel = new ContentPanel(UiBase);
        SetActive(true);
    }

    public static void SetActive(bool active)
    {
        if (ContentPanel == null) return;
        ContentPanel.SetActive(active);
        SetActive(active);
        IsInitialised = true;
    }

    public static void Reset()
    {
        ContentPanel.Reset();
    }

    private static void UiUpdate()
    {
        // Called once per frame when your UI is being displayed.
    }
}
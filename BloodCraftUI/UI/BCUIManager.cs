using System;
using System.Collections.Generic;
using System.Linq;
using BloodCraftUI.NewUI.UniverseLib.UI.Panels;
using BloodCraftUI.UI.CustomLib.Panel;
using BloodCraftUI.UI.ModContent;
using UnityEngine;
using UIBase = BloodCraftUI.NewUI.UniverseLib.UI.UIBase;
using UniversalUI = BloodCraftUI.NewUI.UniverseLib.UI.UniversalUI;

namespace BloodCraftUI.NewUI;

public static class BCUIManager
{
    public enum Panels
    {
        Base,
        BoxList,
        BoxContent,
        FamStats
    }

    public static UIBase UiBase { get; private set; }
    public static GameObject UIRoot => UiBase?.RootObject;
    public static ContentPanel ContentPanel { get; private set; }
    public static bool IsInitialized { get; private set; }


    private static List<PanelBase> UIPanels { get; set; } = new();

    internal static void Initialize()
    {
        UniversalUI.Init();
    }

    public static void SetupAndShowUI()
    {
        if (IsInitialized) return;

        if (UiBase == null)
        {
            UiBase = UniversalUI.RegisterUI(PluginInfo.PLUGIN_GUID, UiUpdate);
            AddPanel(Panels.Base);
        }
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
        foreach (var value in UIPanels)
        {
            if(value is ResizeablePanelBase panel)
                panel.Reset();
            value.Destroy();
        }

        UIPanels.Clear();

        //ContentPanel.Destroy();
        //Object.Destroy(UIRoot);
        SetActive(false);
    }

    private static void UiUpdate()
    {
        // Called once per frame when your UI is being displayed.
    }

    public static void AddPanel(Panels type, string param = null)
    {
        switch (type)
        {
            case Panels.Base:
                ContentPanel = new ContentPanel(UiBase);
                break;
            case Panels.BoxList:
            {
                var panel = GetPanel<BoxListPanel>();
                if (panel == null)
                {
                    var item = new BoxListPanel(UiBase);
                    UIPanels.Add(item);
                    if (Plugin.IS_TESTING)
                    {
                        item.AddListEntry("Test 1 ");
                        item.AddListEntry("My sweet box1");
                        item.AddListEntry("My sweet box2");
                        item.AddListEntry("My sweet box3");
                        item.AddListEntry("My sweet box4");
                        item.AddListEntry("My sweet box5");
                        item.AddListEntry("My sweet box6");
                    }
                }
                else
                {
                    panel.SetActive(true);
                }

                break;
            }
            case Panels.BoxContent:
            {
                var panel = GetBoxPanel(param);
                if (panel == null)
                    UIPanels.Add(new BoxContentPanel(UiBase, param));
                else
                {
                    panel.SetActive(true);
                }
                break;
            }
            case Panels.FamStats:
            {
                var panel = GetPanel<FamStatsPanel>();
                if (panel == null)
                {
                    var item = new FamStatsPanel(UiBase);
                    UIPanels.Add(item);
                }
                else
                {
                    panel.SetActive(!panel.Enabled);
                }
            }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }


    internal static T GetPanel<T>()
        where T : class
    {
        var t = typeof(T);
        return UIPanels.FirstOrDefault(a => a.GetType() == t) as T;
    }

    internal static BoxContentPanel GetBoxPanel(string currentBox)
    {
        return UIPanels.FirstOrDefault(a => a.PanelType == Panels.BoxContent && a.PanelId.Equals(currentBox)) as
            BoxContentPanel;
    }
}
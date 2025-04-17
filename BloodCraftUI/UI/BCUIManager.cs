using System;
using System.Collections.Generic;
using System.Linq;
using BloodCraftUI.UI.CustomLib.Panel;
using BloodCraftUI.UI.ModContent;
using BloodCraftUI.UI.ModContent.Data;
using BloodCraftUI.UI.UniverseLib.UI.Panels;
using UIManagerBase = BloodCraftUI.UI.ModernLib.UIManagerBase;

namespace BloodCraftUI.UI;

public class BCUIManager : UIManagerBase
{
    private List<IPanelBase> UIPanels { get; } = new();
    private IPanelBase ContentPanel;

    public override void Reset()
    {
        base.Reset();
        foreach (var value in UIPanels)
        {
            if(value is ResizeablePanelBase panel)
                panel.Reset();
            value.Destroy();
        }

        UIPanels.Clear();
    }

    protected override void AddMainContentPanel()
    {
        AddPanel(PanelType.Base);
    }

    public override void SetActive(bool active)
    {
        ContentPanel?.SetActive(active);
    }

    public void AddPanel(PanelType type, string param = null)
    {
        switch (type)
        {
            case PanelType.Base:
                ContentPanel = new ContentPanel(UiBase);
                break;
            case PanelType.BoxList:
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
            case PanelType.BoxContent:
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
            case PanelType.FamStats:
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
            case PanelType.TestPanel:
            {
                var panel = GetPanel<TestPanel>();
                if (panel == null)
                {
                    var item = new TestPanel(UiBase);
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

    internal T GetPanel<T>()
        where T : class
    {
        var t = typeof(T);
        return UIPanels.FirstOrDefault(a => a.GetType() == t) as T;
    }

    internal BoxContentPanel GetBoxPanel(string currentBox)
    {
        return UIPanels.FirstOrDefault(a => a.PanelType == PanelType.BoxContent && a.PanelId.Equals(currentBox)) as
            BoxContentPanel;
    }
}
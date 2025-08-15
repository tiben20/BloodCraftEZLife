using System;
using System.Collections.Generic;
using System.Linq;
using BloodmoonPluginsUI.UI.CustomLib.Panel;
using BloodmoonPluginsUI.UI.ModContent;
using BloodmoonPluginsUI.UI.ModContent.Data;
using BloodmoonPluginsUI.UI.UniverseLib.UI.Panels;
using UIManagerBase = BloodmoonPluginsUI.UI.ModernLib.UIManagerBase;

namespace BloodmoonPluginsUI.UI;

public class BCUIManager : UIManagerBase
{
    private List<IPanelBase> UIPanels { get; } = new();
    public IPanelBase _contentPanel;
    private readonly List<string> _visibilityAffectedPanels = new();

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
        if (active && _visibilityAffectedPanels.Any())
        {
            foreach (var p in _visibilityAffectedPanels.Select(panel =>
                         UIPanels.FirstOrDefault(a => a.PanelId.Equals(panel))))
                p?.SetActiveOnly(true);
            _visibilityAffectedPanels.Clear();
        }

        if (!active)
        {
            foreach (var panel in UIPanels.Where(a => a.Enabled))
            {
                _visibilityAffectedPanels.Add(panel.PanelId);
                panel.SetActiveOnly(false);
            }
        }

        _contentPanel?.SetActive(active);
    }

    public void AddPanel(PanelType type, string param = null)
    {
        switch (type)
        {
            case PanelType.Base:
                _contentPanel = new ContentPanel(UiBase);
                break;
            case PanelType.TeleportList:
            {
                var panel = GetPanel<TeleportListPanel>();
                if (panel == null)
                {
                    var item = new TeleportListPanel(UiBase);
                    UIPanels.Add(item);
                }
                else
                {
                    panel.SetActive(true);
                }

                break;
            }
            case PanelType.SettingsPanel:
                {
                    var panel = GetPanel<SettingsPanel>();
                    if (panel == null)
                    {
                        var item = new SettingsPanel(UiBase);
                        UIPanels.Add(item);
                    }
                    else
                    {
                        panel.SetActive(true);
                    }

                    break;
                }
            case PanelType.PullPanel:
                {
                    var panel = GetPanel<PullItemsPanel>();
                    if (panel == null)
                    {
                        var item = new PullItemsPanel(UiBase);
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
}
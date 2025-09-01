using System;
using System.Collections.Generic;
using System.Linq;
using BloodCraftEZLife.UI.ModContent;
using BloodCraftEZLife.UI.ModContent.Data;
using BloodCraftEZLife.UI.UniverseLib.UI.Panels;
using BloodCraftEZLife.Utils;
using BloodCraftEZLife.Config;
using UIManagerBase = BloodCraftEZLife.UI.ModernLib.UIManagerBase;
using BloodCraftEZLife.UI.ModContent.CustomElements;

namespace BloodCraftEZLife.UI;

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
        _contentPanel = new ContentPanel(UiBase);
        ClientActivate(false);
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

    public void AdjustOpacity()
    {
        foreach (var pan in UIPanels)
        {
            
            if (pan is PanelBase panel)
            { 
                var canvasGroup = panel.ContentRoot.GetComponent<UnityEngine.UI.Image>();
                if (canvasGroup != null)
                    canvasGroup.color = new UnityEngine.Color(canvasGroup.color.r, canvasGroup.color.g, canvasGroup.color.b).GetTransparent(Settings.UITransparency);
            }

        }
       
    }

    public void ClientActivate(bool active)
    {
        ContentPanel cpan = (ContentPanel)_contentPanel;
        cpan.ToggleGameObject(active, "TeleportListButton");
        cpan.ToggleGameObject(active, "PullButton");
        cpan.ToggleGameObject(active, "ChatButton");
    }

    internal T GetPanel<T>()
    where T : class, IPanelBase
    {
        // Try to find existing panel
        if (UiBase == null)
            return null;
        var panel = UIPanels.OfType<T>().FirstOrDefault();
        if (panel != null)
            return panel;
        

        panel = PanelFactory<T>();
        // Create new instance with a factory map
        if (typeof(T) != typeof(ContentPanel))
            panel.SetActive(false);
        UIPanels.Add(panel);
        return panel;
    }

    // Factory resolver for known panel types
    private T PanelFactory<T>() where T : class, IPanelBase
    {
        if (typeof(T) == typeof(ContentPanel))
            return _contentPanel as T;
        if (typeof(T) == typeof(PopupPanel))
            return new PopupPanel(UiBase) as T;
        if (typeof(T) == typeof(PullItemsPanel))
            return new PullItemsPanel(UiBase) as T;
        if (typeof(T) == typeof(CommandInput))
            return new CommandInput(UiBase) as T;
        if (typeof(T) == typeof(HotkeysPanel))
            return new HotkeysPanel(UiBase) as T;
        if (typeof(T) == typeof(ChatPanel))
            return new ChatPanel(UiBase) as T;
        if (typeof(T) == typeof(SettingsPanel))
            return new SettingsPanel(UiBase) as T;
        if (typeof(T) == typeof(TeleportListPanel))
            return new TeleportListPanel(UiBase) as T;
        
        throw new InvalidOperationException($"No factory defined for {typeof(T).Name}");
    }

}
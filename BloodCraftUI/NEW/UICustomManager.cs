using System.Collections.Generic;
using BloodCraftUI.NEW.Panels;
using BloodCraftUI.Services;
using BloodCraftUI.UI;
using BloodCraftUI.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace BloodCraftUI.NEW
{
    internal class UICustomManager
    {
        public enum VerticalAnchor
        {
            Top,
            Bottom
        }

        internal static UIBase UiBase { get; private set; }
        public static GameObject UIRoot => UiBase?.RootObject;
        public static RectTransform UIRootRect { get; private set; }
        public static Canvas UICanvas { get; private set; }
        
        private static readonly Vector2 NAVBAR_DIMENSIONS = new(1610f, 35f);
        public static RectTransform NavBarRect;
        public static VerticalAnchor NavbarAnchor = VerticalAnchor.Top;
        public static GameObject NavbarTabButtonHolder;

        internal static readonly Dictionary<PanelType, UEPanel> UIPanels = new();
        internal static readonly Dictionary<string, UEPanel> UIContentPanels = new();
        private static int lastScreenWidth;
        private static int lastScreenHeight;
        public static bool Initializing { get; set; } = true;

        internal static void InitUI()
        {
            UiBase = UiBase ?? UniversalUI.RegisterUI<BloodCraftUIBase>(PluginInfo.PLUGIN_GUID, Update);
            UIRootRect = UIRoot.GetComponent<RectTransform>();
            UICanvas = UIRoot.GetComponent<Canvas>();
            
            DisplayManager.Init();
            lastScreenWidth = DisplayManager.ActiveDisplay.renderingWidth;
            lastScreenHeight = DisplayManager.ActiveDisplay.renderingHeight;
            
            CreateTopNavBar();
            UIPanels.Add(PanelType.BoxList, new BoxListModal(UiBase));

            foreach (var dropdown in UIRoot.GetComponentsInChildren<Dropdown>(true))
                dropdown.RefreshShownValue();

            ShowMenu = true;
            Initializing = false;
        }

        public static void AddBoxContentPanel(string name)
        {
            if (UIContentPanels.TryGetValue(name, out var panel))
            {
                panel.SetActive(true);
                return;
            }
            UIContentPanels.Add(name, new BoxContentPanel(UiBase, name));
        }

        private static void Update()
        {
            if (!UIRoot)
                return;

            if (DisplayManager.ActiveDisplay.renderingWidth != lastScreenWidth || DisplayManager.ActiveDisplay.renderingHeight != lastScreenHeight)
                OnScreenDimensionsChanged();

        }

        public static bool ShowMenu
        {
            get => UiBase is { Enabled: true };
            set
            {
                if (UiBase == null || !UIRoot || UiBase.Enabled == value)
                    return;

                UniversalUI.SetUIActive(PluginInfo.PLUGIN_GUID, value);
            }
        }

        private static void OnScreenDimensionsChanged()
        {
            var display = DisplayManager.ActiveDisplay;
            lastScreenWidth = display.renderingWidth;
            lastScreenHeight = display.renderingHeight;

            foreach (var panel in UIPanels)
            {
                panel.Value.EnsureValidSize();
                panel.Value.EnsureValidPosition();
                panel.Value.Dragger.OnEndResize();
            }

            foreach (var panel in UIContentPanels)
            {
                panel.Value.EnsureValidSize();
                panel.Value.EnsureValidPosition();
                panel.Value.Dragger.OnEndResize();
            }
        }
        private static void CreateTopNavBar()
        {
            var navbarPanel = UIFactory.CreateUIObject("MainNavbar", UIRoot);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(navbarPanel, false, false, true, true, 5, 4, 4, 4, 4, TextAnchor.MiddleCenter);
            navbarPanel.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f);
            NavBarRect = navbarPanel.GetComponent<RectTransform>();
            NavBarRect.pivot = new Vector2(0.5f, 1f);

            NavbarAnchor = VerticalAnchor.Top;
            SetNavBarAnchor();

            // Title
            var titleTxt = $"BCUI <i><color=grey>{PluginInfo.PLUGIN_VERSION}</color></i>";
            var title = UIFactory.CreateLabel(navbarPanel, "Title", titleTxt, TextAnchor.MiddleCenter, default, true, 14);
            UIFactory.SetLayoutElement(title.gameObject, minWidth: 75, flexibleWidth: 0);

            // panel tabs

            NavbarTabButtonHolder = UIFactory.CreateUIObject("NavTabButtonHolder", navbarPanel);
            UIFactory.SetLayoutElement(NavbarTabButtonHolder, minHeight: 25, flexibleHeight: 999, flexibleWidth: 999);
            UIFactory.SetLayoutGroup<HorizontalLayoutGroup>(NavbarTabButtonHolder, false, true, true, true, 4, 2, 2, 2, 2);

            //spacer
            var spacer = UIFactory.CreateUIObject("Spacer", navbarPanel);
            UIFactory.SetLayoutElement(spacer, minWidth: 15);
        }

        private static void SetNavBarAnchor()
        {
            switch (NavbarAnchor)
            {
                case VerticalAnchor.Top:
                    NavBarRect.anchorMin = new Vector2(0.5f, 1f);
                    NavBarRect.anchorMax = new Vector2(0.5f, 1f);
                    NavBarRect.anchoredPosition = new Vector2(NavBarRect.anchoredPosition.x, 0);
                    NavBarRect.sizeDelta = NAVBAR_DIMENSIONS;
                    break;

                case VerticalAnchor.Bottom:
                    NavBarRect.anchorMin = new Vector2(0.5f, 0f);
                    NavBarRect.anchorMax = new Vector2(0.5f, 0f);
                    NavBarRect.anchoredPosition = new Vector2(NavBarRect.anchoredPosition.x, 35);
                    NavBarRect.sizeDelta = NAVBAR_DIMENSIONS;
                    break;
            }
        }

        public static UEPanel GetPanel(PanelType panel) => UIPanels[panel];
        public static T GetPanel<T>(PanelType panel) where T : UEPanel => (T)UIPanels[panel];
        public static T GetPanel<T>(string name) where T : UEPanel => (T)UIContentPanels[name];
        public static void TogglePanel(PanelType panel)
        {
            if (panel == PanelType.BoxContent)
            {
                return;
            }

            UEPanel uiPanel = GetPanel(panel);
            SetPanelActive(panel, !uiPanel.Enabled);
        }

        public static void TogglePanel(string id)
        {
            var uiPanel = UIContentPanels[id];
            uiPanel.SetActive(!uiPanel.Enabled);
        }

        public static void SetPanelActive(PanelType panelType, bool active)
        {
            GetPanel(panelType).SetActive(active);
        }

        public static void SetPanelActive(UEPanel panel, bool active)
        {
            panel.SetActive(active);
        }

        public static void Destroy()
        {
            UIPanels.ForEach(a=> a.Value.Destroy());
            UIPanels.Clear();
            UIContentPanels.ForEach(a => a.Value.Destroy());
            UIContentPanels.Clear();

            ShowMenu = false;
            Initializing = true;
            MessageService.Destroy();
        }
    }

    public enum PanelType
    {
        BoxList,
        BoxContent
    }
}

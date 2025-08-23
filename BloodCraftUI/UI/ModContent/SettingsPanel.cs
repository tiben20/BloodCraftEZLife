using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BloodCraftEZLife.Config;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.UI.CustomLib;
using BloodCraftEZLife.UI.CustomLib.Cells;
using BloodCraftEZLife.UI.CustomLib.Cells.Handlers;
using BloodCraftEZLife.UI.CustomLib.Panel;
using BloodCraftEZLife.UI.CustomLib.Util;
using BloodCraftEZLife.UI.ModContent.Data;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.UI.UniverseLib.UI.Panels;
using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;
using BloodCraftEZLife.Utils;
using ProjectM;
using UnityEngine;
using UnityEngine.UI;

namespace BloodCraftEZLife.UI.ModContent
{
    internal class SettingsPanel : PanelBase
    {
        public override string PanelId => "SettingsList";
        public override int MinWidth => (int)480;
        public override int MinHeight => (int)240;
        public override int MaxWidth => (int)3840;
        
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 0.5f);

        public override bool CanDrag => false;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.None;
        public override PanelType PanelType => PanelType.SettingsPanel;

        public GameObject PanelSettingsContent { get; protected set; }

        private ScrollPool<ConfigboxCell> _scrollPool;
        private ConfigCellHandler<Setting, ConfigboxCell> _scrollDataHandler;
        private bool _isInitialized;

        public SettingsPanel(UIBase owner) : base(owner)
        {
            SetTitle("Settings");
        }


        protected override void LateConstructUI()
        {
            base.LateConstructUI();
            float xfact = Owner.Scaler.referenceResolution.x / 1920;
            float yfact = Owner.Scaler.referenceResolution.y / 1080;
            //put sizing here

            Vector2 newRect =new Vector2(FullscreenSettingService.DeltaRect.x*xfact, FullscreenSettingService.DeltaRect.y * yfact);
            Rect.sizeDelta = newRect;

            Rect.anchoredPosition = FullscreenSettingService.AnchorRect;
            EnsureValidSize();
            EnsureValidPosition();
        }

        protected override void OnClosePanelClicked()
        {
            SetActive(false);
        }

        protected override void ConstructPanelContent()
        {
            PanelSettingsContent = UIFactory.CreateVerticalGroup(ContentRoot, "HotkeyVerticalLayout", false, true, true, true, 2,
                new Vector4(2, 2, 2, 2), Theme.PanelBackground);
            UIFactory.SetLayoutElement(PanelSettingsContent, minHeight: 25, flexibleHeight: 0);

            _scrollDataHandler = new ConfigCellHandler<Setting, ConfigboxCell>(_scrollPool, Settings.GetSettingsEntries, SetCell, ShouldDisplay, OnCellChanged);
            _scrollPool = UIFactory.CreateScrollPool<ConfigboxCell>(PanelSettingsContent, "ContentList", out GameObject scrollObj,
                out _, new Color32(0,1,3,255));
            _scrollPool.Initialize(_scrollDataHandler);
            UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);
            var btnHotkey = UIFactory.CreateButton(PanelSettingsContent, "btnHotkey", "Hotkeys", new ColorBlock
            {
                normalColor = new Color(0.11f, 0.11f, 0.11f).GetTransparent(Settings.UITransparency),
                disabledColor = new Color(0.3f, 0.3f, 0.3f).GetTransparent(Settings.UITransparency),
                highlightedColor = new Color(0.16f, 0.16f, 0.16f).GetTransparent(Settings.UITransparency),
                pressedColor = new Color(0.05f, 0.05f, 0.05f).GetTransparent(Settings.UITransparency)
            });
            UIFactory.SetLayoutElement(btnHotkey.Component.gameObject, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);

            btnHotkey.OnClick += () =>
            {
                var panel = Plugin.UIManager.GetPanel<HotkeysPanel>();
                if (panel != null)
                {
                    panel.SetActive(true);
                    return;

                }
                Plugin.UIManager.AddPanel(PanelType.HotkeysPanel);
            };

            RefreshData();
        }

        

        internal void Reset()
        {
            //_scrollDataHandler.RefreshData();
            //_scrollPool.Refresh(true);
        }


        public override void SetActive(bool active)
        {
           
            _isInitialized = true;
            base.SetActive(active);
        }

        #region ScrollPool handling

        public void RefreshData()
        {
            _scrollDataHandler.RefreshData();
            _scrollPool.Refresh(true);
        }

        private void OnCellChanged(Setting newValue)
        {
            Settings.SetConfigValue(newValue.ConfigName, newValue.Value);
            return;
        }

        private bool ShouldDisplay(Setting data, string filter) => true;

        private void SetCell(ConfigboxCell cell, int index)
        {
            if (index < 0 || index >= Settings._settingList.Count)
            {
                cell.Disable();
                return;
            }
            cell.InitialiseSetting(Settings._settingList[index]);
        }

        #endregion
    }
}
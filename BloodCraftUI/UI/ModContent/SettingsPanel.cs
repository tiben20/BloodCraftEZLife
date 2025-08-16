using System.Collections.Generic;
using System.Linq;
using BloodCraftEZLife.Config;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.UI.CustomLib.Cells;
using BloodCraftEZLife.UI.CustomLib.Cells.Handlers;
using BloodCraftEZLife.UI.CustomLib.Panel;
using BloodCraftEZLife.UI.ModContent.Data;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.UI.UniverseLib.UI.Panels;
using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;
using BloodCraftEZLife.Utils;
using UnityEngine;

namespace BloodCraftEZLife.UI.ModContent
{
    internal class SettingsPanel : ResizeablePanelBase
    {
        public override string PanelId => "SettingsList";
        public override int MinWidth => 180;
        public override int MinHeight => 120;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 0.5f);
        public override bool CanDrag => true;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.All;
        public override PanelType PanelType => PanelType.SettingsPanel;

        private ScrollPool<ConfigboxCell> _scrollPool;
        private ConfigCellHandler<Setting, ConfigboxCell> _scrollDataHandler;
        private bool _isInitialized;

        public SettingsPanel(UIBase owner) : base(owner)
        {
            SetTitle("Settings");
        }

        public void AddListEntry(string name,bool value)
        {
            /*if (Settings._settingList.Any(a => a.Name.Equals(name)))
                return;
            Settings._settingList.Add(new Setting { Name = name, Value=value });*/
            _scrollDataHandler.RefreshData();
            _scrollPool.Refresh(true);
        }

        protected override void LateConstructUI()
        {
            base.LateConstructUI();
            RunUpdateCommand();
        }

        protected override void OnClosePanelClicked()
        {
            SetActive(false);
        }

        protected override void ConstructPanelContent()
        {
            _scrollDataHandler = new ConfigCellHandler<Setting, ConfigboxCell>(_scrollPool, Settings.GetSettingsEntries, SetCell, ShouldDisplay, OnCellChanged);
            _scrollPool = UIFactory.CreateScrollPool<ConfigboxCell>(ContentRoot, "ContentList", out GameObject scrollObj,
                out _, new Color(0.03f, 0.03f, 0.03f, Settings.UITransparency));
            _scrollPool.Initialize(_scrollDataHandler);
            UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);

            RefreshData();
        }

        internal override void Reset()
        {
            //Object.Destroy(UIRoot);
            //_dataList.Clear();
            _scrollDataHandler.RefreshData();
            _scrollPool.Refresh(true);
        }

        private void RunUpdateCommand()
        {
            Reset();

        }

        public void RefreshData()
        {
            _scrollDataHandler.RefreshData();
            _scrollPool.Refresh(true);
        }

        private void EnableAllButtons(bool value)
        {
            //todo
            //foreach (var a in _scrollPool.CellPool)
            //    a.Button.Component.interactable = value;
        }

        public override void SetActive(bool active)
        {
            var shouldUpdateData = _isInitialized && active && Enabled == false;
            _isInitialized = true;
            base.SetActive(active);
            if (shouldUpdateData)
                RunUpdateCommand();
        }

        #region ScrollPool handling


        private void SetOpacity()
        {
            ContentPanel panel;
            panel = (ContentPanel)Plugin.UIManager._contentPanel;
            if (panel != null)
            {
                var canvasGroup = panel.ContentRoot.GetComponent<UnityEngine.UI.Image>();
                canvasGroup.color = new Color(canvasGroup.color.r, canvasGroup.color.g, canvasGroup.color.b).GetTransparent(Settings.UITransparency);
            }
            foreach (ConfigboxCell cell in _scrollPool.CellPool)
            {
                cell.SetOpacity(Settings.UITransparency);
            }
        }

        private void OnCellChanged(Setting newValue, int dataIndex)
        {
            var curSetting = Settings._settingList[dataIndex];
            ContentPanel panel;
            switch ((Settings.SettingsId)dataIndex)
            {
                case Settings.SettingsId.TpPanel:
                    Settings.IsTeleportPanelEnabled = (bool)newValue.Value;
                    panel = (ContentPanel)Plugin.UIManager._contentPanel;
                    if (panel != null)
                    {
                        panel.ToggleGameObject((bool)newValue.Value, "TeleportListButton");
                    }
                    break;
                case Settings.SettingsId.AutoTp:
                    Settings.IsAutoTeleportEnabled = (bool)newValue.Value;
                    break;
                case Settings.SettingsId.Header:
                    Settings.IsHeaderVisible = (bool)newValue.Value;
                    panel = (ContentPanel)Plugin.UIManager._contentPanel;
                    if (panel != null)
                    {
                        panel.ToggleGameObject((bool)newValue.Value, "header");
                    }
                    break;
                case Settings.SettingsId.Opacity:
                    Settings.UITransparency = (float)newValue.Value;
                    SetOpacity();
                    break; 
                default:
                    break;
            }
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
            
            cell.SetValue(Settings._settingList[index]);



        }

        #endregion
    }
}

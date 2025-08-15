using System.Collections.Generic;
using System.Linq;
using BloodmoonPluginsUI.Config;
using BloodmoonPluginsUI.Services;
using BloodmoonPluginsUI.UI.CustomLib.Cells;
using BloodmoonPluginsUI.UI.CustomLib.Cells.Handlers;
using BloodmoonPluginsUI.UI.CustomLib.Panel;
using BloodmoonPluginsUI.UI.ModContent.Data;
using BloodmoonPluginsUI.UI.UniverseLib.UI;
using BloodmoonPluginsUI.UI.UniverseLib.UI.Models;
using BloodmoonPluginsUI.UI.UniverseLib.UI.Panels;
using BloodmoonPluginsUI.UI.UniverseLib.UI.Widgets.ScrollView;
using BloodmoonPluginsUI.Utils;
using UnityEngine;

namespace BloodmoonPluginsUI.UI.ModContent
{
    internal class SettingsPanel : ResizeablePanelBase
    {
        public override string PanelId => "SettingsList";
        public override int MinWidth => 340;
        public override int MinHeight => 180;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 1f);
        public override bool CanDrag => true;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.All;
        public override PanelType PanelType => PanelType.SettingsPanel;
        public override float Opacity => Settings.UITransparency;
        
        public SettingsPanel(UIBase owner) : base(owner)
        {
            SetTitle("Settings");
        }

        public void AddListEntry(string name,bool value)
        {
            if (Settings._settingList.Any(a => a.Name.Equals(name)))
                return;
            Settings._settingList.Add(new Setting { Name = name, Value=value });
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
            _scrollDataHandler = new CheckBoxHandler<Setting, CheckboxCell>(_scrollPool, Settings.GetSettingsEntries, SetCell, ShouldDisplay, OnCellChanged);
            _scrollPool = UIFactory.CreateScrollPool<CheckboxCell>(ContentRoot, "ContentList", out GameObject scrollObj,
                out _, new Color(0.03f, 0.03f, 0.03f, Opacity));
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

        private ScrollPool<CheckboxCell> _scrollPool;
        private CheckBoxHandler<Setting, CheckboxCell> _scrollDataHandler;
        private bool _isInitialized;

        

        private void OnCellChanged(int dataIndex,bool newValue)
        {
            var curSetting = Settings._settingList[dataIndex];
            ContentPanel panel;
            switch (dataIndex)
            {
                case 0:
                    Settings.IsTeleportPanelEnabled = newValue;
                    panel = (ContentPanel)Plugin.UIManager._contentPanel;
                    if (panel != null)
                    {
                        panel.ToggleGameObject(newValue, "TeleportListButton");
                    }
                    break;
                case 1:
                    Settings.IsAutoTeleportEnabled = newValue;
                    break;
                case 2:
                    Settings.IsHeaderVisible = newValue;
                    panel = (ContentPanel)Plugin.UIManager._contentPanel;
                    if (panel != null)
                    {
                        panel.ToggleGameObject(newValue,"header");
                    }
                    break;
                default:
                    break;
            }
        }

        private bool ShouldDisplay(Setting data, string filter) => true;
        

        private void SetCell(CheckboxCell cell, int index)
        {
            if (index < 0 || index >= Settings._settingList.Count)
            {
                cell.Disable();
                return;
            }
            cell.Checkbox.Text.text = Settings._settingList[index].Name;
            cell.Checkbox.Toggle.Set(Settings._settingList[index].Value, false);
            //cell.Button.ButtonText.text = Settings._settingList[index].Name;

        }

        #endregion
    }
}

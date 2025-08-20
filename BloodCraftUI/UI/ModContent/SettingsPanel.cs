using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BloodCraftEZLife.Config;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.UI.CustomLib;
using BloodCraftEZLife.UI.CustomLib.Cells;
using BloodCraftEZLife.UI.CustomLib.Cells.Handlers;
using BloodCraftEZLife.UI.CustomLib.Panel;
using BloodCraftEZLife.UI.ModContent.Data;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.UI.UniverseLib.UI.Panels;
using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;
using BloodCraftEZLife.Utils;
using ProjectM;
using UnityEngine;

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
            //put sizing here
            Rect.sizeDelta = FullscreenSettingService.DeltaRect;
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
            _scrollDataHandler = new ConfigCellHandler<Setting, ConfigboxCell>(_scrollPool, Settings.GetSettingsEntries, SetCell, ShouldDisplay, OnCellChanged);
            _scrollPool = UIFactory.CreateScrollPool<ConfigboxCell>(ContentRoot, "ContentList", out GameObject scrollObj,
                out _, new Color32(10,10,10,255));
            _scrollPool.Initialize(_scrollDataHandler);
            UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);



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
            
            cell.SetValue(Settings._settingList[index]);



        }

        #endregion
    }
}
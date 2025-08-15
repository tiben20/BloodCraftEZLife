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
using static BloodCraftEZLife.UI.ModContent.PullItemsPanel;

namespace BloodCraftEZLife.UI.ModContent
{
    internal class BossPanel : ResizeablePanelBase
    {
        public override string PanelId => "BossPanel";
        public override int MinWidth => 340;
        public override int MinHeight => 180;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 1f);
        public override bool CanDrag => true;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.All;
        public override PanelType PanelType => PanelType.BossPanel;
        public override float Opacity => Settings.UITransparency;

        private readonly List<PullItemData> _items = new List<PullItemData>();
        private List<PullItemData> GetBossEntries() => _items;



        public BossPanel(UIBase owner) : base(owner)
        {
            SetTitle("Boss List");
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
            _scrollDataHandler = new ButtonListHandler<TeleportBoxData, ButtonCell>(_scrollPool, Settings.GetTeleportEntries, SetCell, ShouldDisplay, OnCellClicked);
            _scrollPool = UIFactory.CreateScrollPool<ButtonCell>(ContentRoot, "BossList", out GameObject scrollObj,
                out _, new Color(0.03f, 0.03f, 0.03f, Opacity));
            _scrollPool.Initialize(_scrollDataHandler);
            UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);
            RefreshData();
        }

        internal override void Reset()
        {
            //Object.Destroy(UIRoot);
            Settings._dataList.Clear();
            _scrollDataHandler.RefreshData();
            _scrollPool.Refresh(true);
        }

        public void RefreshData()
        {
            _scrollDataHandler.RefreshData();
            _scrollPool.Refresh(true);
        }

        private void RunUpdateCommand()
        {

        }

        private void EnableAllButtons(bool value)
        {
            foreach (var a in _scrollPool.CellPool)
                a.Button.Component.interactable = value;
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

        private ScrollPool<ButtonCell> _scrollPool;
        private ButtonListHandler<TeleportBoxData, ButtonCell> _scrollDataHandler;

        private bool _isInitialized;


        private void OnCellClicked(int dataIndex)
        {
            var famBox = Settings._dataList[dataIndex];

            MessageService.EnqueueMessage(".stp tpr " + famBox.Name);
        }

        private bool ShouldDisplay(TeleportBoxData data, string filter) => true;


        private void SetCell(ButtonCell cell, int index)
        {
            if (index < 0 || index >= Settings._dataList.Count)
            {
                cell.Disable();
                return;
            }
            cell.Button.ButtonText.text = Settings._dataList[index].Name;
        }

        #endregion
    }
}

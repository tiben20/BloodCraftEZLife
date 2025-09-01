using BloodCraftEZLife.Config;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.UI.ModContent.CustomElements;
using BloodCraftEZLife.UI.ModContent.CustomElements.Handlers;
using BloodCraftEZLife.UI.ModContent.Data;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Panels;
using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;
using UnityEngine;

namespace BloodCraftEZLife.UI.ModContent
{
    internal class TeleportListPanel : ResizeablePanelBase
    {
        public override string PanelId => "TeleportList";
        public override int MinWidth => 180;
        public override int MinHeight => 180;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 0.5f);
        public override bool CanDrag => true;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.All;
        public override PanelType PanelType => PanelType.TeleportList;
        public override float Opacity => Settings.UITransparency;

        private ScrollPool<ButtonCell> _scrollPool;
        private ButtonListHandler<TeleportsService.TeleportBoxData, ButtonCell> _scrollDataHandler;
        private bool _isInitialized;

        public TeleportListPanel(UIBase owner) : base(owner)
        {
            SetTitle("Teleport List");
        }

        

        protected override void LateConstructUI()
        {
            base.LateConstructUI();
            
        }

        protected override void OnClosePanelClicked()
        {
            SetActive(false);
        }

        protected override void ConstructPanelContent()
        {
            _scrollDataHandler = new ButtonListHandler<TeleportsService.TeleportBoxData, ButtonCell>(_scrollPool, TeleportsService.GetTeleportEntries, SetCell, ShouldDisplay, OnCellClicked);
            _scrollPool = UIFactory.CreateScrollPool<ButtonCell>(ContentRoot, "TeleportList", out GameObject scrollObj,
                out _, new Color(0.03f, 0.03f, 0.03f, Opacity));
            _scrollPool.Initialize(_scrollDataHandler);
            UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);

            //ProjectM.ButtonInputAction.ClanMenu
            //ClanMenu
            RefreshData();
        }

        internal override void Reset()
        {
            //Object.Destroy(UIRoot);
            TeleportsService._dataList.Clear();
            _scrollDataHandler.RefreshData();
            _scrollPool.Refresh(true);
        }

        public void RefreshData()
        {
            _scrollDataHandler.RefreshData();
            _scrollPool.Refresh(true);
        }

        public override void SetActive(bool active)
        {
            var shouldUpdateData = _isInitialized && active && Enabled == false;
            _isInitialized = true;
            base.SetActive(active);
        }

        #region ScrollPool handling

        

        private void OnCellClicked(int dataIndex)
        {
            var famBox = TeleportsService._dataList[dataIndex];
            if (famBox.Name == "Open social menu")
                return;
            
            var panel = Plugin.UIManager.GetPanel<PopupPanel>();
            if (panel != null)
            {
                panel.ShowMessage("Teleporting to "+famBox.Name, 3f, PopupPanel.MessageType.Small);
            }
            MessageService.EnqueueMessage(".stp tpr " + famBox.Name);
        }

        private bool ShouldDisplay(TeleportsService.TeleportBoxData data, string filter) => true;
        

        private void SetCell(ButtonCell cell, int index)
        {
            if (index < 0 || index >= TeleportsService._dataList.Count)
            {
                cell.Disable();
                return;
            }
            cell.Button.ButtonText.text = TeleportsService._dataList[index].Name;
            cell.IconImage.gameObject.SetActive(false);
        }

        #endregion
    }
}

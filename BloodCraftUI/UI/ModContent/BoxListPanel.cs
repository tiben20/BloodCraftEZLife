using System.Collections.Generic;
using System.Linq;
using System.Timers;
using BloodCraftUI.Config;
using BloodCraftUI.NewUI.UniverseLib.UI;
using BloodCraftUI.NewUI.UniverseLib.UI.Models;
using BloodCraftUI.NewUI.UniverseLib.UI.Panels;
using BloodCraftUI.NewUI.UniverseLib.UI.Widgets.ScrollView;
using BloodCraftUI.Services;
using BloodCraftUI.UI.CustomLib.Cells;
using BloodCraftUI.UI.CustomLib.Cells.Handlers;
using BloodCraftUI.UI.CustomLib.Panel;
using UnityEngine;

namespace BloodCraftUI.NewUI.ModContent
{
    internal class BoxListPanel : ResizeablePanelBase
    {
        public override string PanelId => "BoxList";
        public override int MinWidth => 340;
        public override int MinHeight => 180;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 1f);
        public override bool CanDrag => true;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.All;
        public override BCUIManager.Panels PanelType => BCUIManager.Panels.BoxList;

        private ButtonRef _updateButton;

        public BoxListPanel(UIBase owner) : base(owner)
        {
            SetTitle("Box List");
        }

        public void AddListEntry(string name)
        {
            if (_dataList.Any(a => a.Name.Equals(name)))
                return;
            _dataList.Add(new FamBoxData { Name = name });
            _scrollDataHandler.RefreshData();
            _scrollPool.Refresh(true);
        }

        protected override void LateConstructUI()
        {
            base.LateConstructUI();
            RunUpdateCommand();
        }

        public void RunUpdateCommand()
        {
            SendCommand();
        }

        protected override void OnClosePanelClicked()
        {
            SetActive(false);
        }

        protected override void ConstructPanelContent()
        {
            var horGroup = UIFactory.CreateHorizontalGroup(ContentRoot, "ButtonGroup", true, false, true, false, 3,
                default, new Color(1, 1, 1, 0), TextAnchor.MiddleLeft);
            _updateButton = UIFactory.CreateButton(horGroup, "butPopulate", "Populate List Boxes");
            UIFactory.SetLayoutElement(_updateButton.GameObject, minWidth: 250, minHeight: 25, flexibleWidth: 9999);
            _updateButton.OnClick += RunUpdateCommand;

            _scrollDataHandler = new ButtonListHandler<FamBoxData, ButtonCell>(_scrollPool, GetEntries, SetCell, ShouldDisplay, OnCellClicked);
            _scrollPool = UIFactory.CreateScrollPool<ButtonCell>(ContentRoot, "ContentList", out GameObject scrollObj,
                out _, new Color(0.03f, 0.03f, 0.03f));
            _scrollPool.Initialize(_scrollDataHandler);
            UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);
        }

        internal override void Reset()
        {
            //Object.Destroy(UIRoot);
            _dataList.Clear();
            _scrollDataHandler.RefreshData();
            _scrollPool.Refresh(true);
        }

        private void SendCommand()
        {
            EnableAllButtons(false);
            MessageService.EnqueueMessage(MessageService.BCCOM_LISTBOXES1);
            var t = new Timer(3000) { AutoReset = false };
            t.Elapsed += (_, _) => EnableAllButtons(true);
            t.Start();
        }

        private void EnableAllButtons(bool value)
        {
            _updateButton.Component.interactable = value;
            foreach (var a in _scrollPool.CellPool)
                a.Button.Component.interactable = value;
        }

        #region ScrollPool handling

        private ScrollPool<ButtonCell> _scrollPool;
        private ButtonListHandler<FamBoxData, ButtonCell> _scrollDataHandler;
        private readonly List<FamBoxData> _dataList = new();

        private class FamBoxData
        {
            public string Name { get; set; }
        }

        private void OnCellClicked(int dataIndex)
        {
            var famBox = _dataList[dataIndex];
            BCUIManager.AddPanel(BCUIManager.Panels.BoxContent, famBox.Name);
        }

        private bool ShouldDisplay(FamBoxData data, string filter) => true;
        private List<FamBoxData> GetEntries() => _dataList;

        private void SetCell(ButtonCell cell, int index)
        {
            if (index < 0 || index >= _dataList.Count)
            {
                cell.Disable();
                return;
            }
            cell.Button.ButtonText.text = _dataList[index].Name;
        }

        #endregion
    }
}

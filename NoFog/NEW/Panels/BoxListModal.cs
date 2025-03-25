using System.Collections.Generic;
using System.Timers;
using NoFog.Services;
using NoFog.UI;
using UnityEngine;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Widgets.ButtonList;
using UniverseLib.UI.Widgets.ScrollView;

namespace NoFog.NEW.Panels
{
    internal class BoxListModal : UEPanel
    {
        public override string Name => "Box List";
        public override PanelType PanelType => PanelType.BoxList;
        public override int MinWidth => 250;
        public override int MinHeight => 100;
        public override Vector2 DefaultAnchorMin => new(0.4f, 0.4f);
        public override Vector2 DefaultAnchorMax => new(0.6f, 0.6f);
        public override bool NavButtonWanted => true;
        public override bool ShouldSaveActiveState => false;
        private bool _isInitialized;
        private ButtonRef _updateButton;

        public BoxListModal(UIBase owner) : base(owner)
        {
        }

        public override void SetActive(bool active)
        {
            var shouldUpdateData = _isInitialized && active && this.Enabled == false;
            _isInitialized = true;
            base.SetActive(active);
            if (shouldUpdateData)
                SendCommand();
        }

        public void ClearList()
        {
            _dataList.Clear();
        }

        public void AddList(string name)
        {
            _dataList.Add(new FamBoxData { Name = name});
            _scrollDataHandler.RefreshData();
            _scrollPool.Refresh(true);
        }

        protected override void ConstructPanelContent()
        {
            var horGroup = UIFactory.CreateHorizontalGroup(ContentRoot, "ButtonGroup", true, false, true, false, 3,
                default, new Color(1, 1, 1, 0), TextAnchor.MiddleLeft);
            _updateButton = UIFactory.CreateButton(horGroup, "butPopulate", "Populate List Boxes");
            UIFactory.SetLayoutElement(_updateButton.GameObject, minWidth: 250, minHeight: 25, flexibleWidth: 9999);
            _updateButton.OnClick += SendCommand;

            var l = UIFactory.CreateLabel(ContentRoot, "Header", "", TextAnchor.UpperCenter);
            UIFactory.SetLayoutElement(l.gameObject, minWidth: 250, minHeight: 25, flexibleWidth: 0);

            _scrollDataHandler = new ButtonListHandler<FamBoxData, ButtonCell>(_scrollPool, GetEntries, SetCell, ShouldDisplay, OnCellClicked);
            _scrollPool = UIFactory.CreateScrollPool<ButtonCell>(ContentRoot, "ContentList", out GameObject scrollObj,
                out GameObject scrollContent, new Color(0.03f, 0.03f, 0.03f));
            _scrollPool.Initialize(_scrollDataHandler);
            UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);
        }

        private void SendCommand()
        {
            EnableAllButtons(false);
            MessageService.QueueMessage(NoFog.Config.Settings.BCCOM_LISTBOXES);
            var t = new Timer(3000) { AutoReset = false};
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

        private static ScrollPool<ButtonCell> _scrollPool;
        private static ButtonListHandler<FamBoxData, ButtonCell> _scrollDataHandler;

        private List<FamBoxData> GetEntries() => _dataList;

        private bool ShouldDisplay(FamBoxData data, string filter) => true;

        private void OnCellClicked(int dataIndex)
        {
            var famBox = _dataList[dataIndex];
            UICustomManager.AddBoxContentPanel(famBox.Name);
        }

        private void SetCell(ButtonCell cell, int index)
        {
            if (index < 0 || index >= _dataList.Count)
            {
                cell.Disable();
                return;
            }

            var data = _dataList[index];
            cell.Button.ButtonText.text = data.Name;
        }

        private readonly List<FamBoxData> _dataList = new();

        public class FamBoxData
        {
            public string Name { get; set; }
        }
        #endregion
    }
}

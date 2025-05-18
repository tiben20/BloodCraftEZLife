using System.Collections.Generic;
using BloodCraftUI.Config;
using BloodCraftUI.Services;
using BloodCraftUI.UI.CustomLib.Cells;
using BloodCraftUI.UI.CustomLib.Cells.Handlers;
using BloodCraftUI.UI.CustomLib.Panel;
using BloodCraftUI.UI.ModContent.Data;
using BloodCraftUI.UI.UniverseLib.UI;
using BloodCraftUI.UI.UniverseLib.UI.Models;
using BloodCraftUI.UI.UniverseLib.UI.Panels;
using BloodCraftUI.UI.UniverseLib.UI.Widgets.ScrollView;
using BloodCraftUI.Utils;
using ProjectM;
using UnityEngine;

namespace BloodCraftUI.UI.ModContent
{
    internal class BoxContentPanel : ResizeablePanelBase
    {
        public override string PanelId { get; }
        public override int MinWidth => 340;
        public override int MinHeight => 220;
        public override Vector2 DefaultAnchorMin => new(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 1f);
        public override bool CanDrag => true;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.All;
        public override PanelType PanelType => PanelType.BoxContent;
        public override float Opacity => Settings.UITransparency;

        private readonly string _boxName;
        private bool _isInitialized;

        public BoxContentPanel(UIBase owner, string name) : base(owner)
        {
            PanelId = name;
            SetTitle($"Box '{name}' content");
            _boxName = name;
        }

        protected override void LateConstructUI()
        {
            base.LateConstructUI();
            SendUpdateCommand();
        }

        public override void SetActive(bool active)
        {
            var shouldUpdateData = _isInitialized && active && Enabled == false;
            _isInitialized = true;
            base.SetActive(active);
            if (shouldUpdateData)
                SendUpdateCommand();
        }

        protected override void OnClosePanelClicked()
        {
            SetActive(false);
        }

        #region Commands
        public void SendUpdateCommand()
        {
            if (string.IsNullOrEmpty(_boxName))
                return;

            EnableAllButtons(false);
            MessageService.EnqueueMessage(string.Format(MessageService.BCCOM_SWITCHBOX, _boxName));
            TimerHelper.OneTickTimer(1000, () =>
            {
                try
                {
                    if (Plugin.IS_TESTING)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            AddListEntry(i, $"Test Familiar {i}", AbilitySchoolType.Unholy);
                        }
                    }

                    MessageService.EnqueueMessage(MessageService.BCCOM_BOXCONTENT);
                    MessageService.BoxContentFlag = true;
                }
                finally
                {
                    EnableAllButtons(true);
                }
            });
        }

        private void SendBindCommand(int number)
        {
            EnableAllButtons(false);
            MessageService.EnqueueMessage(MessageService.BCCOM_UNBINDFAM);
            TimerHelper.OneTickTimer(3000, () =>
            {
                try
                {
                    var message = string.Format(MessageService.BCCOM_BINDFAM, number);
                    Settings.LastBindCommand = message;
                    MessageService.EnqueueMessage(message);
                }
                finally
                {
                    EnableAllButtons(true);
                }
            });
        }
        #endregion

        public void AddListEntry(int number, string name, AbilitySchoolType? schoolType)
        {
            _dataList.Add(new FamData { Number = number, Name = name, SpellSchool = schoolType });
            _scrollDataHandler.RefreshData();
            _scrollPool.Refresh(true);
        }

        protected override void ConstructPanelContent()
        {
            _scrollDataHandler = new ButtonListHandler<FamData, ButtonCell>(_scrollPool, GetEntries, SetCell, ShouldDisplay, OnCellClicked);
            _scrollPool = UIFactory.CreateScrollPool<ButtonCell>(ContentRoot, "ContentList", out GameObject scrollObj,
                out _, new Color(0.03f, 0.03f, 0.03f).GetTransparent(Opacity));
            _scrollPool.Initialize(_scrollDataHandler);
            UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);
        }

        internal override void Reset()
        {
            _dataList.Clear();
        }

        private void EnableAllButtons(bool value)
        {
            foreach (var a in _scrollPool.CellPool)
                a.Button.Component.interactable = value;
        }

        #region ScrollPool handling

        private static ScrollPool<ButtonCell> _scrollPool;
        private static ButtonListHandler<FamData, ButtonCell> _scrollDataHandler;

        private List<FamData> GetEntries() => _dataList;

        private bool ShouldDisplay(FamData data, string filter) => true;

        private void OnCellClicked(int dataIndex)
        {
            var fam = _dataList[dataIndex];
            SendBindCommand(fam.Number);
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

        private readonly List<FamData> _dataList = new();

        public class FamData
        {
            public int Number { get; set; }
            public string Name { get; set; }
            public AbilitySchoolType? SpellSchool { get; set; }
        }
        #endregion
    }
}

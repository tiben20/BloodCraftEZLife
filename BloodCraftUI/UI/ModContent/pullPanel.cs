using System.Collections.Generic;
using System.Linq;
using BloodmoonPluginsUI.Services;
using BloodmoonPluginsUI.UI.CustomLib.Cells;
using BloodmoonPluginsUI.UI.CustomLib.Cells.Handlers;
using BloodmoonPluginsUI.UI.CustomLib.Panel;
using BloodmoonPluginsUI.UI.ModContent.Data;
using BloodmoonPluginsUI.UI.UniverseLib.UI;
using BloodmoonPluginsUI.UI.UniverseLib.UI.Models;
using BloodmoonPluginsUI.UI.UniverseLib.UI.Panels;
using BloodmoonPluginsUI.UI.UniverseLib.UI.Widgets.ScrollView;
using UnityEngine;
using UnityEngine.UI;
using static BloodmoonPluginsUI.UI.ModContent.TeleportListPanel;

namespace BloodmoonPluginsUI.UI.ModContent
{
    internal class PullItemsPanel : ResizeablePanelBase
    {
        public override string PanelId => "PullItemsList";
        public override int MinWidth => 340;
        public override int MinHeight => 180;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 1f);
        public override bool CanDrag => true;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.All;
        public override PanelType PanelType => PanelType.SettingsPanel;
        public override float Opacity => 0.9f;
        private LabelRef _valueLabel;
        public class PullItemData
        {
            public string Name { get; set; }
            public string IngameName { get; set; }
            public PullItemData(string name, string ingameName)
            {
                Name = name;
                IngameName = ingameName;
            }
        }

        protected Slider _amountSlider;

        public PullItemsPanel(UIBase owner) : base(owner)
        {
            SetTitle("Pull Items from Chest");
        }

        public void AddItemEntry(PullItemData item)
        {
            if (_items.Any(a => a.Equals(item)))
                return;

            _items.Add(item);
            _scrollDataHandler?.RefreshData();
            _scrollPool?.Refresh(true);
        }

        protected override void LateConstructUI()
        {
            base.LateConstructUI();
            RefreshData();
        }

        protected override void OnClosePanelClicked()
        {
            SetActive(false);
        }

        protected override void ConstructPanelContent()
        {
            // Horizontal slider container
            GameObject sliderContainer = UIFactory.CreateHorizontalGroup(
                ContentRoot,
                "AmountSliderContainer",
                true, // forceExpand
                true, // childForceExpand
                true, // controlChildHeight
                true, // controlChildWidth
                spacing: 8,
                padding: new Vector4(8, 8, 8, 8)
            );

            // Create the slider itself
            UIFactory.CreateSlider(
                sliderContainer,
                "AmountSlider",
                out _amountSlider
            );
            _amountSlider.value = 1;
            _amountSlider.m_MaxValue = 1000;
            _amountSlider.m_MinValue = 1;
            UIFactory.SetLayoutElement(_amountSlider.gameObject, minHeight: 25);

            // Optional: Add label to show value
            _valueLabel = UIFactory.CreateLabel(sliderContainer, "SliderValueLabel", "1");


            _amountSlider.onValueChanged.AddListener((val) =>
            {
                _valueLabel.TextMesh.text = ((int)val).ToString();
            });

            // --- Scroll pool for items ---
            _scrollDataHandler = new ButtonListHandler<PullItemData, ButtonCell>(
                _scrollPool,
                GetItemEntries,
                SetCell,
                ShouldDisplay,
                OnCellClicked
            );

            _scrollPool = UIFactory.CreateScrollPool<ButtonCell>(
                ContentRoot,
                "ContentList",
                out GameObject scrollObj,
                out _,
                new Color(0.03f, 0.03f, 0.03f, Opacity)
            );
            _scrollPool.Initialize(_scrollDataHandler);
            UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);

            PopulateAllPullableItems(); // <-- Fill the panel with all items

            RefreshData();
        }

        internal override void Reset()
        {
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
            if (shouldUpdateData)
                RefreshData();
        }

        #region ScrollPool handling

        private ScrollPool<ButtonCell> _scrollPool;
        private ButtonListHandler<PullItemData, ButtonCell> _scrollDataHandler;
        private bool _isInitialized;

        private readonly List<PullItemData> _items = new List<PullItemData>();

        private List<PullItemData> GetItemEntries() => _items;

        private void OnCellClicked(int index)
        {
            if (index < 0 || index >= _items.Count)
                return;

            string itemName = _items[index].IngameName;
            MessageService.EnqueueMessage(".pull \"" + itemName+"\" "+ _valueLabel.TextMesh.text);
            //Plugin.Console.SendCommand($".pull {itemName}");
        }

        private void SetCell(ButtonCell cell, int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                cell.Disable();
                return;
            }
            cell.Button.ButtonText.text = _items[index].Name;
            
        }

        #endregion

        private void PopulateAllPullableItems()
        {
            // Example list of all pullable items; replace with your actual items
            var allItems = new List<PullItemData>
            {
                new PullItemData("Iron Ore","Iron Ore"),
                new PullItemData("Copper Ore","Copper Ore"),
                new PullItemData("Wood", "Wood"),
                new PullItemData("Stone","Stone"),
                new PullItemData("Blood Essence","Blood Essence")
                // Add every pullable item here
            };

            foreach (var item in allItems)
                AddItemEntry(item);
        }

        private bool ShouldDisplay(PullItemData data, string filter) => true;

        private class ItemEntry
        {
            public string Name;
        }
    }
}

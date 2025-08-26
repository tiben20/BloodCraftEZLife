using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using BloodCraftEZLife.Config;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.Services.Data;
using BloodCraftEZLife.UI.CustomLib.Cells;
using BloodCraftEZLife.UI.CustomLib.Cells.Handlers;
using BloodCraftEZLife.UI.CustomLib.Panel;
using BloodCraftEZLife.UI.ModContent.Data;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.UI.UniverseLib.UI.Panels;
using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;
using ProjectM;
using Stunlock.Core;
using UnityEngine;
using UnityEngine.UI;
using static BloodCraftEZLife.UI.ModContent.TeleportListPanel;

namespace BloodCraftEZLife.UI.ModContent
{
    internal class PullItemsPanel : ResizeablePanelBase
    {
        private Dictionary<string, ItemCategory> _itemCategorys = new Dictionary<string, ItemCategory>();
        //public override string  => "PullItemsList";
        public override int MinWidth => 340;
        public override int MinHeight => 180;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPosition => new Vector2(0.5f, 1f);
        public override bool CanDrag { get; protected set; } = true;
        // Allow vertical resizing only
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.All;
        


        public override string PanelId => "PullItemsList";

        public override PanelType PanelType => PanelType.PullPanel;
        private TMPro.TMP_Dropdown _dropdown;
        private InputFieldRef _valueLabel;
        public class PullItemData
        {
            public string Name { get; set; }
            public PrefabGUID PrefabGUID { get; set; }
            public Sprite Icon { get; set; }
            public PullItemData(string name, PrefabGUID ingameGuid,Sprite icon)
            {
                Name = name;
                PrefabGUID = ingameGuid;
                Icon = icon;
            }
        }

        public Slider _amountSlider;

        public PullItemsPanel(UIBase owner) : base(owner)
        {
            SetTitle("Pull items from chest");
            
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
                spacing: 2,
                padding: new Vector4(2, 2, 2, 2)
            );

            // Create the slider itself
            UIFactory.CreateSlider(
                sliderContainer,
                "AmountSlider",
                out _amountSlider
            );
            _amountSlider.value = 1;
            _amountSlider.m_MaxValue = 10000;
            _amountSlider.m_MinValue = 0;
            UIFactory.SetLayoutElement(_amountSlider.gameObject, minHeight: 40,minWidth:180);

            // Optional: Add label to show value
            _valueLabel = UIFactory.CreateInputField(sliderContainer, "SliderValueLabel", "");
            
            _valueLabel.Component.text = ((int)1).ToString();
            _amountSlider.onValueChanged.AddListener((val) =>
            {
                _valueLabel.Component.text = ((int)val).ToString();
            });
            

            _itemCategorys.Add("ALL", ItemCategory.ALL);
            _itemCategorys.Add("Alchemy", ItemCategory.Alchemy);
            _itemCategorys.Add("Materials", ItemCategory.Mineral);
            _itemCategorys.Add("Tailoring", ItemCategory.Tailoring);
            _itemCategorys.Add("Herb", ItemCategory.Herb);
            _itemCategorys.Add("Fish", ItemCategory.Fish);
            _itemCategorys.Add("Gems", ItemCategory.Gem);
            
            


            var dropdownobj = UIFactory.CreateDropdown(ContentRoot,"DropdownCategory",out _dropdown, "All",18, OnDropdownChanged);
            foreach (var cat in _itemCategorys) 
            {
                _dropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(cat.Key));
            }
            UIFactory.SetLayoutElement(dropdownobj, minHeight: 40);


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
                new Color(0.03f, 0.03f, 0.03f, Settings.UITransparency)
            );
            _scrollPool.Initialize(_scrollDataHandler);
            UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);


            PopulateAllPullableItems(ItemCategory.ALL);
            RefreshData();
        }

        private void OnDropdownChanged(int index)
        {
            PopulateAllPullableItems(_itemCategorys[_dropdown.options[index].text]);
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

        private List<PullItemData> GetItemEntries()
        {
            return _items;
        }
        

        private void OnCellClicked(int index)
        {
            if (index < 0 || index >= _items.Count)
                return;

            string itemName = _items[index].Name;
            var panel = Plugin.UIManager.GetPanel<PopupPanel>();
            if (panel != null)
            {
                string themessage = $"Pulling {_valueLabel.Text} {_items[index].Name}";
                panel.ShowMessage(themessage, 3f,PopupPanel.MessageType.Small);
            }
            
            MessageService.EnqueueMessage(".pull \"" + _items[index].Name + "\" "+ _valueLabel.Text);
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
            cell.IconImage.sprite = _items[index].Icon;


        }

        #endregion

        private void PopulateAllPullableItems(ItemCategory cat)
        {
            _items.Clear();
            
            foreach (var itm in Items.Instance.Stackables(cat))
            {
                string newitm = itm.Name +" "+itm.ItemCategory.ToString();
                _items.Add(new PullItemData(newitm, itm.PrefabGuid,itm.Icon));
                
            }
            
        }

        private bool ShouldDisplay(PullItemData data, string filter) => true;

        public void SetActiveOnly(bool active)
        {
            throw new System.NotImplementedException();
        }

        private class ItemEntry
        {
            public string Name;
        }
    }
}

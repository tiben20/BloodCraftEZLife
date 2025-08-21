using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BloodCraftEZLife.Config;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.UI.CustomLib;
using BloodCraftEZLife.UI.CustomLib.Cells;
using BloodCraftEZLife.UI.CustomLib.Cells.Handlers;
using BloodCraftEZLife.UI.CustomLib.Panel;
using BloodCraftEZLife.UI.CustomLib.Util;
using BloodCraftEZLife.UI.ModContent.Data;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.UI.UniverseLib.UI.Panels;
using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;
using BloodCraftEZLife.Utils;
using ProjectM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloodCraftEZLife.UI.ModContent
{
    internal class HotkeysPanel : PanelBase
    {
        public override string PanelId => "HotkeysList";
        public override int MinWidth => (int)480;
        public override int MinHeight => (int)240;
        public override int MaxWidth => (int)3840;
        
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 0.5f);

        public override bool CanDrag => false;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.None;
        public override PanelType PanelType => PanelType.SettingsPanel;

        public GameObject PanelHotkeyContent { get; protected set; }

        private ScrollPool<HotkeyCell> _scrollPool;
        private ConfigCellHandler<Hotkey, HotkeyCell> _scrollDataHandler;
        private bool _isInitialized;
        private int _indexWaitingFor = 0;

        public HotkeysPanel(UIBase owner) : base(owner)
        {
            SetTitle("Settings");
        }


        protected override void LateConstructUI()
        {
            base.LateConstructUI();

            float xfact = Owner.Scaler.referenceResolution.x / 1920;
            float yfact = Owner.Scaler.referenceResolution.y / 1080;


            Vector2 newRect = new Vector2(FullscreenSettingService.DeltaRect.x * xfact, FullscreenSettingService.DeltaRect.y * yfact);
            Rect.sizeDelta = newRect;
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
            PanelHotkeyContent = UIFactory.CreateVerticalGroup(ContentRoot, "HotkeyVerticalLayout", false, true, true, true, 2,
            new Vector4(2, 2, 2, 2), Theme.PanelBackground);
            UIFactory.SetLayoutElement(PanelHotkeyContent, minHeight: 25, flexibleHeight: 0);
            _scrollDataHandler = new ConfigCellHandler<Hotkey, HotkeyCell>(_scrollPool, HotkeyService.GetHotkeys, SetCell, ShouldDisplay, OnCellChanged);
            _scrollPool = UIFactory.CreateScrollPool<HotkeyCell>(PanelHotkeyContent, "ContentList", out GameObject scrollObj,
                out _, new Color32(10,10,10,255));
            _scrollPool.Initialize(_scrollDataHandler);
            UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);

            var saveHkButton = UIFactory.CreateButton(PanelHotkeyContent, "SaveHkButton", "Save and close", new ColorBlock
            {
                normalColor = new Color(0.11f, 0.11f, 0.11f).GetTransparent(Settings.UITransparency),
                disabledColor = new Color(0.3f, 0.3f, 0.3f).GetTransparent(Settings.UITransparency),
                highlightedColor = new Color(0.16f, 0.16f, 0.16f).GetTransparent(Settings.UITransparency),
                pressedColor = new Color(0.05f, 0.05f, 0.05f).GetTransparent(Settings.UITransparency)
            });
            UIFactory.SetLayoutElement(saveHkButton.Component.gameObject, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);

            //creating the group horizontal group
            var newhotkeyrow = UIFactory.CreateHorizontalGroup(PanelHotkeyContent, "NewButtonRow", false, false, true, true, spacing: 10, bgColor: new Color32(18, 18, 18, 255));
            UIFactory.SetLayoutElement(UIRoot, minHeight: 40, flexibleWidth: 9999);


            RectTransform newRect = newhotkeyrow.GetComponent<RectTransform>();
            newRect.anchorMin = new Vector2(0, 1);
            newRect.anchorMax = new Vector2(0, 1);
            newRect.pivot = new Vector2(0.5f, 1);
            newRect.sizeDelta = new Vector2(25, 25);


            var newhklabel = UIFactory.CreateLabel(newhotkeyrow, "LabelTitle", "setting.Name",
                                              TextAlignmentOptions.MidlineLeft, fontSize: 16, bold: false);// medium-light gray
            UIFactory.SetLayoutElement(newhklabel.GameObject, minHeight: 40, minWidth: 120, flexibleWidth: 9999);


            var newHkButton = UIFactory.CreateButton(newhotkeyrow, "NewHkButton", "New Hotkey", new ColorBlock
            {
                normalColor = new Color(0.11f, 0.11f, 0.11f).GetTransparent(Settings.UITransparency),
                disabledColor = new Color(0.3f, 0.3f, 0.3f).GetTransparent(Settings.UITransparency),
                highlightedColor = new Color(0.16f, 0.16f, 0.16f).GetTransparent(Settings.UITransparency),
                pressedColor = new Color(0.05f, 0.05f, 0.05f).GetTransparent(Settings.UITransparency)
            });
            UIFactory.SetLayoutElement(newHkButton.Component.gameObject, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);
            var buttonText = newHkButton.Component.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.overflowMode = TextOverflowModes.Overflow;
            buttonText.alignment = TextAlignmentOptions.MidlineLeft;
            buttonText.margin = new Vector4(5, 0, 0, 0);

            newHkButton.OnClick += () =>
            {
                HotkeyService.Register(KeyCode.None, "waiting input");
                HotkeyService.WaitingForInput = true;
                //index not used yet
                _indexWaitingFor = HotkeyService.HotkeyCount() - 1;
                RefreshData();
            };
            saveHkButton.OnClick += () =>
            {
                HotkeyService.SaveHotkeys();
                this.SetActive(false);
            };

            RefreshData();
        }

        public override void Update()
        {


            //send an update on the hotkeys
            
            if (HotkeyService.WaitedForKey != KeyCode.None)
            {
                HotkeyService.SetKey(HotkeyService.GetHotkey(_indexWaitingFor), HotkeyService.WaitedForKey);
                HotkeyService.WaitedForKey = KeyCode.None;
                RefreshData();
            }
            base.Update();
            // Call update on the panels that need it
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

        private void OnCellChanged(Hotkey newValue)
        {
            HotkeyService.WaitingForInput = true;
            _indexWaitingFor = HotkeyService.GetHotkeyIndex(newValue.key);
            //buttonText.text = "Press a key";
            //HotkeyService.SetKey(newValue);
            return;
        }

        private bool ShouldDisplay(Hotkey data, string filter) => true;

        private void SetCell(HotkeyCell cell, int index)
        {
            if (index < 0 || index >= HotkeyService.HotkeyCount())
            {
                cell.Disable();
                return;
            }

            cell.SetValue(HotkeyService.GetHotkey(index));


        }

        #endregion
    }
}
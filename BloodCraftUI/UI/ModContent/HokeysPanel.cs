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
using ProjectM.UI;
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
        private HotkeyCellHandler<Hotkey, HotkeyCell> _scrollDataHandler;
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


            Vector2 newRect = new Vector2(FullscreenSettingService.DeltaRect.x * xfact, FullscreenSettingService.DeltaRect.y * yfact );
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
            PanelHotkeyContent = UIFactory.CreateVerticalGroup(ContentRoot, "HotkeyVerticalLayout", true, true, true, true, 2,
            new Vector4(2, 2, 2, 2), Theme.PanelBackground);


            GameObject lblHeader = UnityEngine.Object.Instantiate(FullscreenSettingService._templates.Header.gameObject, PanelHotkeyContent.transform);
            SettingsEntry_Label lbl = lblHeader.GetComponent<SettingsEntry_Label>();

            if (lbl != null)
            {
                lbl.HeaderText.Text.text = "Hotkeys";
            }
            UIFactory.SetLayoutElement(PanelHotkeyContent, minHeight: 25, flexibleHeight: 0);
            _scrollDataHandler = new HotkeyCellHandler<Hotkey, HotkeyCell>(_scrollPool, HotkeyService.GetHotkeys, SetCell, ShouldDisplay, OnCellChanged,OnInputBox);
            _scrollPool = UIFactory.CreateScrollPool<HotkeyCell>(PanelHotkeyContent, "ContentList", out GameObject scrollObj,
                out _, Theme.PanelBackground);
            _scrollPool.Initialize(_scrollDataHandler);
            UIFactory.SetLayoutElement(scrollObj, flexibleHeight: 9999);

            GameObject newButtonObj = UnityEngine.Object.Instantiate(FullscreenSettingService._templates.Button.gameObject, PanelHotkeyContent.transform);
            SettingsEntry_Button newButton = newButtonObj.GetComponent<SettingsEntry_Button>();
            newButton.ButtonText.Text.text = "New hotkey";
            UnityHelper.HideObject(newButton.ResetButton.gameObject);
            newButton.HeaderText.Text.text = "";
            newButton.SecondaryText.Text.text = "";
            GameObject saveButtonObj = UnityEngine.Object.Instantiate(FullscreenSettingService._templates.Button.gameObject, PanelHotkeyContent.transform);
            SettingsEntry_Button saveButton = saveButtonObj.GetComponent<SettingsEntry_Button>();
            saveButton.ButtonText.Text.text = "Save and close";
            UnityHelper.HideObject(saveButton.ResetButton.gameObject);
            saveButton.HeaderText.Text.text = "";
            saveButton.SecondaryText.Text.text = "";


            newButton.Button.onClick.AddListener(() =>
            {
                HotkeyService.NewKey = true;
                HotkeyService.Register(KeyCode.None, "press a key");
                HotkeyService.WaitingForInput = true;
                //index not used yet
                _indexWaitingFor = HotkeyService.HotkeyCount() - 1;
                RefreshData();
            });

            saveButton.Button.onClick.AddListener(() =>
            {
                HotkeyService.SaveHotkeys();
                this.SetActive(false);
            });
            
            RefreshData();
        }

        public override void Update()
        {
            //send an update on the hotkeys
            
            if (HotkeyService.WaitedForKey != KeyCode.None)
            {
                HotkeyService.SetKey(HotkeyService.GetHotkey(_indexWaitingFor), HotkeyService.WaitedForKey);
                HotkeyService.WaitedForKey = KeyCode.None;
                if (HotkeyService.NewKey)
                {
                    OnInputBox(HotkeyService.GetHotkey(_indexWaitingFor));
                }
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

        private void OnInputBox(Hotkey newValue)
        {
            var panel = Plugin.UIManager.GetPanel<CommandInput>();
            panel._onCommand = OnInputBoxSubmit;
            
            panel.SetActive(true);
            panel.Show(newValue);

        }

        public void OnInputBoxSubmit(Hotkey newValue)
        {
            HotkeyService.Register(newValue.key, newValue.action);
            Plugin.UIManager.GetPanel<CommandInput>().SetActive(false);

            RefreshData();
        }

        private void OnCellChanged(Hotkey newValue)
        {
            HotkeyService.WaitingForInput = true;
            _indexWaitingFor = HotkeyService.GetHotkeyIndex(newValue.key);
            //buttonText.text = "Press a key";
            //HotkeyService.SetKey(newValue);
            return;
        }

        private bool ShouldDisplay(Hotkey data, string filter)
        {
            return true;
        
        }

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
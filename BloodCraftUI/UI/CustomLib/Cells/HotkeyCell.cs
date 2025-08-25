using System;
using System.ComponentModel;
using BloodCraftEZLife.Config;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.UI.CustomLib.Util;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.Utils;
using ProjectM.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.UI.GridLayoutGroup;

namespace BloodCraftEZLife.UI.CustomLib.Cells
{
    public class HotkeyCell : CellHotkeyBase, IConfigurableCell<Hotkey>
    {
        private InputFieldRef _inputField;
        
        private GameObject ConfigboxContainer;

        public override float DefaultHeight => 25f;
        private Hotkey _hotkey;
        
        public Action<Hotkey> OnInputBox { get; set; }
        public Action<Hotkey> OnDelete { get; set; }

        event Action<Hotkey> IConfigurableCell<Hotkey>.OnInputBox
        {
            add => OnInputBox += value;
            remove => OnInputBox -= value;
        }

        public Action<Hotkey> OnValueChanged { get; set; }

        event Action<Hotkey> IConfigurableCell<Hotkey>.OnValueChanged
        {
            add => OnValueChanged += value;
            remove => OnValueChanged -= value;
        }

        event Action<Hotkey> IConfigurableCell<Hotkey>.OnDelete
        {
            add => OnDelete += value;
            remove => OnDelete -= value;
        }

        private SimpleStunButton ActionButton;
        private SimpleStunButton DeleteButton;
        private SettingsEntry_Button ButtonClone;

        private static void PrintRect(string rectname, RectTransform rect)
        {
            LogUtils.LogInfo(rectname + " LocalPosition x:" + rect.localPosition.x.ToString() + " y:" + rect.localPosition.y.ToString());
            LogUtils.LogInfo(rectname + " sizeDelta width:" + rect.sizeDelta.x.ToString() + " height:" + rect.sizeDelta.y.ToString());
            
        }

        public override GameObject CreateContent(GameObject parent)
        {
            //creating the group horizontal group
            ConfigboxContainer = UIFactory.CreateHorizontalGroup(UIRoot, "TextScalingRow", true, true, true, true, bgColor: Theme.PanelBackground);
            UIFactory.SetLayoutElement(ConfigboxContainer, minHeight: 40, flexibleWidth: 9999);


            Rect = ConfigboxContainer.GetComponent<RectTransform>();
            Rect.anchorMin = new Vector2(0, 1);
            Rect.anchorMax = new Vector2(0, 1);
            Rect.pivot = new Vector2(0.5f, 1);
            Rect.sizeDelta = new Vector2(25, 25);
            GameObject lblHeader = UnityEngine.Object.Instantiate(FullscreenSettingService._templates.Button.gameObject, ConfigboxContainer.transform);
            GameObject btn2 = UnityEngine.Object.Instantiate(FullscreenSettingService._templates.Button.Button.gameObject, lblHeader.transform);
            GameObject btn3 = UnityEngine.Object.Instantiate(FullscreenSettingService._templates.Button.Button.gameObject, lblHeader.transform);
            ButtonClone = lblHeader.GetComponent<SettingsEntry_Button>();
            //hide reset button
            UnityHelper.HideObject(ButtonClone.ResetButton.gameObject);
            //action button
            ActionButton = btn2.GetComponent<SimpleStunButton>();
            DeleteButton = btn3.GetComponent<SimpleStunButton>();
            RectTransform keyRect = ButtonClone.Button.GetComponent<RectTransform>();
            RectTransform actionRect = ActionButton.GetComponent<RectTransform>();
            RectTransform deleteRect = DeleteButton.GetComponent<RectTransform>();
            float xfact = FullscreenSettingService.ResFactor.x;
            float offsetDelete = (FullscreenSettingService.DeltaRect.x * FullscreenSettingService.ResFactor.x * 0.2f) - 20f;

            float offset = (FullscreenSettingService.DeltaRect.x * FullscreenSettingService.ResFactor.x * 0.8f) - 20f;
            
            keyRect.localPosition = keyRect.localPosition - new Vector3(offset, 0, 0);
            
            deleteRect.sizeDelta = new Vector2((deleteRect.localPosition.x - keyRect.localPosition.x - 20f)/4, deleteRect.sizeDelta.y);
            actionRect.sizeDelta = new Vector2((deleteRect.localPosition.x - keyRect.localPosition.x - 20f) / 1.3333333333f, deleteRect.sizeDelta.y);
            //actionRect.localPosition = deleteRect.localPosition - new Vector3((actionRect.sizeDelta.x - 20f)/2 , 0, 0);
            actionRect.localPosition = deleteRect.localPosition - new Vector3(deleteRect.sizeDelta.x, 0, 0);
            //PrintRect("key", keyRect);
            //PrintRect("action", actionRect);
            //PrintRect("delete", deleteRect);
            SettingsEntryBase lbl = lblHeader.GetComponent<SettingsEntryBase>();
            ButtonClone.HeaderText.Text.text = "";
            ButtonClone.SecondaryText.Text.text = "";
            DeleteButton.SetText("Delete hotkey");
            DeleteButton.onClick.AddListener(() =>
            {
                OnDelete?.Invoke(_hotkey);
            });

            ActionButton.onClick.AddListener(() =>
            {
                OnInputBox?.Invoke(_hotkey);
            });

            //This is the key field listenener
            ButtonClone.Button.onClick.AddListener(() =>
            {
                ButtonClone.Button.SetText("Press key");
                
                HotkeyService.ChangingOldKey = _hotkey.key;
                OnValueChanged?.Invoke(_hotkey);

            });
            return ConfigboxContainer;

        }

        public void SetValue(Hotkey value) 
        {
            _hotkey = value;
            ButtonClone.Button.SetText(value.key.ToString());
            ActionButton.SetText(value.action);
            if (value.action == "Press key")
            {
                ButtonClone.Button.SetText("Press key");
            }

            ConfigboxContainer?.SetActive(true);
        }
        

        public Hotkey GetValue()
        { 
            return _hotkey;
        }


        private void OnInputSubmit(string val)
        {
            _hotkey.action = val;
            HotkeyService.Register(_hotkey.key, _hotkey.action);
        }
        private void OnInputSelect(string val)
        {
            _inputField.Component.caretPosition = _inputField.Component.text.Length;
            _inputField.Component.m_CaretSelectPosition = _inputField.Component.text.Length;
            
        }
        

        private void OnInputChanged(string val)
        {
            _hotkey.action = val;
        }

        public override void Disable()
        {
            ConfigboxContainer?.SetActive(false);
        }

        
    }

}
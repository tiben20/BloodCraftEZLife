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
        private SimpleStunButton SecondButton;
        private SettingsEntry_Button ButtonClone;

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
            ButtonClone = lblHeader.GetComponent<SettingsEntry_Button>();
            //hide reset button
            UnityHelper.HideObject(ButtonClone.ResetButton.gameObject);

            SecondButton = btn2.GetComponent<SimpleStunButton>();
            RectTransform primaryRect = ButtonClone.Button.GetComponent<RectTransform>();
            RectTransform secondaryRect = SecondButton.GetComponent<RectTransform>();
            float xfact = FullscreenSettingService.ResFactor.x;

            float offset = (FullscreenSettingService.DeltaRect.x * FullscreenSettingService.ResFactor.x * 0.7f) - 20f;

            primaryRect.localPosition = primaryRect.localPosition - new Vector3(offset, 0, 0);
            secondaryRect.sizeDelta = new Vector2(secondaryRect.localPosition.x - primaryRect.localPosition.x - 20f, secondaryRect.sizeDelta.y);
            SettingsEntryBase lbl = lblHeader.GetComponent<SettingsEntryBase>();
            ButtonClone.HeaderText.Text.text = "";
            ButtonClone.SecondaryText.Text.text = "";
            
            GameObject inputGO = new GameObject("HiddenInputField");

            TMP_InputField inputField = inputGO.AddComponent<TMP_InputField>();
            Navigation nav = inputField.navigation;
            nav.mode = Navigation.Mode.None;
            inputField.navigation = nav;

            // Add a Text component for the InputField to work, but keep it hidden
            TextMeshProUGUI inputText = inputGO.AddComponent<TextMeshProUGUI>();

            inputText.enabled = true; // completely invisible
            inputField.textComponent = inputText;

            inputField.characterLimit = 100; // or whatever limit
            inputField.interactable = true;
            inputField.selectionColor = Color.red;
            SecondButton.onClick.AddListener(() =>
            {
                OnInputBox?.Invoke(_hotkey);
                //inputField.text = SecondButton.GetText();
                //inputField.ActivateInputField();
                //inputField.m_CaretPosition = inputField.text.Length;
                //inputField.m_CaretSelectPosition = inputField.text.Length;
            });
      

            // Hook up InputField end edit to update Button Text
            inputField.onEndEdit.AddListener((string s) =>
            {
               
                SecondButton.SetText(s);
                OnInputSubmit(s);

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
            SecondButton.SetText(value.action);
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
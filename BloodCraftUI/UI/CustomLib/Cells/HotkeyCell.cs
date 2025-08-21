using System;
using BloodCraftEZLife.Config;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

namespace BloodCraftEZLife.UI.CustomLib.Cells
{
    public class HotkeyCell : CellBase, IConfigurableCell<Hotkey>
    {
        private InputFieldRef _inputField;
        public ButtonRef Button { get; set; }
        public string Label
        {
            get => _inputField.Text;
            set => _inputField.Text = value;
        }

        public override float DefaultHeight => 25f;
        private Hotkey _hotkey;
        public Action OnClick { get; set; }

        public override GameObject CreateContent(GameObject parent)
        {
            //creating the group horizontal group
            UIRoot = UIFactory.CreateHorizontalGroup(parent, "HotkeyRow", false, false, true, true, spacing: 10, bgColor: new Color32(18, 18, 18, 255));
            UIFactory.SetLayoutElement(UIRoot, minHeight: 40, flexibleWidth: 9999);


            Rect = UIRoot.GetComponent<RectTransform>();
            Rect.anchorMin = new Vector2(0, 1);
            Rect.anchorMax = new Vector2(0, 1);
            Rect.pivot = new Vector2(0.5f, 1);
            Rect.sizeDelta = new Vector2(25, 25);


            _inputField = UIFactory.CreateInputField(UIRoot, "InputField", "..");// medium-light gray
            //var rowBtn = _inputField.GameObject.AddComponent<Button>();
            /*var colors = rowBtn.colors;
            colors.normalColor = new Color(0, 0, 0, 0);
            colors.highlightedColor = new Color(0.1f, 0.1f, 0.1f, 0.5f); // hover highlight*/

            UIFactory.SetLayoutElement(_inputField.GameObject, minHeight: 40, minWidth: 120, flexibleWidth: 9999);
            _inputField.OnValueChanged += OnInputChanged;
            _inputField.Component.onSubmit.AddListener(OnInputSubmit);
            _inputField.Component.onSelect.AddListener(OnInputSelect);
            UIRoot.SetActive(false);

            Button = UIFactory.CreateButton(UIRoot, "NameButton", "Name", new ColorBlock
            {
                normalColor = new Color(0.11f, 0.11f, 0.11f).GetTransparent(Settings.UITransparency),
                disabledColor = new Color(0.3f, 0.3f, 0.3f).GetTransparent(Settings.UITransparency),
                highlightedColor = new Color(0.16f, 0.16f, 0.16f).GetTransparent(Settings.UITransparency),
                pressedColor = new Color(0.05f, 0.05f, 0.05f).GetTransparent(Settings.UITransparency)
            });
            UIFactory.SetLayoutElement(Button.Component.gameObject, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);
            var buttonText = Button.Component.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.overflowMode = TextOverflowModes.Overflow;
            buttonText.alignment = TextAlignmentOptions.MidlineLeft;
            buttonText.margin = new Vector4(5, 0, 0, 0);

            Button.OnClick += () => { OnValueChanged?.Invoke(_hotkey); };
            
            //rowBtn.onClick.AddListener(new Action(() => { OnActionInput?.Invoke(_hotkey); }));

            /*Button.OnClick += () => 
            {
                HotkeyService.WaitingForInput = true;
                buttonText.text = "Press a key";
            };*/
            return UIRoot;
        }

        public void SetValue(Hotkey value) 
        {
            _hotkey = value;
            Label = value.action;
            Button.ButtonText.text = value.key.ToString();
            
        }
        

        public Hotkey GetValue()
        { 
            return _hotkey;
        }

        public void SetLabel(string label)
        {
            throw new NotImplementedException();
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

        public Action<Hotkey> OnValueChanged { get; set; }

        event Action<Hotkey> IConfigurableCell<Hotkey>.OnValueChanged
        {
            add => OnValueChanged += value;
            remove => OnValueChanged -= value;
        }
    }

}
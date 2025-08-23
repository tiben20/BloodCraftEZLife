using System;
using BloodCraftEZLife.Config;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.UI.CustomLib.Util;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;
using BloodCraftEZLife.Utils;
using Il2CppSystem.Data;
using ProjectM.UI;
using Stunlock.Localization;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities.DebugProxies;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using static BloodCraftEZLife.Config.Settings;
using static Il2CppSystem.Linq.Expressions.Interpreter.NullableMethodCallInstruction;
using static UnityEngine.Rendering.ProbeReferenceVolume;


namespace BloodCraftEZLife.UI.CustomLib.Cells
{
    public interface IFormedCheckbox : ICell
    {
        public int CurrentDataIndex { get; set; }
        public Action<int> OnValueChanged { get; set; }
    }

    public class ConfigboxCell : CellBase, IConfigurableCell<Setting>
    {
        private LabelRef _label;
        private GameObject ConfigboxContainer;
        public string Label
        {
            get => _label.TextMesh.text;
            set => _label.TextMesh.text = value;
        }

        
        public override float DefaultHeight => 25f;
        private Setting _setting;
        private bool _subCreated;

        public event Action<Setting> OnActionInput;

        public override GameObject CreateContent(GameObject parent)
        {
            //creating the group horizontal group
            /*ConfigboxContainer = UIFactory.CreateHorizontalGroup(UIRoot, "TextScalingRow", false, false, true, true, padding:new Vector4(0,0,50,70),
                                                    bgColor: new Color32(18, 18, 18, 255),childAlignment:TextAnchor.MiddleLeft);
            UIFactory.SetLayoutElement(ConfigboxContainer, minHeight: 40, flexibleWidth: 9999);*/

            ConfigboxContainer = UIFactory.CreateHorizontalGroup(UIRoot, "TextScalingRow", true, true, true, true, bgColor: Theme.PanelBackground);
            Rect = ConfigboxContainer.GetComponent<RectTransform>();
            Rect.anchorMin = new Vector2(0, 1);
            Rect.anchorMax = new Vector2(0, 1);
            Rect.pivot = new Vector2(0.5f, 1);
            Rect.sizeDelta = new Vector2(25, 25);
            /*int labelwidth = (int)FullscreenSettingService.DeltaRect.x / 2;

            _label = UIFactory.CreateLabel(ConfigboxContainer, "LabelTitle", "",
                                              TextAlignmentOptions.MidlineLeft, fontSize: 16, bold: false);// medium-light gray
            UIFactory.SetLayoutElement(_label.GameObject, minHeight: 40, minWidth: labelwidth, flexibleWidth: 9999);
            */
            
            _subCreated = false;
            return ConfigboxContainer;
        }
        public void CreateHeader(Setting setting)
        {
            GameObject lblHeader = UnityEngine.Object.Instantiate(FullscreenSettingService._templates.Header.gameObject, ConfigboxContainer.transform);
            SettingsEntry_Label lbl = lblHeader.GetComponent<SettingsEntry_Label>();

            if (lbl != null)
            {
                lbl.HeaderText.Text.m_text = (string)setting.Value;
            }
        }
        private void SetLabelName(GameObject obj, string lbltext)
        {
            var label = obj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (label != null)
                label.text = (string)_setting.Name;
        }

        public Il2CppSystem.Action<bool> OnChangeToggle;

        public void CreateToggle(Setting setting)
        {
            GameObject toggle = UnityEngine.Object.Instantiate(FullscreenSettingService._templates.Toggle.gameObject, ConfigboxContainer.transform);
            SettingsEntry_Checkbox toggleObject = toggle.GetComponent<SettingsEntry_Checkbox>();

            if (toggleObject != null)
            {
                SetLabelName(toggleObject.gameObject, _setting.Name);
                
                toggleObject.Value = (bool)setting.Value;
                UnityHelper.PrintChilds(toggleObject.transform, 1);

                toggleObject.Toggle.onValueChanged.AddListener((bool val) =>
                {
                    setting.Value = val;
                    OnValueChanged?.Invoke(setting);
                });
                toggleObject.Button.m_OnClick.AddListener(() =>
                {
                    toggleObject.Value = !toggleObject.Value;
                });
            }
            
            //Debug.Log("Child of our toggle ");
            //UnityHelper.PrintChilds(toggle.transform,1,true);
        }

        public void CreateDropdown(Setting setting)
        {
            GameObject dropdown = UnityEngine.Object.Instantiate(FullscreenSettingService._templates.ComboBox.gameObject, ConfigboxContainer.transform);
            SettingsEntry_Dropdown dropdownObject = dropdown.GetComponent<SettingsEntry_Dropdown>();
            if (dropdownObject != null)
            {
                SetLabelName(dropdownObject.gameObject, _setting.Name);
                foreach (var opt in setting.Options)
                {
                    dropdownObject.Dropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(opt));
                }
                dropdownObject.Dropdown.onValueChanged.AddListener((int val) =>
                {
                    setting.Value = val;
                    OnValueChanged?.Invoke(setting);
                });
            }
        }

        public void CreateSlider(Setting setting)
        {
            GameObject slider = UnityEngine.Object.Instantiate(FullscreenSettingService._templates.Slider.gameObject, ConfigboxContainer.transform);
            SettingsEntry_Slider sliderObject = slider.GetComponent<SettingsEntry_Slider>();
            Debug.Log("Child of our sliderObject ");
            UnityHelper.PrintChilds(slider.transform, 1, true);
            if (sliderObject != null)
            {
                SetLabelName(sliderObject.gameObject, _setting.Name);
                sliderObject.Slider.value = (float)setting.Value;
                sliderObject.ValueField.text = setting.Value.ToString();
                sliderObject.Slider.minValue = (float)setting.Min;
                sliderObject.Slider.maxValue = (float)setting.Max;
                sliderObject.Slider.StepSize = (float)setting.Step;
                sliderObject.Slider.onValueChanged.AddListener((val) =>
                {
                    setting.Value = val;
                    sliderObject.ValueField.text = val.ToString();
                    OnValueChanged?.Invoke(setting);
                });
            }
            
        }

        public void InitialiseSetting(Setting set)
        {
            _setting = set;
            if (!_subCreated)
            {
                if (set.Type == Setting.SettingType.Bool)
                {
                    CreateToggle(_setting);
                }
                else if (set.Type == Setting.SettingType.Float)
                {
                    CreateSlider(_setting);
                }
                else if (set.Type == Setting.SettingType.Header)
                {
                    CreateHeader(_setting);
                }
                else if (set.Type == Setting.SettingType.Dropdown)
                {
                    CreateDropdown(_setting);
                }
                
                _subCreated = true;
            }
            
        }


        public Action<Setting> OnValueChanged { get; set; }

        public void SetValue(Setting value)
        {
            _setting = value;

        }

        public Setting GetValue() => _setting;
        public void SetLabel(string label)
        {
            switch (_setting.Type)
            {
                case Setting.SettingType.Bool:
                    //_label.TextMesh.text = label;
                    break;
                case Setting.SettingType.String:
                    _label.TextMesh.text = label;
                    break;
                case Setting.SettingType.Float:
                    _label.TextMesh.text = label;
                    break;
                case Setting.SettingType.Dropdown:
                    _label.TextMesh.text = label;
                    break;
                default:
                    break;
            }
            
            _setting.Name = label;
            
        }

        event System.Action<Setting> IConfigurableCell<Setting>.OnValueChanged
        {
            add => OnValueChanged += value;
            remove => OnValueChanged -= value;
        }
    }
}

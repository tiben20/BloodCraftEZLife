using System;
using BloodCraftEZLife.Config;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;
using BloodCraftEZLife.Utils;
using Il2CppSystem.Data;
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
        public ToggleRef Checkbox { get; set; }

        private Slider _slider;
        private LabelRef _label;
        private LabelRef _valueLabel;

        public string Label
        {
            get => _label.TextMesh.text;
            set => _label.TextMesh.text = value;
        }

        public float Value
        {
            get => _slider.value;
            set => _slider.value = value;
        }

        public float MinValue { get; set; } = 0f;
        public float MaxValue { get; set; } = 100f;

        public int CurrentDataIndex { get; set; }
        public override float DefaultHeight => 25f;
        private Setting _setting;
        private bool _subCreated;

        public event Action<Setting> OnActionInput;

        public override GameObject CreateContent(GameObject parent)
        {
            //creating the group horizontal group
            UIRoot = UIFactory.CreateHorizontalGroup(UIRoot, "TextScalingRow", false, false, true, true, spacing: 10,bgColor: new Color32(18, 18, 18, 255));
            UIFactory.SetLayoutElement(UIRoot, minHeight: 40, flexibleWidth: 9999);

            
            Rect = UIRoot.GetComponent<RectTransform>();
            Rect.anchorMin = new Vector2(0, 1);
            Rect.anchorMax = new Vector2(0, 1);
            Rect.pivot = new Vector2(0.5f, 1);
            Rect.sizeDelta = new Vector2(25, 25);


            _label = UIFactory.CreateLabel(UIRoot, "LabelTitle", "setting.Name",
                                              TextAlignmentOptions.MidlineLeft, fontSize: 16, bold: false);// medium-light gray
            UIFactory.SetLayoutElement(_label.GameObject, minHeight: 40, minWidth: 120, flexibleWidth: 9999);

            
            _subCreated = false;
            return UIRoot;
        }
        public void CreateHeader(Setting setting)
        {
            _label.TextMesh.text = (string)setting.Value;
            _label.TextMesh.fontSize = 22;
            _label.TextMesh.color = new Color32(230, 230, 230, 255);

        }

        public void CreateToggle(Setting setting)
        {
            var rowBtn = UIRoot.AddComponent<Button>();
            var colors = rowBtn.colors;
            colors.normalColor = new Color(0, 0, 0, 0);
            colors.highlightedColor = new Color(0.1f, 0.1f, 0.1f, 0.5f); // hover highlight
            rowBtn.colors = colors;
            // === Toggle (right) ===
            Checkbox = UIFactory.CreateToggle(UIRoot, "ToggleSetting"); // dark bluish gray
            UIFactory.SetLayoutElement(Checkbox.GameObject, minWidth: 30, minHeight: 30, flexibleWidth: 0, flexibleHeight: 0);

            
            // === Bind callback ===
            Checkbox.OnValueChanged += (bool newvalue) =>
            {
                Checkbox.Text.text = newvalue.ToString();
                setting.Value = newvalue;
                OnValueChanged?.Invoke(setting);
                // Example: Debug.Log(labelText + " set to " + val);
            };
            rowBtn.onClick.AddListener(() => Checkbox.Toggle.isOn = !Checkbox.Toggle.isOn);
        }

        public void CreateDropdown(Setting setting)
        {
            GameObject allSceneDropObj = UIFactory.CreateDropdown(UIRoot, "DropdownSetting", out var allSceneDropdown, "", 14, null);
            UIFactory.SetLayoutElement(allSceneDropObj, minHeight: 30, minWidth: 150, flexibleWidth: 0, flexibleHeight: 0);
            foreach (var opt in setting.Options)
            {
                allSceneDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData(opt));
            }

            allSceneDropdown.onValueChanged.AddListener((int val) =>
            {
                setting.Value = val;
                OnValueChanged?.Invoke(setting);
            });
        }

        public void CreateSlider(Setting setting)
        {
            // === Container row ===

            // === Label (left) ===
            //_label = UIFactory.CreateLabel(UIRoot, "TextScalingLabel", "Text Scaling",
            //                                 TextAlignmentOptions.MidlineLeft, fontSize: 16, bold: false,color: new Color32(160, 160, 160, 255));// medium-light gray

            // === Slider (middle) ===
            UIFactory.CreateSlider(UIRoot, "AmountSlider", out _slider);
            _slider.value = (float)5f;
            _slider.m_MaxValue = 100f;
            _slider.m_MinValue = 0f;
            //var slider = UIFactory.CreateSlider(row.gameObject, "TextScalingSlider", setting.Min, setting.Min, setting.Value, out var sliderComp);
            UIFactory.SetLayoutElement(_slider.gameObject, minWidth: 300, minHeight: 30, flexibleWidth: 1);

            // background color (dark bluish gray)
            Image sliderimg = _slider.transform.FindChild("Background").GetComponent<Image>();
            sliderimg.color = new Color32(16, 24, 32, 255);
            
            // handle color (steel blue)
            var handle = _slider.transform.Find("Handle Slide Area/Handle");
            if (handle != null)
                handle.GetComponent<Image>().color = new Color32(64, 96, 128, 255);

            // fill color (active track)
            var fill = _slider.transform.Find("Fill Area/Fill");
            if (fill != null)
                fill.GetComponent<Image>().color = new Color32(90, 155, 213, 255);

            // === Value label (right) ===
            var valueLabel = UIFactory.CreateLabel(UIRoot.gameObject, "TextScalingValue", "yo",
                                                   TextAlignmentOptions.MidlineLeft, fontSize: 16,color: new Color32(218, 218, 218, 255));// near-white for emphasis
            
            UIFactory.SetLayoutElement(valueLabel.GameObject, minWidth: 40, flexibleWidth: 0);

            // === Value binding ===
            _slider.onValueChanged.AddListener((val) =>
            {
                valueLabel.TextMesh.text = val.ToString("0.00");
                setting.Value = val;
                OnValueChanged?.Invoke(setting);
            });
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

        public void SetOpacity(float opacity)
        {
            if (_setting == null)
                return;
            var bg = UIRoot.GetComponent<Image>();
            if (bg == null)
                return;
            bg.color = new Color(0.11f, 0.11f, 0.11f).GetTransparent(Settings.UITransparency);
            if (_setting.Type == Setting.SettingType.Bool)
            {
               /* var bg = Checkbox.GameObject.GetComponentByName("Background");
                if (bg != null && bg.GetType().Name == "Image")
                {
                    Image bgImage = (Image)bg;
                    bgImage.color = new Color(0.11f, 0.11f, 0.11f).GetTransparent(Settings.UITransparency);
                }*/
                
            }
            else if (_setting.Type == Setting.SettingType.Float)
            {
            }
        }

        public void SetValue(Setting value)
        {
            SetLabel(value.Name);
            switch (_setting.Type)
            {
                case Setting.SettingType.Bool:
                    Checkbox.Toggle.Set((bool)value.Value, false);
                    break;
                case Setting.SettingType.String:
                    break;
                case Setting.SettingType.Float:
                    _slider.value = (float)value.Value;
                    _slider.m_MaxValue = value.Max;
                    _slider.m_MinValue = value.Min;
                    //_slider.stepSize = value.Step;
                    break;
                default:
                    break;
            }
            
            _setting = value;

        }

        public Setting GetValue() => _setting;
        public void SetLabel(string label)
        {
            switch (_setting.Type)
            {
                case Setting.SettingType.Bool:
                    _label.TextMesh.text = label;
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

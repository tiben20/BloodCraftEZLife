using System;
using BloodCraftEZLife.Config;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;
using BloodCraftEZLife.Utils;
using BloodmoonUI.UI.CustomLib.Cells;
using Il2CppSystem.Data;
using Stunlock.Localization;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities.DebugProxies;
using UnityEngine;
using UnityEngine.UI;
using static Il2CppSystem.Linq.Expressions.Interpreter.NullableMethodCallInstruction;
using static UnityEngine.Rendering.ProbeReferenceVolume;


namespace BloodCraftEZLife.UI.CustomLib.Cells
{
    public interface IFormedCheckbox : ICell
    {
        public int CurrentDataIndex { get; set; }
        public Action<int,bool> OnValueChanged { get; set; }
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

        public event System.Action<float> OnFloatValueChanged;

        public int CurrentDataIndex { get; set; }
        public override float DefaultHeight => 25f;
        private Setting _setting;
        private bool _subCreated;
        public override GameObject CreateContent(GameObject parent)
        {
            //creating the group horizontal group
            UIRoot = UIFactory.CreateHorizontalGroup(parent, "ConfigboxCell", true, false, true, true,2, default,
                new Color(0.11f, 0.11f, 0.11f).GetTransparent(Settings.UITransparency), TextAnchor.MiddleCenter);
            Rect = UIRoot.GetComponent<RectTransform>();
            Rect.anchorMin = new Vector2(0, 1);
            Rect.anchorMax = new Vector2(0, 1);
            Rect.pivot = new Vector2(0.5f, 1);
            Rect.sizeDelta = new Vector2(25, 25);
            UIFactory.SetLayoutElement(UIRoot, minWidth: 100, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);

            UIRoot.SetActive(false);

            

            _subCreated = false;
            return UIRoot;
        }
        public void InitialiseSetting(Setting set)
        {
            _setting = set;
            if (!_subCreated)
            {
                if (set.Type == Setting.SettingType.Bool)
                {
                    Checkbox = UIFactory.CreateToggle(UIRoot, "Checkbox", new Color(0.11f, 0.11f, 0.11f).GetTransparent(Settings.UITransparency),160);
                    UIFactory.SetLayoutElement(Checkbox.GameObject, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);
                    Checkbox.OnValueChanged += (bool newvalue) => { OnValueChanged?.Invoke(new Setting(Checkbox.Text.text, newvalue), CurrentDataIndex); };
                }
                else if (set.Type == Setting.SettingType.Float)
                {
                    // Slider
                    //_slider = UIFactory.CreateSlider(UIRoot, "Slider", MinValue, MaxValue, Value, out var sliderValueText);
                    // Create the slider itself
                    
                    
                    // Add label to show value
                    _label = UIFactory.CreateLabel(UIRoot, "SliderNameLabel", "1",TextAlignmentOptions.Left);
                    UIFactory.CreateSlider(UIRoot, "AmountSlider", out _slider);
                    _slider.value = 1.0f;
                    _slider.m_MaxValue = 100.0f;
                    _slider.m_MinValue = 1.0f;
                    UIFactory.SetLayoutElement(_slider.gameObject, minHeight: 25);
                    _valueLabel = UIFactory.CreateLabel(UIRoot, "SliderValueLabel", "1",TextAlignmentOptions.Right);
                    

                    _slider.onValueChanged.AddListener((val) =>
                    {
                        _valueLabel.TextMesh.text = ((int)val).ToString();
                        _setting.Value = val;
                        OnValueChanged?.Invoke(_setting, CurrentDataIndex);
                    });
                   

                }
                _subCreated = true;
            }
            
        }


        public Action<Setting, int> OnValueChanged { get; set; }

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
                    Checkbox.Text.text = label;
                    break;
                case Setting.SettingType.String:
                    
                    break;
                case Setting.SettingType.Float:
                    _label.TextMesh.text = label;
                    break;
                default:
                    break;
            }
            
            _setting.Name = label;
            
        }

        public int GetIndex()
        {
            return CurrentDataIndex;
        }

        public void SetIndex(int value)
        {
            CurrentDataIndex = value;
        }

        event System.Action<Setting, int> IConfigurableCell<Setting>.OnValueChanged
        {
            add => OnValueChanged += value;
            remove => OnValueChanged -= value;
        }
    }
}

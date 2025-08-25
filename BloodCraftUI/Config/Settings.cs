using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BloodCraftEZLife.Services;
using BloodCraftEZLife.UI.ModContent;
using BloodCraftEZLife.Utils;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace BloodCraftEZLife.Config
{
    public class Setting
    {
        public enum SettingType
        {
            Bool,
            String,
            Float,
            Header,
            Dropdown
        }

        public Setting(string configname, string name, bool value)
        {
            Name = name;    
            Value = value;
            Type = SettingType.Bool;
            ConfigName = configname;
        }

        public Setting(string configname, string name, string value, List<string> options)
        {
            Name = name;
            Value = value;
            Options = options;
            Type = SettingType.Dropdown;
            ConfigName = configname;
        }

        public Setting(string configname, string name, float value, float min, float max, float step)
        {
            Name = name;
            Value = value;
            Type = SettingType.Float;
            Min = min;
            Max = max;
            Step = step;
            ConfigName = configname;
        }
        public Setting(string configname, string name, string value,bool header = false)
        {
            Name = name;
            Value = value;
            Type = SettingType.String;
            ConfigName = configname;
        }
        public Setting(string name, string value)
        {
            Name = name;
            Value = value;
            Type = SettingType.Header;
            ConfigName = "HEADER";

        }
        public string ConfigName { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }
        public float Step { get; set; }
        public SettingType Type { get; set; }
        public List<string> Options { get; set; }
    }



    public class Settings
    {
        public static string CONFIG_PATH = Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_NAME);
        private static readonly Dictionary<string, ConfigEntryBase> ConfigEntries = new();
        public const string UI_SETTINGS_GROUP = "UISettings";
        public const string FAM_SETTINGS_GROUP = "FamiliarSettings";
        
        public enum SettingsId
        { 
            TpPanel,
            AutoTp,
            Header,
            Opacity
        }


        #region "TELEPORT"
        
        #endregion

        #region "Settings"
        public static readonly List<Setting> _settingList = new();
        public static List<Setting> GetSettingsEntries()
        {
            return _settingList;
        }
        public static void AddSettingHeader(string name, string settingValue)
        {
            if (_settingList.Any(a => a.Name.Equals(name)))
                return;
            _settingList.Add(new Setting(name, settingValue));
        }

        public static void AddSettingList(string configname, string name, string settingValue, List<string> options)
        {
            if (_settingList.Any(a => a.ConfigName.Equals(configname)))
                return;
            _settingList.Add(new Setting(configname,name, settingValue, options));
        }

        public static void AddSettingBool(string configname, string name,bool settingValue)
        {
            if (_settingList.Any(a => a.ConfigName.Equals(configname)))
                return;
            _settingList.Add(new Setting(configname,name, settingValue));
        }
        public static void AddSettingSlider(string configname, string name, float settingValue,float min, float max, float step)
        {
            if (_settingList.Any(a => a.ConfigName.Equals(configname)))
                return;
            _settingList.Add(new Setting(configname,name, settingValue,min,max,step));
        }
        #endregion


        public static bool UseHorizontalContentLayout =>
            (ConfigEntries[nameof(UseHorizontalContentLayout)] as ConfigEntry<bool>)?.Value ?? true;

        public static int GlobalQueryIntervalInSeconds { get; } = 2;

        /*public static float UITransparency =>
            (ConfigEntries[nameof(UITransparency)] as ConfigEntry<float>)?.Value ?? 0.6f;*/
        public static void SetConfigValue(string nameofsetting, object value)
        {
            var type = typeof(Settings);
            var prop = type.GetProperty(nameofsetting, BindingFlags.Static | BindingFlags.Public);
            if (prop != null && prop.CanWrite)
            {
                // null instance because it's static
                prop.SetValue(null, value);
            }
            else
            {
                LogUtils.LogError($"Property {nameofsetting} not found or read-only");
            }
        }

        public static float UITransparency
        {
            get => (ConfigEntries[nameof(UITransparency)] as ConfigEntry<float>)?.Value ?? 0.6f;
            set
            {
                ConfigEntries[nameof(UITransparency)].BoxedValue = value;
            } 
        }

        public static string UiTextSize
        {
            get => (ConfigEntries[nameof(UiTextSize)] as ConfigEntry<string>)?.Value ?? "Small";
            set => ConfigEntries[nameof(UiTextSize)].BoxedValue = value;
        }

        public static bool IsTeleportPanelEnabled
        {
            get => (ConfigEntries[nameof(IsTeleportPanelEnabled)] as ConfigEntry<bool>)?.Value ?? true;
            set 
            { 
                ConfigEntries[nameof(IsTeleportPanelEnabled)].BoxedValue = value;
                ContentPanel panel;
                panel = (ContentPanel)Plugin.UIManager._contentPanel;
                if (panel != null)
                {
                    panel.ToggleGameObject(value, "TeleportListButton");
                }
            }
        }

        public static bool IsHeaderVisible
        {
            get => (ConfigEntries[nameof(IsHeaderVisible)] as ConfigEntry<bool>)?.Value ?? true;
            set 
            { 
                ConfigEntries[nameof(IsHeaderVisible)].BoxedValue = value;
                ContentPanel panel;
                panel = (ContentPanel)Plugin.UIManager._contentPanel;
                if (panel != null)
                {
                    panel.ToggleGameObject(value, "header");
                }
            }
        }

        public static bool KeepTrackOfVbloodKills
        {
            get => (ConfigEntries[nameof(KeepTrackOfVbloodKills)] as ConfigEntry<bool>)?.Value ?? true;
            set => ConfigEntries[nameof(KeepTrackOfVbloodKills)].BoxedValue = value;
        }

        public static bool IsAutoTeleportEnabled
        {
            get => (ConfigEntries[nameof(IsAutoTeleportEnabled)] as ConfigEntry<bool>)?.Value ?? true;
            set => ConfigEntries[nameof(IsAutoTeleportEnabled)].BoxedValue = value;
        }
    
        public Settings InitConfig()
        {
            if (!Directory.Exists(CONFIG_PATH))
            {
                Directory.CreateDirectory(CONFIG_PATH);
            }
            
            InitConfigEntry(UI_SETTINGS_GROUP, nameof(UseHorizontalContentLayout), true,
                "Use horizontal or vertical layout for main content panel");
            InitConfigEntry(UI_SETTINGS_GROUP, nameof(UITransparency), 0.6f,
                "Set transparency for all panels between 1.0f as opaque and 0f as transparent");
            InitConfigEntry(UI_SETTINGS_GROUP, nameof(IsTeleportPanelEnabled), true, "Is teleport box panel enabled");
            InitConfigEntry(UI_SETTINGS_GROUP, nameof(IsAutoTeleportEnabled), true, "Is auto teleport enabled");
            InitConfigEntry(UI_SETTINGS_GROUP, nameof(IsHeaderVisible), true, "Is title of plugin visible");
            InitConfigEntry(UI_SETTINGS_GROUP, nameof(KeepTrackOfVbloodKills), true, "Keep track of vblood kills");

            /*Adding settings the order is important for the index*/
            AddSettingHeader("General options", "General");
            AddSettingBool(nameof(IsTeleportPanelEnabled),"Teleport panel", Settings.IsTeleportPanelEnabled);
            AddSettingBool(nameof(IsAutoTeleportEnabled),"Auto teleport", Settings.IsAutoTeleportEnabled);
            AddSettingBool(nameof(IsHeaderVisible), "Header visible",Settings.IsHeaderVisible);
            AddSettingBool(nameof(KeepTrackOfVbloodKills), "Keep track of vblood kills", Settings.KeepTrackOfVbloodKills);
            /*AddSettingSlider(nameof(UITransparency),"UI Opacity", Settings.UITransparency,0.1f,1.0f,0.05f);
            List<string> options =new List<string>();
            options.Add("Small");
            options.Add("Medium");
            options.Add("Large");
            AddSettingList(nameof(UiTextSize),"Text size", "Small",options);*/

            
            return this;
        }

        private static ConfigEntry<T> InitConfigEntry<T>(string section, string key, T defaultValue, string description)
        {
            // Bind the configuration entry and get its value
            var entry = Plugin.Instance.Config.Bind(section, key, defaultValue, description);

            // Check if the key exists in the configuration file and retrieve its current value
            var newFile = Path.Combine(Paths.ConfigPath, $"{PluginInfo.PLUGIN_GUID}.cfg");

            if (File.Exists(newFile))
            {
                var config = new ConfigFile(newFile, true);
                if (config.TryGetEntry(section, key, out ConfigEntry<T> existingEntry))
                {
                    // If the entry exists, update the value to the existing value
                    entry.Value = existingEntry.Value;
                }
            }

            ConfigEntries.Add(key, entry);
            return entry;
        }
    }
}
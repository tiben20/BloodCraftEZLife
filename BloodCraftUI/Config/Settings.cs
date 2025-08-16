using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;

namespace BloodCraftEZLife.Config
{
    public class Setting
    {
        public enum SettingType
        {
            Bool,
            String,
            Float
        }
        public Setting(string name, bool value)
        {
            Name = name;    
            Value = value;
            Type = SettingType.Bool;
        }
        public Setting(string name, float value, float min, float max, float step)
        {
            Name = name;
            Value = value;
            Type = SettingType.Float;
            Min = min;
            Max = max;
            Step = step;
        }
        public Setting(string name, string value)
        {
            Name = name;
            Value = value;
            Type = SettingType.String;
        }
        public string Name { get; set; }
        public object Value { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }
        public float Step { get; set; }
        public SettingType Type { get; set; }
    }



    public class Settings
    {
        private static string CONFIG_PATH = Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_NAME);
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
        public class TeleportBoxData
        {
            public string Name { get; set; }
        }
        public static readonly List<TeleportBoxData> _dataList = new();
        public static List<TeleportBoxData> GetTeleportEntries() => _dataList;
        public static void AddListEntry(string name)
        {
            if (_dataList.Any(a => a.Name.Equals(name)))
                return;
            _dataList.Add(new TeleportBoxData { Name = name });
            
        }
        #endregion

        #region "Settings"
        public static readonly List<Setting> _settingList = new();
        public static List<Setting> GetSettingsEntries() => _settingList;
        public static void AddSettingEntry(string name,bool settingValue)
        {
            if (_settingList.Any(a => a.Name.Equals(name)))
                return;
            _settingList.Add(new Setting(name, settingValue));
        }
        public static void AddSettingEntry(string name, float settingValue,float min, float max, float step)
        {
            if (_settingList.Any(a => a.Name.Equals(name)))
                return;
            _settingList.Add(new Setting(name, settingValue,min,max,step));
        }
        #endregion

        public static bool UseHorizontalContentLayout =>
            (ConfigEntries[nameof(UseHorizontalContentLayout)] as ConfigEntry<bool>)?.Value ?? true;

        public static int GlobalQueryIntervalInSeconds { get; } = 2;

        /*public static float UITransparency =>
            (ConfigEntries[nameof(UITransparency)] as ConfigEntry<float>)?.Value ?? 0.6f;*/

        public static float UITransparency
        {
            get => (ConfigEntries[nameof(UITransparency)] as ConfigEntry<float>)?.Value ?? 0.6f;
            set => ConfigEntries[nameof(UITransparency)].BoxedValue = value;
        }

        public static bool IsTeleportPanelEnabled
        {
            get => (ConfigEntries[nameof(IsTeleportPanelEnabled)] as ConfigEntry<bool>)?.Value ?? true;
            set => ConfigEntries[nameof(IsTeleportPanelEnabled)].BoxedValue = value;
        }

        public static bool IsHeaderVisible
        {
            get => (ConfigEntries[nameof(IsHeaderVisible)] as ConfigEntry<bool>)?.Value ?? true;
            set => ConfigEntries[nameof(IsHeaderVisible)].BoxedValue = value;
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
            /*Adding settings the order is important for the index*/
            AddSettingEntry("Teleport panel", Settings.IsTeleportPanelEnabled);
            AddSettingEntry("Autoteleport", Settings.IsAutoTeleportEnabled);
            AddSettingEntry("Header visible", Settings.IsHeaderVisible);
            AddSettingEntry("UI Opacity", Settings.UITransparency,0.1f,1.0f,0.05f);

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
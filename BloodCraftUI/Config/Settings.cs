using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;

namespace BloodCraftEZLife.Config
{
    public class Setting
    {
        public string Name { get; set; }
        public bool Value { get; set; }
    }

    public class TeleportBoxData
    {
        public string Name { get; set; }
    }
    public class Settings
    {
        private static string CONFIG_PATH = Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_NAME);
        private static readonly Dictionary<string, ConfigEntryBase> ConfigEntries = new();
        public const string UI_SETTINGS_GROUP = "UISettings";
        public const string FAM_SETTINGS_GROUP = "FamiliarSettings";
        public const string GENERAL_SETTINGS_GROUP = "GeneralOptions";



        #region "TELEPORT"
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
            _settingList.Add(new Setting { Name = name, Value = settingValue });
        }
        #endregion

        public static bool UseHorizontalContentLayout =>
            (ConfigEntries[nameof(UseHorizontalContentLayout)] as ConfigEntry<bool>)?.Value ?? true;

        public static int GlobalQueryIntervalInSeconds { get; } = 2;

        public static float UITransparency =>
            (ConfigEntries[nameof(UITransparency)] as ConfigEntry<float>)?.Value ?? 0.6f;

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

        /*public static bool IsFamStatsPanelEnabled => (ConfigEntries[nameof(IsFamStatsPanelEnabled)] as ConfigEntry<bool>)?.Value ?? true;
        public static bool IsBoxPanelEnabled      => (ConfigEntries[nameof(IsBoxPanelEnabled)] as ConfigEntry<bool>)?.Value ?? true;
        public static bool IsBindButtonEnabled    => (ConfigEntries[nameof(IsBindButtonEnabled)] as ConfigEntry<bool>)?.Value ?? true;
        public static bool IsCombatButtonEnabled  => (ConfigEntries[nameof(IsCombatButtonEnabled)] as ConfigEntry<bool>)?.Value ?? true;
        public static bool AutoEnableFamiliarEquipment => (ConfigEntries[nameof(AutoEnableFamiliarEquipment)] as ConfigEntry<bool>)?.Value ?? true;
        public static bool IsPrestigeButtonEnabled => (ConfigEntries[nameof(IsPrestigeButtonEnabled)] as ConfigEntry<bool>)?.Value ?? true;
        public static bool IsToggleButtonEnabled => (ConfigEntries[nameof(IsToggleButtonEnabled)] as ConfigEntry<bool>)?.Value ?? true;*/
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
            AddSettingEntry("Teleport panel", Settings.IsTeleportPanelEnabled);
            AddSettingEntry("Autoteleport", Settings.IsAutoTeleportEnabled);
            AddSettingEntry("Header visible", Settings.IsHeaderVisible);
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
using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Configuration;

namespace BloodCraftUI.Config
{
    public class Settings
    {
        private static string CONFIG_PATH = Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_NAME);
        private static readonly List<ConfigEntryBase> ConfigEntries = new();
        public const string UI_SETTINGS_GROUP = "UISettings";
        public static bool ClearServerMessages => (ConfigEntries[0] as ConfigEntry<bool>)?.Value ?? false;
        public static int FamStatsQueryIntervalInSeconds
        {
            get
            {
                var value = (ConfigEntries[1] as ConfigEntry<int>)?.Value ?? 10;
                if (value < 5) value = 5;
                return value;
            }
        }
        public static bool UseHorizontalLayout => (ConfigEntries[2] as ConfigEntry<bool>)?.Value ?? true;

        public static int GlobalQueryIntervalInSeconds { get; } = 2;


        public Settings InitConfig()
        {
            if (!Directory.Exists(CONFIG_PATH))
            {
                Directory.CreateDirectory(CONFIG_PATH);
            }

            ConfigEntries.Add(InitConfigEntry("GeneralOptions",  "ClearServerMessages", true, "Clear server and command messages from chat"));
            ConfigEntries.Add(InitConfigEntry("GeneralOptions",  "FamStatsQueryIntervalInSeconds", 10, "Query interval for familiar stats update (no less than 10 sec)"));
            ConfigEntries.Add(InitConfigEntry(UI_SETTINGS_GROUP, "UseHorizontalContentLayout", true, "Use horizontal or vertical layout for main content panel"));
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
            return entry;
        }
    }
}

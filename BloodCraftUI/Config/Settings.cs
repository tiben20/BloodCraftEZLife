using System.Collections.Generic;
using System.IO;
using BepInEx;
using BepInEx.Configuration;

namespace BloodCraftUI.Config
{
    public class Settings
    {
        public const string BCCOM_LISTBOXES = ".fam boxes";
        public const string BCCOM_SWITCHBOX = ".fam cb {0}";
        public const string BCCOM_BOXCONTENT = ".fam l";
        public const string BCCOM_BINDFAM = ".fam b {0}";
        public const string BCCOM_UNBINDFAM = ".fam ub";

        private static string CONFIG_PATH = Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_NAME);

        public static bool ClearServerMessages => (_configEntries[0] as ConfigEntry<bool>)?.Value ?? false;

        private static List<ConfigEntryBase> _configEntries = new();

        public void InitConfig()
        {
            if (!Directory.Exists(CONFIG_PATH))
            {
                Directory.CreateDirectory(CONFIG_PATH);
            }

            _configEntries.Add(InitConfigEntry("GeneralOptions", "ClearServerMessages", false, "Enable/Disable"));
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

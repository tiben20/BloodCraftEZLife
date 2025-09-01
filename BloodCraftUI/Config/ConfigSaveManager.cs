using BloodCraftEZLife.UI.ModContent.Data;
using Newtonsoft.Json;
using Newtonsoft.Json;
using ProjectM.Network;
using ProjectM.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BloodCraftEZLife.Config
{
    public static class ConfigSaveManager
    {
        private static string SavePath;
        
        private static SettingsHotkeys _settingsHotkeys;
        private static SettingsPerServer _settingsPerServer;

        public static Dictionary<KeyCode, string> Hotkeys => _settingsHotkeys.HotKeys;
        public static SettingsVblood VBloodKills => _settingsPerServer.VBloods;
        public static SettingsChatMessage ChatMessages => _settingsPerServer.ChatMessages;
        public static List<string> GetUsers() => _settingsPerServer.ChatMessages.GetUsers;
        public static List<ChatMessage> GetMessages(string user) => _settingsPerServer.ChatMessages.GetMessages(user);

        public static void SavePerServer()
        {
            //var options = new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve };
            //string jsonoutput = JsonSerializer.Serialize(_settingsPerServer, options);
            string jsonoutput = JsonConvert.SerializeObject(_settingsPerServer, Formatting.Indented);
            File.WriteAllText(SavePath, jsonoutput);

        }

        public static void SaveGlobal(Dictionary<KeyCode, string> hk)
        {
            _settingsHotkeys.HotKeys = hk;
            string thepath = System.IO.Path.Combine(Settings.CONFIG_PATH, "hotkeys") + ".json";
            //var options = new JsonSerializerOptions { WriteIndented = true };
            //string jsonoutput = JsonSerializer.Serialize(_settingsHotkeys.HotKeys, options);
            string jsonoutput = JsonConvert.SerializeObject(_settingsHotkeys, Formatting.Indented);
            File.WriteAllText(thepath, jsonoutput);

        }

        public static void LoadGlobal()
        {
            string thepath = System.IO.Path.Combine(Settings.CONFIG_PATH, "hotkeys") + ".json";
            if (File.Exists(thepath))
            {
                string json = File.ReadAllText(thepath);
                _settingsHotkeys = JsonConvert.DeserializeObject<SettingsHotkeys>(json);
            }
            else
            {
                _settingsHotkeys = new SettingsHotkeys(); // empty if none
            }

        }

        public static void LoadPerServerSettings(string flepath)
        {
            SavePath = flepath;
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                _settingsPerServer = JsonConvert.DeserializeObject<SettingsPerServer>(json);
            }
            else
            {
                _settingsPerServer = new SettingsPerServer(); // empty if none
            }
        }
    }
}

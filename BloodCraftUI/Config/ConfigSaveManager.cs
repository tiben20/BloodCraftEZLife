using BloodCraftEZLife.UI.ModContent.Data;
using ProjectM.Network;
using ProjectM.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using TMPro;
using UnityEngine;

namespace BloodCraftEZLife.Config
{
    public static class ConfigSaveManager
    {
        private static string SavePathVblood;
        private static string SavePathChat;

        private static SettingsHotkeys _settingsHotkeys;
        private static SettingsVbloodParent _vBloods { get; set; } = new();
        private static SettingsChatMessage _chatMessages { get; set; } = new();
        public static Dictionary<KeyCode, string> Hotkeys => _settingsHotkeys.HotKeys;

        public static SettingsVblood VBloodKills => _vBloods.VBloods;
        public static SettingsVbloodParent VBloodKillsRoot => _vBloods;
        public static SettingsChatMessage ChatMessages => _chatMessages;
        public static List<string> GetUsers() => _chatMessages.GetUsers;
        public static List<ChatMessage> GetMessages(string user) => _chatMessages.GetMessages(user);

        public static void SavePerServerChat()
        {
            var options = new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles };
            string jsonoutput = JsonSerializer.Serialize(ChatMessages, options);
            //string jsonoutput = JsonConvert.SerializeObject(_settingsPerServer, Formatting.Indented);
            File.WriteAllText(SavePathChat, jsonoutput);

        }

        public static void SavePerServerVbloods()
        {
            var options = new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles };
            string jsonoutput = JsonSerializer.Serialize(VBloodKillsRoot, options);
            //string jsonoutput = JsonConvert.SerializeObject(_settingsPerServer, Formatting.Indented);
            File.WriteAllText(SavePathVblood, jsonoutput);

        }
        public static void SaveGlobal(Dictionary<KeyCode, string> hk)
        {
            _settingsHotkeys.HotKeys = hk;
            string thepath = System.IO.Path.Combine(Settings.CONFIG_PATH, "hotkeys") + ".json";
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonoutput = JsonSerializer.Serialize(_settingsHotkeys.HotKeys, options);
            //string jsonoutput = JsonConvert.SerializeObject(_settingsHotkeys, Formatting.Indented);
            File.WriteAllText(thepath, jsonoutput);

        }
        private static string FixJson(string json)
        {
            string result="";
            using (StringReader reader = new StringReader(json))
            {
                string line = string.Empty;
                do
                {
                    line = reader.ReadLine();
                    if (line != null)
                    {
                        if (!line.Contains("""$id"""))
                        {
                            result += line + "\n";
                            // do something with the line
                        }
                    }

                } while (line != null);
            }
            return result;
        
        }
        public static void LoadGlobal()
        {
            string thepath = System.IO.Path.Combine(Settings.CONFIG_PATH, "hotkeys") + ".json";
            if (File.Exists(thepath))
            {
                try
                {
                    string json = File.ReadAllText(thepath);
                    _settingsHotkeys = new SettingsHotkeys();
                    json = FixJson(json);
                    _settingsHotkeys.HotKeys = JsonSerializer.Deserialize<Dictionary<KeyCode, string>>(json);
                }
                catch (Exception ex)
                {
                    _settingsHotkeys = new SettingsHotkeys(); // empty if bugged
                }
            }
            else
            {
                _settingsHotkeys = new SettingsHotkeys(); // empty if none
            }

        }

        public static void LoadPerServerSettings(string flepath)
        {
            SavePathVblood = System.IO.Path.Combine(Settings.CONFIG_PATH, flepath) + ".json";
            SavePathChat = System.IO.Path.Combine(Settings.CONFIG_PATH, flepath) + "chat.json";
            if (File.Exists(SavePathVblood))
            {
                try
                {
                    string json = File.ReadAllText(SavePathVblood);
                    json = FixJson(json);
                    _vBloods = JsonSerializer.Deserialize<SettingsVbloodParent>(json);
                }
                catch (Exception)
                {
                    _vBloods = new SettingsVbloodParent(); // empty if bugged
                }
                
            }
            else
            {
                _vBloods = new SettingsVbloodParent(); // empty if none
            }
            if (File.Exists(SavePathChat))
            {
                try
                {
                    string json = File.ReadAllText(SavePathChat);
                    json = FixJson(json);
                    _chatMessages = JsonSerializer.Deserialize<SettingsChatMessage>(json);
                }
                catch (Exception)
                {
                _chatMessages = new SettingsChatMessage(); // empty if bugged
            }
        }
            else
            {
                _chatMessages = new SettingsChatMessage(); // empty if none
            }
        }
    }
}

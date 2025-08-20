using BloodCraftEZLife.UI.ModContent;
using ProjectM;
using System.Collections.Generic;
using System.IO;

using System.Text.Json;
using UnityEngine;
namespace BloodCraftEZLife.Config
{

    
    public class SettingsVblood
    {
        float thetime;
        string lastadded;
        public SettingsVblood()
        {
            VBloodKills = new Dictionary<string, int> { };
            lastadded = "";
            thetime = 0f;
        }
        // Must be a field, not a property
        //public List<string> items = new List<string>();
        public Dictionary<string, int> VBloodKills { get; set; }

        public void AddVbloodKill(string name)
        {
            if (name == lastadded)
            {
                float timeelapsed = Time.time - thetime;
                if (timeelapsed < 60f)
                {
                    return;
                }
            }
            if (VBloodKills.ContainsKey(name))
            {
                VBloodKills[name] += 1;
            }
            else 
            {
                VBloodKills.Add(name, 1);
            }
            var panel = Plugin.UIManager.GetPanel<PopupPanel>();
            if (panel != null)
            {
                panel.ShowMessage("Killed " + name + " " + VBloodKills[name].ToString() + " times");
            }
            thetime = Time.time;
            lastadded = name;
        }

    }

    public static class ConfigSaveManager
    {
        private static string SavePath;


        public static void Save(SettingsVblood data)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonoutput = JsonSerializer.Serialize(data, options);
            File.WriteAllText(SavePath, jsonoutput);

        }

        public static SettingsVblood Load(string flepath)
        {
            SavePath = flepath;
            if (File.Exists(SavePath))
            {
                string json = File.ReadAllText(SavePath);
                return JsonSerializer.Deserialize<SettingsVblood>(json);
            }
            else
            {
                return new SettingsVblood(); // empty if none
            }
        }
    }
}

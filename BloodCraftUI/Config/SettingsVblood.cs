using BloodCraftEZLife.Services;
using BloodCraftEZLife.UI.CustomLib.Util;
using BloodCraftEZLife.UI.ModContent;
using Epic.OnlineServices;
using ProjectM;
using System.Collections.Generic;
using System.IO;

using System.Text.Json;
using System.Xml;
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
                panel.ShowMessage("Killed " + name + " " + VBloodKills[name].ToString() + " times",7f);
            }
            thetime = Time.time;
            lastadded = name;
        }

    }

    
}

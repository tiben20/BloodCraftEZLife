using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace BloodCraftEZLife.Config
{
    public class SettingsHotkeys
    {
        public Dictionary<KeyCode, string> HotKeys { get; set; }
        public SettingsHotkeys()
        {
            HotKeys = new Dictionary<KeyCode, string> { };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodCraftEZLife.Config
{
    
    public class SettingsPerServer
    {
        public SettingsVblood VBloods { get; set; } = new();
        public SettingsChatMessage ChatMessages { get; set; } = new();

    }
}

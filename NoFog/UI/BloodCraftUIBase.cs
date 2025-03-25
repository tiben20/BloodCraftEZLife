using System;
using UniverseLib.UI.Panels;
using UniverseLib.UI;
using NoFog.NEW;

namespace NoFog.UI
{
    internal class BloodCraftUIBase : UIBase
    {
        public BloodCraftUIBase(string id, Action updateMethod)
            : base(id, updateMethod) { }

        protected override PanelManager CreatePanelManager()
        {
            return new BCPanelManager(this);
        }
    }
}

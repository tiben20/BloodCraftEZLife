using System;

namespace BloodCraftUI.UI
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

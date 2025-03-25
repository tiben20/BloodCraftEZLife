using System;
using UnityEngine;
using UIBase = BloodCraftUI.NewUI.UICore.UniverseLib.UI.UIBase;

namespace BloodCraftUI.NewUI.UICore.UI.Panel
{
    public class ContentPanel : ResizeablePanelBase
    {
        public ContentPanel(UIBase owner) : base(owner)
        {
        }

        public override string Name { get; }
        public override int MinWidth { get; }
        public override int MinHeight { get; }
        public override Vector2 DefaultAnchorMin { get; }
        public override Vector2 DefaultAnchorMax { get; }
        protected override BCUIManager.Panels PanelType { get; }
        internal override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
using UnityEngine;
using UniverseLib.UI.Panels;
using UniverseLib.UI;

namespace NoFog.UI
{
    public class BCPanelManager : PanelManager
    {
        public BCPanelManager(UIBase owner) : base(owner) { }

        protected override Vector3 MousePosition => DisplayManager.MousePosition;

        protected override Vector2 ScreenDimensions => new(DisplayManager.Width, DisplayManager.Height);

        protected override bool MouseInTargetDisplay => DisplayManager.MouseInTargetDisplay;

        internal void DoInvokeOnPanelsReordered()
        {
            InvokeOnPanelsReordered();
        }

        protected override void SortDraggerHeirarchy()
        {
            base.SortDraggerHeirarchy();
        }
    }
}

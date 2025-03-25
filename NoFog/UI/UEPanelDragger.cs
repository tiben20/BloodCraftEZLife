using NoFog.NEW;
using UnityEngine;
using UniverseLib.UI.Panels;

namespace NoFog.UI
{
    public class UEPanelDragger : PanelDragger
    {
        public UEPanelDragger(PanelBase uiPanel) : base(uiPanel) { }

        protected override bool MouseInResizeArea(Vector2 mousePos)
        {
            return !UICustomManager.NavBarRect.rect.Contains(UICustomManager.NavBarRect.InverseTransformPoint(mousePos))
                   && base.MouseInResizeArea(mousePos);
        }
    }
}

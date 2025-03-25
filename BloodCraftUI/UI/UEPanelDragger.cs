using BloodCraftUI.NEW;
using UnityEngine;

namespace BloodCraftUI.UI
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

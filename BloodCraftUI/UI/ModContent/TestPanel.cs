using BloodCraftUI.Config;
using BloodCraftUI.UI.ModContent.Data;
using BloodCraftUI.UI.UniverseLib.UI;
using BloodCraftUI.UI.UniverseLib.UI.Models;
using BloodCraftUI.UI.UniverseLib.UI.Panels;
using UnityEngine;
using UIBase = BloodCraftUI.UI.UniverseLib.UI.UIBase;

namespace BloodCraftUI.UI.ModContent
{
    internal class TestPanel : UIBehaviourModel, IPanelBase
    {
        private GameObject _uiRoot;

        public UIBase Owner { get; }
        public RectTransform Rect { get; private set; }
        public PanelType PanelType => PanelType.TestPanel;
        public string PanelId => "TestPanel";
        public override GameObject UIRoot => _uiRoot;
        public PanelDragger Dragger { get; internal set; }
        public float Opacity => Settings.UITransparency;

        public TestPanel(UIBase uiBase)
        {
            Owner = uiBase;
            ConstructUI();
            Owner.Panels.AddPanel(this);
        }

        private void ConstructUI()
        {
            _uiRoot = UIFactory.CreatePanel(PanelId, Owner.Panels.PanelHolder, out GameObject contentRoot);
        }


        public override void Destroy()
        {
        }

        public void EnsureValidSize()
        {

        }

        public void EnsureValidPosition()
        {

        }

        public void SetActiveOnly(bool active)
        {
            throw new System.NotImplementedException();
        }
    }
}

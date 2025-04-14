using BloodCraftUI.Config;
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
        public BCUIManager.Panels PanelType => BCUIManager.Panels.TestPanel;
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


        public void Destroy()
        {
        }

        public void EnsureValidSize()
        {

        }

        public void EnsureValidPosition()
        {

        }
    }
}

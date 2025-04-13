using BloodCraftUI.NewUI;
using BloodCraftUI.NewUI.UniverseLib.UI;
using BloodCraftUI.NewUI.UniverseLib.UI.Panels;
using BloodCraftUI.Services;
using BloodCraftUI.UI.CustomLib.Controls;
using BloodCraftUI.UI.CustomLib.Panel;
using BloodCraftUI.Utils;
using UnityEngine;
using UIBase = BloodCraftUI.NewUI.UniverseLib.UI.UIBase;

namespace BloodCraftUI.UI.ModContent
{
    public class ContentPanel : ResizeablePanelBase
    {
        public override string PanelId => "CorePanel";
        public override int MinWidth => 340;
        public override int MinHeight => 25;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPosition => new Vector2(0f, Owner.Scaler.m_ReferenceResolution.y);
        public override bool CanDrag => true;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.None;
        public override BCUIManager.Panels PanelType => BCUIManager.Panels.Base;
        private GameObject _uiAnchor;
        private UIScaleSettingButton _scaleButtonData;

        public ContentPanel(UIBase owner) : base(owner)
        {
        }

        protected override void ConstructPanelContent()
        {
            TitleBar.SetActive(false);
            _uiAnchor = UIFactory.CreateHorizontalGroup(ContentRoot, "UIAnchor", true, true, true, true);
            Dragger.DraggableArea = Rect;
            Dragger.OnEndResize();

            var text = UIFactory.CreateLabel(_uiAnchor, "UIAnchorText", $"BCUI {PluginInfo.PLUGIN_VERSION}");
            UIFactory.SetLayoutElement(text.gameObject, 0, 25, 1, 1);

            var boxListButton = UIFactory.CreateButton(_uiAnchor, "BoxListButton", "Box List");
            UIFactory.SetLayoutElement(boxListButton.GameObject, ignoreLayout: false, minWidth: 80, minHeight: 25);
            boxListButton.OnClick = () =>
            {
                BCUIManager.AddPanel(BCUIManager.Panels.BoxList);
            };
            var famStatsButton = UIFactory.CreateButton(_uiAnchor, "FamStatsButton", "Fam Stats");
            UIFactory.SetLayoutElement(famStatsButton.GameObject, ignoreLayout: false, minWidth: 80, minHeight: 25);
            famStatsButton.OnClick = () =>
            {
                BCUIManager.AddPanel(BCUIManager.Panels.FamStats);
            };

            var unbindButton = UIFactory.CreateButton(_uiAnchor, "FamStatsButton", "Unbind");
            UIFactory.SetLayoutElement(unbindButton.GameObject, ignoreLayout: false, minWidth: 80, minHeight: 25);
            unbindButton.OnClick = () =>
            {
                unbindButton.Component.interactable = false;
                MessageService.EnqueueMessage(MessageService.BCCOM_UNBINDFAM);
                TimerHelper.OneTickTimer(2000, () => unbindButton.Component.interactable = true);
            };

            var scaleButton = UIFactory.CreateButton(_uiAnchor, "ScaleButton", "*");
            UIFactory.SetLayoutElement(scaleButton.GameObject, ignoreLayout: false, minWidth: 25, minHeight: 25);
            _scaleButtonData = new UIScaleSettingButton();
            scaleButton.OnClick = () =>
            {
                _scaleButtonData.PerformAction();
                var panel = BCUIManager.GetPanel<FamStatsPanel>();
                if(panel != null && panel.UIRoot.active)
                    panel.RecalculateHeight();
            };

            SetDefaultSizeAndPosition();
        }

        protected override void LateConstructUI()
        {
            base.LateConstructUI();
        }

        internal override void Reset()
        {
        }

        public override void Update()
        {
            base.Update();
            // Call update on the panels that need it
        }
    }
}
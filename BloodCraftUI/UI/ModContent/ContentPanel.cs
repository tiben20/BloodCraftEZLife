using System.Collections.Generic;
using BloodCraftUI.Config;
using BloodCraftUI.Services;
using BloodCraftUI.UI.CustomLib.Controls;
using BloodCraftUI.UI.CustomLib.Panel;
using BloodCraftUI.UI.CustomLib.Util;
using BloodCraftUI.UI.ModContent.Data;
using BloodCraftUI.UI.UniverseLib.UI;
using BloodCraftUI.UI.UniverseLib.UI.Panels;
using BloodCraftUI.Utils;
using UnityEngine;
using UnityEngine.UI;
using UIBase = BloodCraftUI.UI.UniverseLib.UI.UIBase;

namespace BloodCraftUI.UI.ModContent
{
    public class ContentPanel : ResizeablePanelBase
    {
        public override string PanelId => "CorePanel";

        public override int MinWidth => Settings.UseHorizontalContentLayout ? 340 : 100;
        //public override int MaxWidth => 150;

        public override int MinHeight => 25;
        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPosition => new Vector2(0f, Owner.Scaler.m_ReferenceResolution.y);
        public override bool CanDrag => true;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.None;
        public override PanelType PanelType => PanelType.Base;
        private GameObject _uiAnchor;
        private UIScaleSettingButton _scaleButtonData;
        private List<GameObject> _objectsList;
        private Toggle _pinToggle;
        public override float Opacity => Settings.UITransparency;

        public ContentPanel(UIBase owner) : base(owner)
        {
        }

        protected override void ConstructPanelContent()
        {
            TitleBar.SetActive(false);
            _uiAnchor = Settings.UseHorizontalContentLayout
                ? UIFactory.CreateHorizontalGroup(ContentRoot, "UIAnchor", true, true, true, true)
                : UIFactory.CreateVerticalGroup(ContentRoot, "UIAnchor", false, true, true, true, padding: new Vector4(5,5,5,5));

            Dragger.DraggableArea = Rect;
            Dragger.OnEndResize();

            _objectsList = new List<GameObject>();


            if (CanDrag)
            {
                // Create pin button as a child of ContentRoot (panel root) instead of _uiAnchor
                var pinButton = UIFactory.CreateToggle(_uiAnchor, "PinButton", out var pinToggle, out var pinText);
                // Set layout element to position it correctly
                UIFactory.SetLayoutElement(pinButton, minHeight: 15, preferredHeight: 15, flexibleHeight: 0,
                    minWidth: 15, preferredWidth: 15, flexibleWidth: 0, ignoreLayout: false);
                // Set RectTransform to position it at the top left
                //RectTransform pinRect = pinButton.GetComponent<RectTransform>();
                
                // Set toggle properties
                pinToggle.isOn = false;
                pinToggle.onValueChanged.AddListener(value => IsPinned = value);
                _pinToggle = pinToggle;

                // Make the label text empty or minimal
                pinText.text = " ";
            }

            var text = UIFactory.CreateLabel(_uiAnchor, "UIAnchorText", $"BCUI {PluginInfo.PLUGIN_VERSION}");
            UIFactory.SetLayoutElement(text.gameObject, 80, 25, 1, 1);
            _objectsList.Add(text.gameObject);

            if (Settings.IsBoxPanelEnabled)
            {
                var boxListButton = UIFactory.CreateButton(_uiAnchor, "BoxListButton", "Box List");
                UIFactory.SetLayoutElement(boxListButton.GameObject, ignoreLayout: false, minWidth: 80, minHeight: 25);
                _objectsList.Add(boxListButton.GameObject);
                boxListButton.OnClick = () => { Plugin.UIManager.AddPanel(PanelType.BoxList); };
            }

            if (Settings.IsFamStatsPanelEnabled)
            {
                var famStatsButton = UIFactory.CreateButton(_uiAnchor, "FamStatsButton", "Fam Stats");
                UIFactory.SetLayoutElement(famStatsButton.GameObject, ignoreLayout: false, minWidth: 80, minHeight: 25);
                _objectsList.Add(famStatsButton.GameObject);
                famStatsButton.OnClick = () => { Plugin.UIManager.AddPanel(PanelType.FamStats); };
            }

            if (Settings.IsBindButtonEnabled)
            {
                var unbindButton = UIFactory.CreateButton(_uiAnchor, "FamStatsButton", "Unbind");
                UIFactory.SetLayoutElement(unbindButton.GameObject, ignoreLayout: false, minWidth: 80, minHeight: 25);
                _objectsList.Add(unbindButton.GameObject);
                unbindButton.OnClick = () =>
                {
                    unbindButton.Component.interactable = false;
                    MessageService.EnqueueMessage(MessageService.BCCOM_UNBINDFAM);
                    TimerHelper.OneTickTimer(2000, () => unbindButton.Component.interactable = true);
                };

                var bindLastButton = UIFactory.CreateButton(_uiAnchor, "FamBindLastButton", "Bind Last");
                UIFactory.SetLayoutElement(bindLastButton.GameObject, ignoreLayout: false, minWidth: 80, minHeight: 25);
                _objectsList.Add(bindLastButton.GameObject);
                bindLastButton.OnClick = () =>
                {
                    if (string.IsNullOrEmpty(Settings.LastBindCommand))
                        return;
                    bindLastButton.Component.interactable = false;
                    MessageService.EnqueueMessage(Settings.LastBindCommand);
                    TimerHelper.OneTickTimer(2000, () => bindLastButton.Component.interactable = true);
                };
            }

            if (Settings.IsCombatButtonEnabled)
            {
                var combatToggle = UIFactory.CreateToggle(_uiAnchor, "FamToggleCombatButton", out var toggle, out var combatText);
                combatText.text = "Combat Mode";
                combatText.fontSize = 12;
                toggle.onValueChanged.AddListener(value =>
                {
                    toggle.interactable = false;
                    MessageService.EnqueueMessage(MessageService.BCCOM_COMBAT);
                    TimerHelper.OneTickTimer(2000, () => toggle.interactable = true);
                });
                UIFactory.SetLayoutElement(combatToggle, ignoreLayout: false, minWidth: 110, minHeight: 25);
            }

            var scaleButton = UIFactory.CreateButton(_uiAnchor, "ScaleButton", "*");
            UIFactory.SetLayoutElement(scaleButton.GameObject, ignoreLayout: false, minWidth: 25, minHeight: 25);
            _objectsList.Add(scaleButton.GameObject);
            _scaleButtonData = new UIScaleSettingButton();
            scaleButton.OnClick = () =>
            {
                _scaleButtonData.PerformAction();
                var panel = Plugin.UIManager.GetPanel<FamStatsPanel>();
                if(panel != null && panel.UIRoot.active)
                    panel.RecalculateHeight();
            };

            if (Plugin.IS_TESTING)
            {
                var b = UIFactory.CreateButton(_uiAnchor, "TestButton", "T");
                UIFactory.SetLayoutElement(b.GameObject, ignoreLayout: false, minWidth: 25, minHeight: 25);
                _objectsList.Add(scaleButton.GameObject);
                b.OnClick = () => Plugin.UIManager.AddPanel(PanelType.TestPanel);
            }
        }

        protected override void LateConstructUI()
        {
            base.LateConstructUI();

            if (!Settings.UseHorizontalContentLayout)
                ForceRecalculateBasePanelWidth(_objectsList);
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
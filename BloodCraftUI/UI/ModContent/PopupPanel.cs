
using BloodCraftEZLife.Config;

using BloodCraftEZLife.UI.CustomLib.Cells;
using BloodCraftEZLife.UI.CustomLib.Cells.Handlers;
using BloodCraftEZLife.UI.ModContent.Data;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.UI.UniverseLib.UI.Panels;
using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;
using MS.Internal.Xml.XPath;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BloodCraftEZLife.UI.ModContent
{
    internal class PopupPanel : PanelBase
    {
        public override string PanelId => "PopupPanel";
        public override int MinWidth => (int)480;
        public override int MinHeight => (int)240;
        public override int MaxWidth => (int)600;

        public override Vector2 DefaultAnchorMin => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultAnchorMax => new Vector2(0.5f, 0.5f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 0.5f);

        public override bool CanDrag => false;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.None;
        public override PanelType PanelType => PanelType.PullPanel;
        private CanvasGroup _canvasGroup;
        private LabelRef _messageLabel;
        private float timer = 0f;

        public PopupPanel(UIBase owner) : base(owner)
        {
            SetTitle("Settings");
        }
        protected override void ConstructPanelContent()
        {
            _canvasGroup = UIRoot.AddComponent<CanvasGroup>();
            _messageLabel = UIFactory.CreateLabel(UIRoot, "Popup Message","Message",fontSize:64,color:new Color32(226,32,32,255), outlineColor: new Color32(226, 32, 32, 85), outlineWidth:0.15f);
            
            Image BgImg = ContentRoot.GetComponent<UnityEngine.UI.Image>();
            Color bgColor = BgImg.color;
            bgColor.a = 0.0f;
            BgImg.color = bgColor;
            _messageLabel.TextMesh.m_fontWeight = FontWeight.Bold;
            HideMessage();
            //TeleportListTeleportList = 600,417|35.666626,38.5|False
            // Hide
            //_canvasGroup.alpha = 0f;
            //_canvasGroup.interactable = false;
            //_canvasGroup.blocksRaycasts = false;
        }

        public override void Update()
        {
            //visible lets fade in or out
            if (_canvasGroup.alpha > 0f)
            {
                float alpha= alpha = Mathf.Lerp(1f, 0f,timer / 15);
                Color originalColor = _messageLabel.TextMesh.color;

                _messageLabel.TextMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            }
            timer += Time.deltaTime;
            if (timer >= 15)
            {
                timer = 0f;
                HideMessage();
            }

            base.Update();
            // Call update on the panels that need it
        }

        public void HideMessage()
        {
            // Hide
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            
        }

        public void ShowMessage(string message)
        {
            timer = 0f;
            _messageLabel.TextMesh.text = message;
            // Show
            _canvasGroup.alpha = 1f;
        }

        protected override void LateConstructUI()
        {
            base.LateConstructUI();
            Rect.sizeDelta = new Vector2(640, 417);
            Rect.anchoredPosition = new Vector2(35, 38);
            EnsureValidSize();
            EnsureValidPosition();
        }
    }
}

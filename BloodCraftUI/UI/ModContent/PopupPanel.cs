using BloodCraftEZLife.Services;
using BloodCraftEZLife.UI.ModContent.Data;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.UI.UniverseLib.UI.Panels;
using System.Collections.Generic;
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


        public override Vector2 DefaultAnchorMin => new Vector2(0f, 0f);
        public override Vector2 DefaultAnchorMax => new Vector2(1f, 1f);
        public override Vector2 DefaultPivot => new Vector2(0.5f, 0.5f);

        public override bool CanDrag => false;
        public override PanelDragger.ResizeTypes CanResize => PanelDragger.ResizeTypes.None;
        public override PanelType PanelType => PanelType.PopupPanel;

        private CanvasGroup _canvasGroup;
        private LabelRef _messageLabel;        // top-left
        private LabelRef _smallMessageLabel;   // bottom-left

        private float _normalTimer = 0f;
        private float _smallTimer = 0f;

        public struct Message
        {
            public string message { get; set; } // Using an auto-property
            public float fadetime { get; set; }
            public Vector2 vector { get; set; }
            
            
        }
        public enum MessageType
        {
            Small,
            Normal
        }

        private readonly List<Message> _normalMessages = new List<Message>();
        private readonly List<Message> _smallMessages = new List<Message>();

        

        public PopupPanel(UIBase owner) : base(owner)
        {
            SetTitle("Settings");
        }
        protected override void ConstructPanelContent()
        {
            
            Image BgImg = ContentRoot.GetComponent<UnityEngine.UI.Image>();
            Color bgColor = BgImg.color;
            bgColor.a = 0.0f;
            BgImg.color = bgColor;
            _canvasGroup = ContentRoot.AddComponent<CanvasGroup>();
            _messageLabel = UIFactory.CreateLabel(_canvasGroup.gameObject, "Popup Message", "Message", fontSize: 64, color: new Color32(226, 32, 32, 255), outlineColor: new Color32(226, 32, 32, 85), outlineWidth: 0.15f);
            _smallMessageLabel = UIFactory.CreateLabel(_canvasGroup.gameObject, "Small Popup Message", "SmallMessage", fontSize: 32, color: new Color32(226, 226, 32, 255), outlineColor: new Color32(226, 226, 32, 85), outlineWidth: 0.1f);
            _messageLabel.TextMesh.m_fontWeight = FontWeight.Bold;
            //_messageLabel.TextMesh.alignment = TextAlignmentOptions.Left;

            RectTransform rect = _messageLabel.GameObject.GetComponent<RectTransform>();
            // Set anchor and pivot to middle - upper - right
            
            rect.sizeDelta = new Vector2(400f, 70f);
            //1440

            // --- Small message label (bottom-left) ---
            _smallMessageLabel.TextMesh.m_fontWeight = FontWeight.Bold;


            RectTransform rectSmall1 = _smallMessageLabel.GameObject.GetComponent<RectTransform>();
            RectTransform rectSmall = _smallMessageLabel.TextMesh.GetComponent<RectTransform>();
            rectSmall.sizeDelta = new Vector2(150f,70f);
            float xx, yy;
            xx= Owner.Scaler.referenceResolution.x * -1;
            xx = xx + rectSmall.sizeDelta.x + 20f;
            yy = (Owner.Scaler.referenceResolution.y / 2f) - 70f;
            //var pan = Plugin.UIManager.GetPanel<PullItemsPanel>();
            //RectTransform rect = _messageLabel.GameObject.GetComponent<RectTransform>();
            //rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -149f);
            //RectTransform rect2 = _smallMessageLabel.GameObject.GetComponent<RectTransform>();
            //rect2.anchoredPosition = new Vector2(280f, pan._amountSlider.value);

            // Hide both initially
            _messageLabel.TextMesh.text = "";
            _smallMessageLabel.TextMesh.text = "";
            _canvasGroup.alpha = 1f; // keep canvas alive, labels control visibility
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public override void Update()
        {
            base.Update();
            RectTransform rectSmall = _canvasGroup.GetComponent<RectTransform>();
            RectTransform rectContent = ContentRoot.GetComponent<RectTransform>();
            UpdateQueue(_normalMessages, _messageLabel ,ref _normalTimer);
            UpdateQueue(_smallMessages, _smallMessageLabel, ref _smallTimer);
            
            

        }

        private void UpdateQueue(List<Message> list, LabelRef label, ref float timer)
        {
            if (list.Count == 0)
            {
                label.TextMesh.text = "";
                return;
            }
            
            // Fading
            float alpha = Mathf.Lerp(1f, 0f, timer / list[0].fadetime); // 5 sec fade
            Color c = label.TextMesh.color;
            label.TextMesh.color = new Color(c.r, c.g, c.b, alpha);
            var pan = Plugin.UIManager.GetPanel<PullItemsPanel>();
            RectTransform rect = _messageLabel.GameObject.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -149f);
            
            RectTransform rect2 = _smallMessageLabel.GameObject.GetComponent<RectTransform>();

            rect2.anchoredPosition = new Vector2(280f, -92f);

            timer += Time.deltaTime;
            if (timer >= list[0].fadetime)
            {
                timer = 0f;
                list.RemoveAt(0);
                if (list.Count > 0)
                {
                    label.TextMesh.text = list[0].message;
                }
                else
                {
                    label.TextMesh.text = "";
                }
            }
        }

        public void ShowMessage(string message,float fadetime, MessageType type = MessageType.Normal)
        {
            
            if (type == MessageType.Normal)
            {
                _normalMessages.Add(new Message { message = message,fadetime = fadetime,vector = new Vector2(Owner.Scaler.referenceResolution.x/2f,300f) });
                if (_normalMessages.Count == 1)
                {
                    _normalTimer = 0f;
                    _messageLabel.TextMesh.text = message;
                }
            }
            else
            {
                _smallMessages.Add(new Message { message = message, fadetime = fadetime, vector = new Vector2(100f, Owner.Scaler.referenceResolution.y-120f) });
                if (_smallMessages.Count == 1)
                {
                    _smallTimer = 0f;
                    _smallMessageLabel.TextMesh.text = message;
                }
            }
        }

        protected override void LateConstructUI()
        {
            base.LateConstructUI();
            
            //Rect.sizeDelta = Owner.Scaler.referenceResolution;
            
            EnsureValidSize();
            EnsureValidPosition();
        }
    }
}

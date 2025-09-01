using BloodCraftEZLife.Config;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace BloodCraftEZLife.UI.ModContent.CustomElements
{
    public class ButtonCell : CellBase, IFormedCell
    {
        public ButtonRef Button { get; set; }
        public Image IconImage;
        public int CurrentDataIndex { get; set; }
        public override float DefaultHeight => 25f;
        public void DoSelect(bool selected)
        {
            ColorBlock colors = Button.Component.colors;
            if (selected)

                colors.normalColor = new Color(0.65f, 0.35f, 0.26f).GetTransparent(Settings.UITransparency);
            else
                colors.normalColor = new Color(0.11f, 0.11f, 0.11f).GetTransparent(Settings.UITransparency);
            Button.Component.colors = colors;
        }

        public override GameObject CreateContent(GameObject parent)
        {
            UIRoot = UIFactory.CreateHorizontalGroup(parent, "ButtonCell", true, false, true, true, 2, default,
                new Color(0.11f, 0.11f, 0.11f).GetTransparent(Settings.UITransparency), TextAnchor.MiddleCenter);
            Rect = UIRoot.GetComponent<RectTransform>();
            Rect.anchorMin = new Vector2(0, 1);
            Rect.anchorMax = new Vector2(0, 1);
            Rect.pivot = new Vector2(0.5f, 1);
            Rect.sizeDelta = new Vector2(25, 25);
            UIFactory.SetLayoutElement(UIRoot, minWidth: 100, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);

            UIRoot.SetActive(false);
            // ITEM ICON
            var imageIcon = UIFactory.CreateUIObject("IconItem-", UIRoot);
            IconImage = imageIcon.AddComponent<Image>();
            UIFactory.SetLayoutElement(imageIcon, minWidth: 60, minHeight: 60, flexibleHeight: 0, preferredHeight: 60, flexibleWidth: 0, preferredWidth: 60);

            Button = UIFactory.CreateButton(UIRoot, "NameButton", "Name", new ColorBlock
            {
                normalColor = new Color(0.11f, 0.11f, 0.11f).GetTransparent(Settings.UITransparency),
                disabledColor = new Color(0.3f, 0.3f, 0.3f).GetTransparent(Settings.UITransparency),
                highlightedColor = new Color(0.16f, 0.16f, 0.16f).GetTransparent(Settings.UITransparency),
                pressedColor = new Color(0.05f, 0.05f, 0.05f).GetTransparent(Settings.UITransparency),
                selectedColor = new Color(0.65f, 0.35f, 0.26f).GetTransparent(Settings.UITransparency)
            });
            UIFactory.SetLayoutElement(Button.Component.gameObject, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);
            var buttonText = Button.Component.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.overflowMode = TextOverflowModes.Overflow;
            buttonText.alignment = TextAlignmentOptions.MidlineLeft;
            buttonText.margin = new Vector4(5, 0, 0, 0);

            Button.OnClick += () => { OnClick?.Invoke(CurrentDataIndex); };

            return UIRoot;
        }

        public Action<int> OnClick { get; set; }
    }
}

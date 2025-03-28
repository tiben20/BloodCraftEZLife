using System;
using BloodCraftUI.NewUI.UICore.UniverseLib.UI;
using BloodCraftUI.NewUI.UICore.UniverseLib.UI.Models;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace BloodCraftUI.NewUI.UICore.UI.Cells
{
    public class ButtonCell : CellBase
    {
        public ButtonRef Button { get; set; }
        public int CurrentDataIndex { get; set; }
        public override float DefaultHeight => 25f;

        public override GameObject CreateContent(GameObject parent)
        {
            UIRoot = UIFactory.CreateHorizontalGroup(parent, "ButtonCell", true, false, true, true, 2, default,
                new Color(0.11f, 0.11f, 0.11f), TextAnchor.MiddleCenter);
            Rect = UIRoot.GetComponent<RectTransform>();
            Rect.anchorMin = new Vector2(0, 1);
            Rect.anchorMax = new Vector2(0, 1);
            Rect.pivot = new Vector2(0.5f, 1);
            Rect.sizeDelta = new Vector2(25, 25);
            UIFactory.SetLayoutElement(UIRoot, minWidth: 100, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);

            UIRoot.SetActive(false);

            this.Button = UIFactory.CreateButton(UIRoot, "NameButton", "Name", new ColorBlock
            {
                normalColor = new(0.11f, 0.11f, 0.11f),
                disabledColor = new(1, 1, 1, 0),
                highlightedColor = new(0.16f, 0.16f, 0.16f),
                pressedColor = new(0.05f, 0.05f, 0.05f)
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

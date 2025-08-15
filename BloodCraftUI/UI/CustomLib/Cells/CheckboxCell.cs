using System;
using BloodCraftEZLife.Config;
using BloodCraftEZLife.UI.UniverseLib.UI;
using BloodCraftEZLife.UI.UniverseLib.UI.Models;
using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;
using BloodCraftEZLife.Utils;
using TMPro;
using Unity.Entities.DebugProxies;
using UnityEngine;
using UnityEngine.UI;

namespace BloodCraftEZLife.UI.CustomLib.Cells
{
    public interface IFormedCheckbox : ICell
    {
        public int CurrentDataIndex { get; set; }
        public Action<int,bool> OnValueChanged { get; set; }
    }

    public class CheckboxCell : CellBase, IFormedCheckbox
    {
        public ToggleRef Checkbox { get; set; }
        public int CurrentDataIndex { get; set; }
        public override float DefaultHeight => 25f;

        public override GameObject CreateContent(GameObject parent)
        {
            //creating the group horizontal group
            UIRoot = UIFactory.CreateHorizontalGroup(parent, "CheckboxCell", true, false, true, true,2, default,
                new Color(0.11f, 0.11f, 0.11f).GetTransparent(Settings.UITransparency), TextAnchor.MiddleCenter);
            Rect = UIRoot.GetComponent<RectTransform>();
            Rect.anchorMin = new Vector2(0, 1);
            Rect.anchorMax = new Vector2(0, 1);
            Rect.pivot = new Vector2(0.5f, 1);
            Rect.sizeDelta = new Vector2(25, 25);
            UIFactory.SetLayoutElement(UIRoot, minWidth: 100, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);

            UIRoot.SetActive(false);

            Checkbox = UIFactory.CreateToggle(UIRoot, "Checkbox", new Color(0.11f, 0.11f, 0.11f).GetTransparent(Settings.UITransparency));

            UIFactory.SetLayoutElement(Checkbox.GameObject, flexibleWidth: 9999, minHeight: 25, flexibleHeight: 0);

            Checkbox.OnValueChanged += (bool newvalue) => { OnValueChanged?.Invoke(CurrentDataIndex, newvalue); };
            

            return UIRoot;
        }

        public Action<int, bool> OnValueChanged { get; set; }
    }
}

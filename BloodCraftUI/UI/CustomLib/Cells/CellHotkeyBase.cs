using BloodCraftEZLife.UI.UniverseLib.UI.Widgets.ScrollView;
using UnityEngine;

namespace BloodCraftEZLife.UI.CustomLib.Cells
{
    public abstract class CellHotkeyBase : ICell
    {
        public GameObject UIRoot { get; protected set; }
        public virtual float DefaultHeight { get; } = 25f;
        public virtual bool Enabled => UIRoot.activeSelf;
        public RectTransform Rect { get; set; }

        public abstract GameObject CreateContent(GameObject parent);

        public virtual void Enable()
        {
            UIRoot?.SetActive(true);
        }

        public abstract void Disable();

    }
}

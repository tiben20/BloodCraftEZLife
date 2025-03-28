using BloodCraftUI.NewUI.UICore.UniverseLib.UI.Widgets.ScrollView;
using UnityEngine;

namespace BloodCraftUI.NewUI.UICore.UI.Cells
{
    public abstract class CellBase : ICell
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

        public virtual void Disable()
        {
            UIRoot?.SetActive(false);
        }
    }
}

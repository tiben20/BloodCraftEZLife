using BloodCraftUI.NewUI.UICore.UniverseLib.UI.ObjectPool;
using UnityEngine;

namespace BloodCraftUI.NewUI.UICore.UniverseLib.UI.Widgets.ScrollView;

public interface ICell : IPooledObject
{
    bool Enabled { get; }

    RectTransform Rect { get; set; }

    void Enable();
    void Disable();
}
using BloodCraftUI.NewUI.UniverseLib.UI.ObjectPool;
using UnityEngine;

namespace BloodCraftUI.NewUI.UniverseLib.UI.Widgets.ScrollView;

public interface ICell : IPooledObject
{
    bool Enabled { get; }

    RectTransform Rect { get; set; }

    void Enable();
    void Disable();
}